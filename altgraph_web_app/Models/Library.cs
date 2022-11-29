namespace altgraph_web_app.Models
{
  public class Library : Entity
  {
    public string? Name { get; set; }
    public string? Desc { get; set; }
    public List<string>? Keywords { get; set; }
    public Dictionary<string, string>? Dependencies { get; set; }
    public Dictionary<string, string>? DevDependencies { get; set; }
    public string? Author { get; set; }
    public List<string>? Maintainers { get; set; }
    public string? version;
    public List<string>? Versions { get; set; }
    public string? Homepage { get; set; }
    public int? LibraryAgeDays { get; set; }
    public int? VersionAgeDays { get; set; }
  }
}