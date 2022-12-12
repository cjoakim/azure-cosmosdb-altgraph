namespace altgraph_web_app.Models
{
  public class Library : Entity
  {
    public string Name { get; set; } = string.Empty;
    public string Desc { get; set; } = string.Empty;
    public List<string> Keywords { get; set; } = new List<string>();
    public Dictionary<string, string> Dependencies { get; set; } = new Dictionary<string, string>();
    public Dictionary<string, string> DevDependencies { get; set; } = new Dictionary<string, string>();
    public string Author { get; set; } = string.Empty;
    public List<string> Maintainers { get; set; } = new List<string>();
    public string Version { get; set; } = string.Empty;
    public List<string> Versions { get; set; } = new List<string>();
    public string Homepage { get; set; } = string.Empty;
    public int LibraryAgeDays { get; set; }
    public int VersionAgeDays { get; set; }
  }
}