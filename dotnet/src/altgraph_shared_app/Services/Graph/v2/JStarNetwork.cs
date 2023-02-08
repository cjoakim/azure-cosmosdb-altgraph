using altgraph_shared_app.Services.Graph.v2.Structs;
using QuikGraph;

namespace altgraph_shared_app.Services.Graph.v2
{
  public class JStarNetwork
  {
    public string RootVertex { get; private set; } = string.Empty;
    public int Depth { get; private set; }
    public long StartMs { get; private set; }
    public long FinishMs { get; private set; }
    public List<JStarDegree> Degrees { get; private set; } = new List<JStarDegree>();
    public HashSet<string> VisitedSet { get; private set; } = new HashSet<string>();
    public HashSet<string> UnvisitedSet { get; private set; } = new HashSet<string>();

    public JStarNetwork(string root, int depth)
    {
      this.RootVertex = root;
      this.Depth = depth;
      for (int i = 1; i <= depth; i++)
      {
        Degrees.Add(new JStarDegree(i));
      }
      this.VisitedSet = new HashSet<string>();
      this.UnvisitedSet = new HashSet<string>();
      UnvisitedSet.Add(RootVertex);
    }

    public void AddOutEdgesFor(string vertex, IEnumerable<Edge<string>> outEdges, int degree)
    {
      VisitedSet.Add(vertex);
      Degrees[degree - 1].Add(vertex, outEdges);

      foreach (Edge<string> de in outEdges)
      {
        string[] values = ParseDefaultEdge(de);
        string ev1 = values[0];
        string ev2 = values[1];
        if (!VisitedSet.Contains(ev1))
        {
          UnvisitedSet.Add(ev1);
        }
        if (!VisitedSet.Contains(ev2))
        {
          UnvisitedSet.Add(ev2);
        }
      }
    }

    private string[] ParseDefaultEdge(Edge<string> de)
    {
      string[] result = new string[2];
      result[0] = "?";
      result[1] = "?";

      if (de != null)
      {
        //string[] tokens = de.ToString().Split(":");
        string[] tokens = de.ToString().Split(" -> ");
        if (tokens.Length == 2)
        {
          result[0] = tokens[0].Replace('(', ' ').Trim();
          result[1] = tokens[1].Replace(')', ' ').Trim();
        }
      }
      return result;
    }

    public bool ContainsVertex(string v)
    {
      if (v != null)
      {
        return VisitedSet.Contains(v);
      }
      return false;
    }

    public List<string> GetUnvisitedList()
    {
      List<string> list = new List<string>();
      foreach (string s in UnvisitedSet)
      {
        list.Add(s);
      }
      return list;
    }

    public void ResetUnvisitedSet()
    {
      this.UnvisitedSet = new HashSet<string>();
    }

    public EdgesStruct AsEdgesStruct()
    {
      EdgesStruct edgesStruct = new EdgesStruct();
      edgesStruct.Vertex1 = RootVertex;
      edgesStruct.Vertex2 = "" + Depth;
      edgesStruct.ElapsedMs = ElapsedMs();

      for (int i = 0; i < this.Degrees.Count; i++)
      {
        int level = i + 1;
        JStarDegree jsd = Degrees[i];
        foreach (string v in jsd.OutgoingEdgesMap.Keys)
        {
          IEnumerable<Edge<string>> deSet = jsd.OutgoingEdgesMap[v];
          int seq = 0;
          foreach (Edge<string> de in deSet)
          {
            string[] vertices = ParseDefaultEdge(de);
            if (vertices.Length == 2)
            {
              seq++;
              EdgeStruct es = new EdgeStruct(vertices[0], vertices[1]);
              es.Seq = seq;
              es.Level = level;
              edgesStruct.AddEdge(es);
            }
          }
        }
      }
      return edgesStruct;
    }

    public void Display()
    {
      throw new NotImplementedException();
      // sysout("JStar Network:");
      // sysout("  depth:       " + this.Depth);
      // sysout("  visitedSize: " + VisitedSet.Count);
      // sysout("  elapsedMs:   " + ElapsedMs());
      // sysout("  charlotte:   " + VisitedSet.Contains(ImdbConstants.PERSON_CHARLOTTE_RAMPLING));
      // sysout("  julia:       " + VisitedSet.Contains(ImdbConstants.PERSON_JULIA_ROBERTS));
      // sysout("  lori:        " + VisitedSet.Contains(ImdbConstants.PERSON_LORI_SINGER));
      // sysout("  cjoakim:     " + VisitedSet.Contains("cjoakim"));

      // for (int i = 0; i < this.Degrees.Count; i++)
      // {
      //   JStarDegree jsd = Degrees[i];
      //   sysout("  degree : " + jsd.Degree + " (" + jsd.OutgoingEdgesMap.Count + ")");
      //   Iterator<string> vertexIt = jsd.OutgoingEdgesMap().keySet().iterator();
      //   while (vertexIt.hasNext())
      //   {
      //     string v = vertexIt.next();
      //     sysout("    " + v);
      //     Set<DefaultEdge> deSet = jsd.OutgoingEdgesMap().get(v);
      //     Iterator<DefaultEdge> edgeIt = deSet.iterator();
      //     int seq = 0;
      //     while (edgeIt.hasNext())
      //     {
      //       seq++;
      //       DefaultEdge de = edgeIt.next();
      //       //sysout("      " + de);
      //     }
      //   }
      // }
    }

    public void Finish()
    {
      //FinishMs = System.currentTimeMillis();
    }

    public long ElapsedMs()
    {
      return FinishMs - StartMs;
    }
  }
}