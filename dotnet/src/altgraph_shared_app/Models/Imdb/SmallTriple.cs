using System.Text.Json.Serialization;
using Microsoft.Azure.CosmosRepository.Attributes;

namespace altgraph_shared_app.Models.Imdb
{
  public class SmallTriple : AbstractDocument
  {
    [JsonPropertyName("subjectType")]
    public string SubjectType { get; set; } = string.Empty;
    [JsonPropertyName("subjectIdPk")]
    public string SubjectIdPk { get; set; } = string.Empty;
    [JsonPropertyName("subjectTags")]
    public List<string> SubjectTags { get; set; } = new List<string>();
    [JsonPropertyName("predicate")]
    public string Predicate { get; set; } = string.Empty;
    [JsonPropertyName("objectType")]
    public string ObjectType { get; set; } = string.Empty;
    [JsonPropertyName("objectIdPk")]
    public string ObjectIdPk { get; set; } = string.Empty;
    [JsonPropertyName("objectTags")]
    public List<string> ObjectTags = new List<string>();

    public SmallTriple()
    {
    }

    public void ScrubValues()
    {

    }
  }
}