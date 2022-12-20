using System.Text.Json.Serialization;

namespace altgraph_shared_app.Models
{
  public class Author : Entity
  {
    [JsonPropertyName("authorName")]
    public string AuthorName { get; set; } = string.Empty;
  }
}