namespace altgraph_shared_app.Options
{
  public class ImdbPathsOptions
  {
    public const string ImdbPaths = "ImdbPaths";
    public string ImdbRawNameBasicsFile { get; set; } = string.Empty;
    public string ImdbRawTitleBasicsFile { get; set; } = string.Empty;
    public string ImdbRawTitlePrincipalsFile { get; set; } = string.Empty;
    public string ImdbMoviesDocumentsFile { get; set; } = string.Empty;
    public string ImdbMoviesSeedFile { get; set; } = string.Empty;
    public string ImdbMoviesMapFile { get; set; } = string.Empty;
    public string ImdbPeopleDocumentsFile { get; set; } = string.Empty;
    public string ImdbPeopleMapFile { get; set; } = string.Empty;
    public string ImdbMoviesOfInterestFile { get; set; } = string.Empty;
    public string ImdbPeopleOfInterestFile { get; set; } = string.Empty;
    public string ImdbSmallTriplesDocumentsFile { get; set; } = string.Empty;
  }
}
