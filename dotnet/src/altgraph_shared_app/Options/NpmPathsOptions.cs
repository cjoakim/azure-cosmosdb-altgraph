namespace altgraph_shared_app.Options
{
  public class NpmPathsOptions
  {
    public const string NpmPaths = "NpmPaths";
    public string NodesCsvFile { get; set; } = string.Empty;
    public string EdgesCsvFile { get; set; } = string.Empty;
    public string LibraryCacheFileTemplate { get; set; } = string.Empty;
    public string TripleQueryStructCacheFile { get; set; } = string.Empty;
    public string RawLibrariesFile { get; set; } = string.Empty;
    public string LibrariesFile { get; set; } = string.Empty;
    public string AuthorsFile { get; set; } = string.Empty;
    public string MaintainersFile { get; set; } = string.Empty;
    public string TriplesFile { get; set; } = string.Empty;
    public string TripleQueryStructFile { get; set; } = string.Empty;
    public string LibraryGraphJsonFile { get; set; } = string.Empty;
    public string AuthorGraphJsonFile { get; set; } = string.Empty;
    public string D3CsvBuilderFile { get; set; } = string.Empty;
  }
}
