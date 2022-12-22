using System.Text.Json.Serialization;
using Microsoft.Azure.CosmosRepository.Attributes;

namespace altgraph_shared_app.Models.Npm
{
  [Container(Constants.NPM_CONTAINER_NAME)]
  [PartitionKeyPath(Constants.PARTITION_KEY)]
  public class Author : NpmDocument
  {
    [JsonPropertyName("authorName")]
    public string AuthorName { get; set; } = string.Empty;
  }
}