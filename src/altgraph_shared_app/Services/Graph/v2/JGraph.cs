using altgraph_shared_app.Options;
using altgraph_shared_app.Services.Graph.v2.Structs;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using QuikGraph;
using QuikGraph.Algorithms;
using QuikGraph.Algorithms.Ranking;

namespace altgraph_shared_app.Services.Graph.v2
{
  public class JGraph : IJGraph
  {
    public string Domain { get; set; } = string.Empty;
    public string Source { get; set; } = string.Empty;
    public DateTime RefreshDate { get; set; }
    public long RefreshMs { get; set; }
    public IMutableGraph<string, Edge<string>>? Graph { get; set; } = null;
    private readonly ILogger _logger;
    private IJGraphBuilder _graphBuilder;
    private readonly ImdbOptions _imdbOptions;

    public event EventHandler<JGraphStartedLoadingProgressEventArgs>? JGraphStartedLoadingProgress;
    public event EventHandler<JGraphLoadingProgressEventArgs>? JGraphLoadingProgress;
    public event EventHandler<JGraphFinishedLoadingProgressEventArgs>? JGraphFinishedLoadingProgress;

    public JGraph(ILogger<JGraph> logger, IJGraphBuilder graphBuilder, IOptions<ImdbOptions> imdbOptions)
    {
      _logger = logger;
      _graphBuilder = graphBuilder;
      _graphBuilder.JGraphBuilderStartedLoadingProgress += OnJGraphStartedLoadingProgress;
      _graphBuilder.JGraphBuilderLoadingProgress += OnJGraphLoadingProgress;
      _graphBuilder.JGraphBuilderFinishedLoadingProgress += OnJGraphFinishedLoadingProgress;
      _imdbOptions = imdbOptions.Value;
      Domain = _imdbOptions.GraphDomain;
      Source = _imdbOptions.GraphSource;
    }

    public int[] GetVertexAndEdgeCounts()
    {
      int[] counts = new int[2];

      if (Graph is IVertexSet<string> graph0)
      {
        counts[0] = graph0.Vertices.Count();
      }

      if (Graph is IEdgeSet<string, Edge<string>> graph1)
      {
        counts[1] = graph1.Edges.Count();
      }

      return counts;
    }

    public IEnumerable<Edge<string>>? GetShortestPath(string v1, string v2)
    {
      _logger.LogDebug($"GetShortestPath, v1: {v1} to v2: {v2}");
      long start = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
      if (!IsVertexPresent(v1))
      {
        return null;
      }
      if (!IsVertexPresent(v2))
      {
        return null;
      }
      Func<Edge<string>, double> edgeCost = edge => 1; // Constant cost
      IEnumerable<Edge<string>>? path = null;

      if (Graph != null)
      {
        TryFunc<string, IEnumerable<Edge<string>>> tryGetPaths;

        if (Graph.IsDirected)
        {
          tryGetPaths = (Graph as IBidirectionalGraph<string, Edge<string>>).ShortestPathsDijkstra(edgeCost, v1);
        }
        else
        {
          tryGetPaths = (Graph as IUndirectedGraph<string, Edge<string>>).ShortestPathsDijkstra(edgeCost, v1);
        }
        long elapsed = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - start;

        if (!tryGetPaths(v2, out path))
        {
          _logger.LogWarning("path is null");
        }
        else
        {
          _logger.LogDebug($"elapsed milliseconds: {elapsed}");
          _logger.LogDebug($"path Count:       {path.Count()}");
          _logger.LogDebug($"path StartVertex:  {path.First()}");
          _logger.LogDebug($"path EndVertex:    {path.Last()}");
        }
      }
      return path;
    }


    public EdgesStruct? GetShortestPathAsEdgesStruct(string v1, string v2)
    {
      long startMs = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
      IEnumerable<Edge<string>>? path = GetShortestPath(v1, v2);

      if (path != null)
      {
        EdgesStruct edgesStruct = new EdgesStruct();
        edgesStruct.ElapsedMs = (DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - startMs);
        edgesStruct.Vertex1 = v1;
        edgesStruct.Vertex2 = v2;
        foreach (Edge<string> e in path)
        {
          EdgeStruct? edgeStruct = ParseDefaultEdge(e);
          if (edgeStruct != null)
          {
            edgesStruct.AddEdge(edgeStruct);
          }
        }
        return edgesStruct;
      }
      else
      {
        return null;
      }
    }

