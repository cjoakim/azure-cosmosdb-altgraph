using System.Text.Json;
using System.Text.Json.Serialization;
using altgraph_shared_app.Models.Imdb;
using altgraph_shared_app.Options;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Fluent;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using QuikGraph;

namespace altgraph_shared_app.Services.Graph.v2
{
  public class JGraphBuilder : IJGraphBuilder
  {
    public string Uri { get; set; } = string.Empty;
    public string Key { get; set; } = string.Empty;
    public string DbName { get; set; } = string.Empty;
    public string Source { get; set; } = string.Empty;
    public bool Directed { get; set; } = false;
    private ILogger<JGraphBuilder> _logger;
    private CosmosOptions _cosmosOptions;
    private ImdbOptions _imdbOptions;
    public event EventHandler<JGraphBuilderStartedLoadingProgressEventArgs>? JGraphBuilderStartedLoadingProgress;
    public event EventHandler<JGraphBuilderLoadingProgressEventArgs>? JGraphBuilderLoadingProgress;
    public event EventHandler<JGraphBuilderFinishedLoadingProgressEventArgs>? JGraphBuilderFinishedLoadingProgress;

    public JGraphBuilder(ILogger<JGraphBuilder> logger, IOptions<CosmosOptions> cosmosOptions, IOptions<ImdbOptions> imdbOptions)
    {
      _logger = logger;
      _cosmosOptions = cosmosOptions.Value;
      _imdbOptions = imdbOptions.Value;
      Source = _imdbOptions.GraphSource;
    }

    public async Task<IMutableGraph<string, Edge<string>>?> BuildImdbGraphAsync()
    {
      _logger.LogWarning($"buildImdbGraph, source:   {Source}");
      _logger.LogWarning($"buildImdbGraph, directed: {Directed}");

      try
      {
        switch (Source)
        {
          case Constants.IMDB_GRAPH_SOURCE_DISK:
            return LoadImdbGraphFromDisk(Directed);
          case Constants.IMDB_GRAPH_SOURCE_COSMOS:
            return await LoadImdbGraphFromCosmosAsync(Directed);
          default:
            _logger.LogWarning($"undefined graph source: {Source}");
            return null;
        }
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, $"buildImdbGraph, exception: {ex.Message}");
        throw;
      }
    }

    private IMutableGraph<string, Edge<string>> CreateGraphObject(bool directed)
    {
      IMutableGraph<string, Edge<string>> graph;

      if (directed)
      {
        graph = new BidirectionalGraph<string, Edge<string>>();
      }
      else
      {
        graph = new UndirectedGraph<string, Edge<string>>();
      }
      return graph;
    }

    private IMutableGraph<string, Edge<string>> LoadImdbGraphFromDisk(bool directed)
    {
      throw new NotImplementedException();
      // IMutableGraph<string, Edge<string>> graph = CreateGraphObject(directed);

      // JsonLoader jsonLoader = new JsonLoader();
      // Dictionary<string, Movie> moviesHash = new Dictionary<string, Movie>();
      // jsonLoader.readMovieDocuments(moviesHash, true);
      // CheckMemory(true, true, "loadImdbGraphFromDisk - after reading movies from file");

      // Iterator<string> moviesIt = moviesHash.keySet().iterator();
      // long movieNodesCreated = 0;
      // long personNodesCreated = 0;
      // long edgesCreated = 0;

      // while (moviesIt.hasNext())
      // {
      //   string tconst = moviesIt.next();
      //   Movie movie = moviesHash.get(tconst);
      //   if (!graph.ContainsVertex(tconst))
      //   {
      //     graph.AddVertex(tconst);
      //     movieNodesCreated++;
      //   }

      //   Iterator<string> peopleIt = movie.getPeople().iterator();
      //   while (peopleIt.hasNext())
      //   {
      //     string nconst = peopleIt.next();
      //     if (!graph.ContainsVertex(nconst))
      //     {
      //       graph.AddVertex(nconst);
      //       personNodesCreated++;
      //     }
      //     graph.AddEdge(nconst, tconst);  // person-to-movie
      //     edgesCreated++;
      //     if (directed)
      //     {
      //       // just a single edge between vertices
      //     }
      //     else
      //     {
      //       graph.AddEdge(tconst, nconst);  // movie-to-person
      //       edgesCreated++;
      //     }
      //   }
      // }

      // CheckMemory(true, true, "loadImdbGraphFromDisk - after building graph");
      // _logger.LogWarning($"loadImdbGraphFromDisk - movieNodesCreated:  {movieNodesCreated}");
      // _logger.LogWarning($"loadImdbGraphFromDisk - personNodesCreated: {personNodesCreated}");
      // _logger.LogWarning($"loadImdbGraphFromDisk - edgesCreated:       {edgesCreated}");
      // return graph;
    }

