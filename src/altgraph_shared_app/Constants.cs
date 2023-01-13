namespace altgraph_shared_app
{
  public class Constants
  {
    public const double KB = 1024;
    public const double MB = 1024 * 1024;
    public const double GB = 1024 * 1024 * 1024;
    //public const double TB = 1024 * 1024 * 1024 * 1024;
    public const string IMDB_GRAPH_SOURCE_COSMOS = "cosmos";
    public const string IMDB_GRAPH_SOURCE_DISK = "disk";
    public const string IMDB_SEED_CONTAINER_NAME = "imdb_seed";
    public const string IMDB_GRAPH_CONTAINER_NAME = "imdb_graph";
    public const string GRAPH_DOMAIN_IMDB = "imdb";
    public const string NPM_CONTAINER_NAME = "altgraph";
    //public const string NPM_CONTAINER_NAME = "npm_graph";
    public const string PARTITION_KEY = "/pk";

    public const string DEFAULT_CONTAINER = "altgraph";
    public const string DEFAULT_TENANT = "123";
    public const string LOB_NPM_LIBRARIES = "npm";

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
    public const string IMDB_RAW_NAME_BASICS_FILE = "data/imdb_raw/name.basics.tsv";
    public const string IMDB_RAW_TITLE_BASICS_FILE = "data/imdb_raw/title.basics.tsv";
    public const string IMDB_RAW_TITLE_PRINCIPALS_FILE = "data/imdb_raw/title.principals.tsv";
    public const string IMDB_MOVIES_DOCUMENTS_FILE = "data/imdb_refined/movies.json";
    public const string IMDB_MOVIES_SEED_FILE = "data/imdb_refined/movies_seed.json";
    public const string IMDB_MOVIES_MAP_FILE = "data/imdb_refined/movies_map.json";
    public const string IMDB_PEOPLE_DOCUMENTS_FILE = "data/imdb_refined/people.json";
    public const string IMDB_PEOPLE_MAP_FILE = "data/imdb_refined/people_map.json";
    public const string IMDB_MOVIES_OF_INTEREST_FILE = "data/imdb_refined/movies_of_interest.txt";
    public const string IMDB_PEOPLE_OF_INTEREST_FILE = "data/imdb_refined/people_of_interest.txt";
    public const string IMDB_SMALL_TRIPLES_DOCUMENTS_FILE = "data/imdb_refined/sm_triples.json";
    public const string MEMORY_STATS_BASE = "out/memory_";

    // Edge Labels 
    public const int GROUP_MAX = 100;
    public const string PREDICATE_IN_MOVIE = "inMovie";
    public const string PREDICATE_HAS_PERSON = "hasPerson";
  }
}