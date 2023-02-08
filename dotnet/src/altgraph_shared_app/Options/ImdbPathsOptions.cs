namespace altgraph_shared_app.Options
{
  public class ImdbPathsOptions
  {
    public const string ImdbPaths = "ImdbPaths";
    public string RawNameBasicsFile { get; set; } = string.Empty;
    public string RawTitleBasicsFile { get; set; } = string.Empty;
    public string RawTitlePrincipalsFile { get; set; } = string.Empty;
    public string MoviesDocumentsFile { get; set; } = string.Empty;
    public string MoviesSeedFile { get; set; } = string.Empty;
    public string MoviesMapFile { get; set; } = string.Empty;
    public string PeopleDocumentsFile { get; set; } = string.Empty;
    public string PeopleMapFile { get; set; } = string.Empty;
    public string MoviesOfInterestFile { get; set; } = string.Empty;
    public string PeopleOfInterestFile { get; set; } = string.Empty;
    public string SmallTriplesDocumentsFile { get; set; } = string.Empty;
  }
}
