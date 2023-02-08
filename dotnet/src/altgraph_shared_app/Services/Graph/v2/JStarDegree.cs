using QuikGraph;

namespace altgraph_shared_app.Services.Graph.v2
{
  public class JStarDegree
  {
    public int Degree { get; set; }
    public Dictionary<string, IEnumerable<Edge<string>>> OutgoingEdgesMap { get; set; }

    public JStarDegree(int degree)
    {
      Degree = degree;
      OutgoingEdgesMap = new Dictionary<string, IEnumerable<Edge<string>>>();
    }

    public void Add(string vertex, IEnumerable<Edge<string>> outEdges)
    {
      OutgoingEdgesMap.Add(vertex, outEdges);
    }
  }
}