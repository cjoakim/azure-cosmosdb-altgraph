using System.Text.Json.Serialization;

namespace altgraph_shared_app.Services.Graph.v2.Structs
{
  public class DegreeStruct
  {
    public long ElapsedMs { get; set; } = -1;
    public string Doctype { get; private set; } = "DegreeStruct";
    [JsonPropertyName("vertex")]
    public string Vertex { get; set; } = string.Empty;
    [JsonPropertyName("degree")]
    public int Degree { get; set; }
    [JsonPropertyName("inDegree")]
    public int InDegree { get; set; }
  }
}