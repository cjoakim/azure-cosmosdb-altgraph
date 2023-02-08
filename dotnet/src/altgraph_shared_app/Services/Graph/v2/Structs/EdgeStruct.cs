using System.Text.Json.Serialization;

namespace altgraph_shared_app.Services.Graph.v2.Structs
{
  public class EdgeStruct
  {
    [JsonPropertyName("seq")]
    public int Seq { get; set; }
    [JsonPropertyName("level")]
    public int Level { get; set; }
    [JsonPropertyName("v1Value")]
    public string V1Value { get; set; } = string.Empty;
    [JsonPropertyName("v1Name")]
    public string V1Name { get; set; } = string.Empty;
    [JsonPropertyName("v2Value")]
    public string V2Value { get; set; } = string.Empty;
    [JsonPropertyName("v2Name")]
    public string V2Name { get; set; } = string.Empty;

    public EdgeStruct()
    {
    }
    public EdgeStruct(string v1, string v2)
    {
      V1Value = v1;
      V2Value = v2;
    }

    public bool IsValid()
    {
      if (V1Value == null)
      {
        return false;
      }
      if (V2Value == null)
      {
        return false;
      }
      if (V1Value.Length < 1)
      {
        return false;
      }
      if (V2Value.Length < 1)
      {
        return false;
      }
      return true;
    }
  }
}