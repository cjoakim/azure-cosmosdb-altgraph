namespace altgraph_shared_app
{
  public class Constants
  {
    public const string NPM_CONTAINER_NAME = "npm_graph";
    public const string PARTITION_KEY = "/pk";
    public const int GROUP_MAX = 100;
    public const string DEFAULT_CONTAINER = "altgraph";
    public const string DEFAULT_TENANT = "123";
    public const string LOB_NPM_LIBRARIES = "npm";

    // Document Types
    public const string DOCTYPE_LIBRARY = "library";
    public const string DOCTYPE_AUTHOR = "author";
    public const string DOCTYPE_MAINTAINER = "maintainer";
    public const string DOCTYPE_TRIPLE = "triple";
    public const string DOCTYPE_SMALL_TRIPLE = "sm_triple";
  }
}