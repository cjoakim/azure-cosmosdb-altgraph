using System.Text.Json.Serialization;
using Microsoft.Azure.CosmosRepository.Attributes;

namespace altgraph_shared_app.Models.Imdb
{
  public class SeedDocument : AbstractDocument
  {
    [JsonPropertyName("targetId")]
    public string TargetId { get; set; } = string.Empty;
    [JsonPropertyName("targetPk")]
    public string TargetPk { get; set; } = string.Empty;
    [JsonPropertyName("targetDoctype")]
    public string TargetDoctype { get; set; } = string.Empty;
    [JsonPropertyName("adjacentVertices")]
    public List<string> AdjacentVertices { get; set; } = new List<string>();

    public SeedDocument()
    {
    }

    public SeedDocument(string pkDoctype, string tgtId, string tgtPk)
    {
      Id = Guid.NewGuid().ToString();
      Pk = pkDoctype;          // note that doctype and pk (partition key) are the same value
      Doctype = pkDoctype;
      TargetId = tgtId;
      TargetPk = tgtPk;
    }

    public void ScrubValues()
    {

    }

    public void AddAdjacentVertex(string idPk)
    {
      if (idPk != null)
      {
        AdjacentVertices.Add(idPk);
      }
    }
  }
}