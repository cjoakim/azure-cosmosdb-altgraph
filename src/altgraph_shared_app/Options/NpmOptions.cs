namespace altgraph_shared_app.Options
{
  public class NpmOptions
  {
    public const string Npm = "Npm";
    public string ContainerName { get; set; } = string.Empty;
    public string PartitionKey { get; set; } = string.Empty;
    public string DefaultTenant { get; set; } = string.Empty;
  }
}