    private async Task<IMutableGraph<string, Edge<string>>> LoadImdbGraphFromCosmosAsync(bool directed)
    {
      Uri = _cosmosOptions.ConnectionString;
      //Key = _cosmosOptions.Key;
      DbName = _cosmosOptions.DatabaseId;
      Source = _imdbOptions.GraphSource;
      Directed = _imdbOptions.GraphDirected;

      IMutableGraph<string, Edge<string>> graph = CreateGraphObject(directed);

      CosmosClient client;
      Database database;
      Container container;
      double requestCharge = 0;
      long documentsRead = 0;
      long movieNodesCreated = 0;
      long personNodesCreated = 0;
      long edgesCreated = 0;
      var getDataSql = new QueryDefinition("select * from c where c.pk = @pk")
        .WithParameter("@pk", Constants.DOCTYPE_MOVIE_SEED);
      int pageSize = 1000;
      string? continuationToken = null;

      _logger.LogWarning($"uri:    {Uri}");
      _logger.LogWarning($"key:    {Key}");
      _logger.LogWarning($"dbName: {DbName}");
      _logger.LogWarning($"sql:    {getDataSql.QueryText}");

      long startMs = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
      JsonSerializerOptions jsonSerializerOptions = new JsonSerializerOptions()
      {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
      };
      CosmosSystemTextJsonSerializer cosmosSystemTextJsonSerializer = new CosmosSystemTextJsonSerializer(jsonSerializerOptions);
      client = new CosmosClientBuilder(
        connectionString: Uri
      )
              .WithApplicationPreferredRegions(_cosmosOptions.PreferredLocations)
              .WithConsistencyLevel(ConsistencyLevel.Session)
              .WithContentResponseOnWrite(true)
              .WithCustomSerializer(cosmosSystemTextJsonSerializer)
              .Build();

      database = client.GetDatabase(DbName);
      _logger.LogWarning($"client connected to database Id: {database.Id}");

      container = database.GetContainer(_imdbOptions.SeedContainerName);
      _logger.LogWarning($"container: {container.Id}");

      long dbConnectMs = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

      try
      {
        var requestOptions = new QueryRequestOptions();
        requestOptions.EnableScanInQuery = true;
        var getDataSqlCount = new QueryDefinition("select VALUE COUNT(1) from c where c.pk = @pk")
          .WithParameter("@pk", Constants.DOCTYPE_MOVIE_SEED);

        var queryIterator = container.GetItemQueryIterator<long>(getDataSqlCount);
        var response = await queryIterator.ReadNextAsync();
        long count = response.Resource.FirstOrDefault();

        _logger.LogWarning($"count: {count}");

        OnRaiseJGraphBuilderStartedLoadingProgressEvent(new JGraphBuilderStartedLoadingProgressEventArgs(count));
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Error getting count");
      }

      try
      {
        using (FeedIterator<SeedDocument> feedResponseIterator =
                container.GetItemQueryIterator<SeedDocument>(getDataSql, continuationToken, new QueryRequestOptions { MaxItemCount = pageSize }))
        {
          do
          {
            while (feedResponseIterator.HasMoreResults)
            {
              FeedResponse<SeedDocument> page = await feedResponseIterator.ReadNextAsync();

              OnRaiseJGraphBuilderLoadingProgressEvent(new JGraphBuilderLoadingProgressEventArgs(page.Resource.Count()));

              foreach (SeedDocument doc in page.Resource)
              {
                documentsRead++;
                if ((documentsRead % 10000) == 0)
                {
                  _logger.LogWarning($"{documentsRead} -> {JsonSerializer.Serialize(doc)}");
                }
                string tconst = doc.TargetId;
                ((IMutableVertexSet<string>)graph).AddVertex(tconst);
                movieNodesCreated++;

                for (int i = 0; i < doc.AdjacentVertices.Count(); i++)
                {
                  string nconst = doc.AdjacentVertices[i];
                  if (!((IImplicitVertexSet<string>)graph).ContainsVertex(nconst))
                  {
                    ((IMutableVertexSet<string>)graph).AddVertex(nconst);
                    personNodesCreated++;
                  }
                  ((IMutableEdgeListGraph<string, Edge<string>>)graph).AddEdge(new Edge<string>(nconst, tconst));  // person-to-movie
                  edgesCreated++;

                  if (directed)
                  {
                    // just a single edge between vertices
                  }
                  else
                  {
                    ((IMutableEdgeListGraph<string, Edge<string>>)graph).AddEdge(new Edge<string>(tconst, nconst));  // movie-to-person
                    edgesCreated++;
                  }
                }
              }
              requestCharge = requestCharge + page.RequestCharge;
              continuationToken = page.ContinuationToken;

            }
          }
          while (continuationToken != null);
        }
      }
      catch (Exception ex)
      {
        //t.printStackTrace();
        _logger.LogError(ex, $"loadImdbGraphFromCosmos - exception: {ex.Message}");
      }

      long finishMs = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
      long dbConnectElapsed = dbConnectMs - startMs;
      long dbReadingElapsed = finishMs - dbConnectMs;
      double dbReadingSeconds = (double)dbReadingElapsed / 1000.0;
      long totalElapsed = finishMs - startMs;

      double ruPerSec = (double)requestCharge / dbReadingSeconds;

      //CheckMemory(true, true, "loadImdbGraphFromCosmos - after building graph");
      _logger.LogWarning($"loadImdbGraphFromCosmos - documentsRead:      {documentsRead}");
      _logger.LogWarning($"loadImdbGraphFromCosmos - movieNodesCreated:  {movieNodesCreated}");
      _logger.LogWarning($"loadImdbGraphFromCosmos - personNodesCreated: {personNodesCreated}");
      _logger.LogWarning($"loadImdbGraphFromCosmos - edgesCreated:       {edgesCreated}");
      _logger.LogWarning($"loadImdbGraphFromCosmos - requestCharge:      {requestCharge}");
      _logger.LogWarning($"loadImdbGraphFromCosmos - ru per second:      {ruPerSec}");
      _logger.LogWarning($"loadImdbGraphFromCosmos - db connect ms:      {dbConnectElapsed}");
      _logger.LogWarning($"loadImdbGraphFromCosmos - db read ms:         {dbReadingElapsed}");
      _logger.LogWarning($"loadImdbGraphFromCosmos - total elapsed ms:   {totalElapsed}");

      OnRaiseJGraphBuilderFinishedLoadingProgressEvent(new JGraphBuilderFinishedLoadingProgressEventArgs(documentsRead));

      return graph;
    }

