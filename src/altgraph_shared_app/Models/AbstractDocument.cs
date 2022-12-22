using System;
using System.Text;
using System.Text.Json.Serialization;
using Microsoft.Azure.CosmosRepository;

namespace altgraph_shared_app.Models
{
  public abstract class AbstractDocument : Item
  {
    [JsonPropertyName("doctype")]
    public string Doctype { get; set; } = string.Empty;
    [JsonPropertyName("pk")]
    public string Pk { get; set; } = string.Empty;
    [JsonPropertyName("etag")]
    public string ETag { get; set; } = string.Empty;
    [JsonPropertyName("g")]
    public string G { get; set; } = string.Empty; // group, or load-group; a random value from 0 to 99. 

    private Random rnd = new Random();

    public void SetCosmosDbCoordinateAttributes(string idPk, string doctype)
    {
      if ((idPk == null) || (idPk.Trim() == string.Empty))
      {
        Id = System.Guid.NewGuid().ToString();
        Pk = "orphan";
        G = "" + rnd.NextInt64(Constants.GROUP_MAX); // range 0 to 99
        return;
      }
      Id = idPk;
      Pk = idPk;
      Doctype = "" + doctype;
      G = "" + rnd.NextInt64(Constants.GROUP_MAX); // range 0 to 99
    }

    public void SetCosmosDbSmallTripleCoordinateAttributes()
    {
      Id = System.Guid.NewGuid().ToString();
      Pk = Constants.DOCTYPE_SMALL_TRIPLE;
      Doctype = Constants.DOCTYPE_SMALL_TRIPLE;
      G = "" + rnd.NextInt64(Constants.GROUP_MAX); // range 0 to 99
    }

    protected override string GetPartitionKeyValue()
    {
      return Pk;
    }
  }
}