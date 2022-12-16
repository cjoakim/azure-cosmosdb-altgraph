namespace altgraph_shared_app.Options
{
  public class RedisOptions
  {
    public const string Redis = "Redis";
    public string ConnectionString { get; set; } = string.Empty;
    public bool SSL { get; set; } = false;
  }
}
