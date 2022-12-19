namespace altgraph_shared_app.Options
{
  public class KeyVaultOptions
  {
    public const string Cache = "KeyVault";
    public bool UseKeyVault { get; set; } = false;
    public string KeyVaultUri { get; set; } = string.Empty;
  }
}
