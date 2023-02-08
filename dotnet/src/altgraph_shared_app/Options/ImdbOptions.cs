namespace altgraph_shared_app.Options
{
  public class ImdbOptions
  {
    public const string Imdb = "Imdb";
    public string GraphDomain { get; set; } = string.Empty;
    public string GraphSource { get; set; } = string.Empty;
    public bool GraphDirected { get; set; } = false;
    public string SeedContainerName { get; set; } = string.Empty;
    public string GraphContainerName { get; set; } = string.Empty;
    public string PartitionKey { get; set; } = string.Empty;
  }
}