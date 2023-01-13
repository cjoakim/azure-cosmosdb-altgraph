using altgraph_shared_app.Services.Graph.v2.Structs;
using QuikGraph;

namespace altgraph_shared_app.Services.Graph.v2
{
  public interface IJGraph
  {
    string Domain { get; set; }
    string Source { get; set; }
    DateTime RefreshDate { get; set; }
    long RefreshMs { get; set; }
    IMutableGraph<string, Edge<string>>? Graph { get; set; }

    double CentralityOfVertex(string v);
    Dictionary<string, double> CentralityRankAll();
    int DegreeOf(string v);
    HashSet<Edge<string>>? EdgesOf(string v);
    IEnumerable<Edge<string>>? GetShortestPath(string v1, string v2);
    EdgesStruct? GetShortestPathAsEdgesStruct(string v1, string v2);
    int[] GetVertexAndEdgeCounts();
    HashSet<Edge<string>>? IncomingEdgesOf(string v);
    int InDegreeOf(string v);
    bool IsVertexPresent(string v);
    IDictionary<string, double>? PageRankForAll();
    double PageRankForVertex(string v);
    Task RefreshAsync();
    List<JRank> SortedPageRanks(int maxCount);
    JStarNetwork? StarNetworkFor(string rootVertex, int degrees);

    event EventHandler<JGraphStartedLoadingProgressEventArgs>? JGraphStartedLoadingProgress;
    event EventHandler<JGraphLoadingProgressEventArgs>? JGraphLoadingProgress;
    event EventHandler<JGraphFinishedLoadingProgressEventArgs>? JGraphFinishedLoadingProgress;
  }
}