    public HashSet<Edge<string>>? EdgesOf(string v)
    {
      if (IsVertexPresent(v))
      {
        HashSet<Edge<string>> edges = new HashSet<Edge<string>>();
        if (Graph != null)
        {
          if (Graph.IsDirected)
          {
            foreach (Edge<string> edge in ((IBidirectionalGraph<string, Edge<string>>)Graph).InEdges(v))
            {
              edges.Add(edge);
            }
            foreach (Edge<string> edge in ((IBidirectionalGraph<string, Edge<string>>)Graph).OutEdges(v))
            {
              edges.Add(edge);
            }
          }
          else
          {
            foreach (Edge<string> edge in ((IUndirectedGraph<string, Edge<string>>)Graph).AdjacentEdges(v))
            {
              edges.Add(edge);
            }
          }
        }
        return edges;
      }
      return null;
    }

    public JStarNetwork? StarNetworkFor(string rootVertex, int degrees)
    {
      JStarNetwork? star = null;
      if (IsVertexPresent(rootVertex))
      {
        star = new JStarNetwork(rootVertex, degrees);

        for (int d = 1; d <= degrees; d++)
        {
          List<string> unvisitedList = star.GetUnvisitedList();
          star.ResetUnvisitedSet();

          _logger.LogDebug($"networkFor, unvisitedList size: {unvisitedList.Count} degree: {d}");
          for (int i = 0; i < unvisitedList.Count; i++)
          {
            string v = unvisitedList[i];
            HashSet<Edge<string>>? edges = EdgesOf(v);
            if (edges != null)
            {
              star.AddOutEdgesFor(v, edges, d);
            }
          }
        }
      }
      star?.Finish();
      return star;
    }

    public HashSet<Edge<string>>? IncomingEdgesOf(string v)
    {
      if (Graph is IBidirectionalGraph<string, Edge<string>> && IsVertexPresent(v))
      {
        return ((IBidirectionalGraph<string, Edge<string>>)Graph)?.InEdges(v).ToHashSet();
      }
      return null;
    }

    public int DegreeOf(string v)
    {
      if (Graph != null && IsVertexPresent(v))
      {
        if (Graph.IsDirected)
        {
          return ((IBidirectionalGraph<string, Edge<string>>)Graph).Degree(v);
        }
        else
        {
          return ((IUndirectedGraph<string, Edge<string>>)Graph).AdjacentEdges(v).Count();
        }
      }
      return -1;
    }

    public int InDegreeOf(string v)
    {
      if (Graph != null && Graph is IBidirectionalGraph<string, Edge<string>> && IsVertexPresent(v))
      {
        return ((IBidirectionalGraph<string, Edge<string>>)Graph).InDegree(v);
      }
      return -1;
    }

    public double PageRankForVertex(string v)
    {
      if (Graph != null && Graph is IBidirectionalGraph<string, Edge<string>> && IsVertexPresent(v))
      {
        PageRankAlgorithm<string, Edge<string>> pr = new PageRankAlgorithm<string, Edge<string>>((IBidirectionalGraph<string, Edge<string>>)Graph);
        pr.Compute();
        return pr.Ranks[v];
        //return pr.GetVertexScore(v);
      }
      return -1.0;
    }

    public IDictionary<string, double>? PageRankForAll()
    {
      if (Graph != null && Graph is IBidirectionalGraph<string, Edge<string>>)
      {
        PageRankAlgorithm<string, Edge<string>> pr = new PageRankAlgorithm<string, Edge<string>>((IBidirectionalGraph<string, Edge<string>>)Graph);
        pr.MaxIterations = 100;
        pr.Damping = 0.85;
        pr.Tolerance = 1.0e-4;
        pr.Compute();
        return pr.Ranks;
      }
      return null;
    }

    public List<JRank> SortedPageRanks(int maxCount)
    {
      List<JRank> ranks = new List<JRank>();
      VertexValueStruct vvStruct = new VertexValueStruct();
      IDictionary<string, double>? scores = PageRankForAll();
      if (scores != null)
      {
        foreach (string vertex in scores.Keys)
        {
          double value = scores[vertex];
          vvStruct.AddRank(vertex, value);
        }
        vvStruct.Sort();
        for (int i = 0; i < maxCount; i++)
        {
          ranks.Add(vvStruct.GetRank(i));
        }
      }
      return ranks;
    }

    public double CentralityOfVertex(string v)
    {
      throw new NotImplementedException();
      // if (IsVertexPresent(v))
      // {
      //   KatzCentrality kc = new KatzCentrality(Graph);
      //   return kc.getVertexScore(v);
      // }
      // return -1.0;
    }

