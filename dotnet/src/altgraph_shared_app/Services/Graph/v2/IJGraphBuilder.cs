using QuikGraph;

namespace altgraph_shared_app.Services.Graph.v2
{
  public interface IJGraphBuilder
  {
    bool Directed { get; set; }
    Task<IMutableGraph<string, Edge<string>>?> BuildImdbGraphAsync();

    event EventHandler<JGraphBuilderStartedLoadingProgressEventArgs>? JGraphBuilderStartedLoadingProgress;
    event EventHandler<JGraphBuilderLoadingProgressEventArgs>? JGraphBuilderLoadingProgress;
    event EventHandler<JGraphBuilderFinishedLoadingProgressEventArgs>? JGraphBuilderFinishedLoadingProgress;

  }
}