    protected virtual void OnRaiseJGraphBuilderStartedLoadingProgressEvent(JGraphBuilderStartedLoadingProgressEventArgs e)
    {
      EventHandler<JGraphBuilderStartedLoadingProgressEventArgs>? raiseEvent = JGraphBuilderStartedLoadingProgress;

      if (raiseEvent != null)
      {
        raiseEvent(this, e);
      }
    }

    protected virtual void OnRaiseJGraphBuilderLoadingProgressEvent(JGraphBuilderLoadingProgressEventArgs e)
    {
      EventHandler<JGraphBuilderLoadingProgressEventArgs>? raiseEvent = JGraphBuilderLoadingProgress;

      if (raiseEvent != null)
      {
        raiseEvent(this, e);
      }
    }

    protected virtual void OnRaiseJGraphBuilderFinishedLoadingProgressEvent(JGraphBuilderFinishedLoadingProgressEventArgs e)
    {
      EventHandler<JGraphBuilderFinishedLoadingProgressEventArgs>? raiseEvent = JGraphBuilderFinishedLoadingProgress;

      if (raiseEvent != null)
      {
        raiseEvent(this, e);
      }
    }

    // protected MemoryStats CheckMemory(bool doGc, bool display, string note)
    // {
    //   if (doGc)
    //   {
    //     System.gc();
    //   }
    //   MemoryStats ms = new MemoryStats(note);
    //   if (display)
    //   {
    //     try
    //     {
    //       sysout(ms.asDelimitedHeaderLine("|"));
    //       sysout(ms.asDelimitedDataLine("|"));
    //     }
    //     catch (Exception e)
    //     {
    //       sysout("error serializing MemoryStats to JSON");
    //     }
    //   }
    //   return ms;
    // }

    // protected void sysout(string s)
    // {
    //   System.out.println(s);
    // }

  }

  public class JGraphBuilderStartedLoadingProgressEventArgs
  {
    public long MaxCount { get; private set; }
    public JGraphBuilderStartedLoadingProgressEventArgs(long maxCount)
    {
      MaxCount = maxCount;
    }
  }

  public class JGraphBuilderLoadingProgressEventArgs
  {
    public long Progress { get; private set; }
    public JGraphBuilderLoadingProgressEventArgs(long progress)
    {
      Progress = progress;
    }
  }

  public class JGraphBuilderFinishedLoadingProgressEventArgs
  {
    public long Count { get; private set; }
    public JGraphBuilderFinishedLoadingProgressEventArgs(long count)
    {
      Count = count;
    }
  }
}