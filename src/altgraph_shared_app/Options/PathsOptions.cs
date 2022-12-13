namespace altgraph_shared_app.Options
{
  public class PathsOptions
  {
    public const string Paths = "Paths";
    public string NodesCsvFile { get; set; } = string.Empty;
    public string EdgesCsvFile { get; set; } = string.Empty;
    public string LibraryCacheFileTemplate { get; set; } = string.Empty;
    public string TripleQueryStructCacheFile { get; set; } = string.Empty;
  }
}
