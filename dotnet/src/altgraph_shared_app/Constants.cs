namespace altgraph_shared_app
{
  public class Constants
  {
    public const double KB = 1024;
    public const double MB = 1024 * 1024;
    public const double GB = 1024 * 1024 * 1024;
    //public const double TB = 1024 * 1024 * 1024 * 1024;
    // public const string GRAPH_DOMAIN_IMDB = "imdb";
    public const string LOB_NPM_LIBRARIES = "npm";
    public const string IMDB_GRAPH_SOURCE_COSMOS = "cosmos";
    public const string IMDB_GRAPH_SOURCE_DISK = "file";

    // Document Types
    public const string DOCTYPE_LIBRARY = "library";
    public const string DOCTYPE_AUTHOR = "author";
    public const string DOCTYPE_MAINTAINER = "maintainer";
    public const string DOCTYPE_TRIPLE = "triple";
    public const string DOCTYPE_SMALL_TRIPLE = "sm_triple";
    public const string DOCTYPE_MOVIE = "movie";
    public const string DOCTYPE_MOVIE_SEED = "movie_seed";
    public const string DOCTYPE_PERSON = "person";
    public const string DOCTYPE_PRINCIPAL = "principal";
    public const string MEMORY_STATS_BASE = "out/memory_";

    // Edge Labels
    public const int GROUP_MAX = 100;
    public const string PREDICATE_IN_MOVIE = "inMovie";
    public const string PREDICATE_HAS_PERSON = "hasPerson";
  }
}