    public Dictionary<string, double> CentralityRankAll()
    {
      throw new NotImplementedException();
      // KatzCentrality kc = new KatzCentrality(Graph);
      // return kc.getScores();
    }

    public async Task RefreshAsync(bool directed = false)
    {
      long t1 = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
      IMutableGraph<string, Edge<string>>? newGraph = null;
      _logger.LogDebug($"JGraph refresh(), domain: {Domain}");

      try
      {
        if (Domain.Equals(_imdbOptions.GraphDomain, StringComparison.OrdinalIgnoreCase))
        {
          _graphBuilder.Directed = directed;
          newGraph = await _graphBuilder.BuildImdbGraphAsync();
          if (newGraph != null)
          {
            RefreshMs = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - t1;
            RefreshDate = DateTime.Now;
            _logger.LogDebug($"JGraph refresh() - replacing Graph with newGraph, elapsed ms: {RefreshMs}");
            Graph = newGraph;
          }
        }
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, ex.Message);
      }
    }

    public bool IsVertexPresent(string v)
    {
      if (Graph != null && v != null)
      {
        if (Graph.IsDirected)
        {
          return ((IBidirectionalGraph<string, Edge<string>>)Graph).ContainsVertex(v);
        }
        else
        {
          return ((IUndirectedGraph<string, Edge<string>>)Graph).ContainsVertex(v);
        }
      }
      return false;
    }

    private EdgeStruct? ParseDefaultEdge(Edge<string>? e)
    {
      if (e != null)
      {
        string[] tokens = e.ToString().Split(" -> ");
        if (tokens.Length == 2)
        {
          EdgeStruct edgeStruct = new EdgeStruct();
          edgeStruct.V1Value = tokens[0].Replace('(', ' ').Trim();
          edgeStruct.V2Value = tokens[1].Replace(')', ' ').Trim();
          return edgeStruct;
        }
      }
      return null;
    }

    private void OnJGraphStartedLoadingProgress(object? sender, JGraphBuilderStartedLoadingProgressEventArgs e)
    {
      OnRaiseJGraphStartedLoadingProgressEvent(new JGraphStartedLoadingProgressEventArgs(e.MaxCount));
    }

    private void OnJGraphLoadingProgress(object? sender, JGraphBuilderLoadingProgressEventArgs e)
    {
      OnRaiseJGraphLoadingProgressEvent(new JGraphLoadingProgressEventArgs(e.Progress));
    }

    private void OnJGraphFinishedLoadingProgress(object? sender, JGraphBuilderFinishedLoadingProgressEventArgs e)
    {
      OnRaiseJGraphFinishedLoadingProgressEvent(new JGraphFinishedLoadingProgressEventArgs(e.Count));
    }

    protected virtual void OnRaiseJGraphStartedLoadingProgressEvent(JGraphStartedLoadingProgressEventArgs e)
    {
      EventHandler<JGraphStartedLoadingProgressEventArgs>? raiseEvent = JGraphStartedLoadingProgress;

      if (raiseEvent != null)
      {
        raiseEvent(this, e);
      }
    }

    protected virtual void OnRaiseJGraphLoadingProgressEvent(JGraphLoadingProgressEventArgs e)
    {
      EventHandler<JGraphLoadingProgressEventArgs>? raiseEvent = JGraphLoadingProgress;

      if (raiseEvent != null)
      {
        raiseEvent(this, e);
      }
    }

    protected virtual void OnRaiseJGraphFinishedLoadingProgressEvent(JGraphFinishedLoadingProgressEventArgs e)
    {
      EventHandler<JGraphFinishedLoadingProgressEventArgs>? raiseEvent = JGraphFinishedLoadingProgress;

      if (raiseEvent != null)
      {
        raiseEvent(this, e);
      }
    }
  }

  public class JGraphStartedLoadingProgressEventArgs
  {
    public long MaxCount { get; private set; }
    public JGraphStartedLoadingProgressEventArgs(long maxCount)
    {
      MaxCount = maxCount;
    }
  }

  public class JGraphLoadingProgressEventArgs
  {
    public long Progress { get; private set; }
    public JGraphLoadingProgressEventArgs(long progress)
    {
      Progress = progress;
    }
  }

  public class JGraphFinishedLoadingProgressEventArgs
  {
    public long Count { get; private set; }
    public JGraphFinishedLoadingProgressEventArgs(long count)
    {
      Count = count;
    }
  }
}