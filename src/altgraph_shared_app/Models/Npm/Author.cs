using System.Text.Json.Serialization;
using Microsoft.Azure.CosmosRepository.Attributes;

namespace altgraph_shared_app.Models.Npm
{
  public class Author : NpmDocument
  {
    [JsonPropertyName("authorName")]
    public string AuthorName { get; set; } = string.Empty;
  }
}