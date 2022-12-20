using System.Text.Json.Serialization;

namespace altgraph_shared_app.Models
{
  public class Library : Entity
  {
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
    [JsonPropertyName("desc")]
    public string Desc { get; set; } = string.Empty;
    [JsonPropertyName("keywords")]
    public List<string> Keywords { get; set; } = new List<string>();
    [JsonPropertyName("dependencies")]
    public Dictionary<string, string> Dependencies { get; set; } = new Dictionary<string, string>();
    [JsonPropertyName("devDependencies")]
    public Dictionary<string, string> DevDependencies { get; set; } = new Dictionary<string, string>();
    [JsonPropertyName("author")]
    public string Author { get; set; } = string.Empty;
    [JsonPropertyName("maintainers")]
    public List<string> Maintainers { get; set; } = new List<string>();
    [JsonPropertyName("version")]
    public string Version { get; set; } = string.Empty;
    [JsonPropertyName("versions")]
    public List<string> Versions { get; set; } = new List<string>();
    [JsonPropertyName("homepage")]
    public string Homepage { get; set; } = string.Empty;
    [JsonPropertyName("library_age_days")]
    public int LibraryAgeDays { get; set; }
    [JsonPropertyName("version_age_days")]
    public int VersionAgeDays { get; set; }
  }
}