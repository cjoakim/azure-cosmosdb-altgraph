using System;
using System.Text;
using System.Text.Json.Serialization;
using Microsoft.Azure.CosmosRepository;

namespace altgraph_shared_app.Models
{
  public class Entity : Item
  {
    [JsonPropertyName("doctype")]
    public string Doctype { get; set; } = string.Empty;
    [JsonPropertyName("label")]
    public string Label { get; set; } = string.Empty;
    [JsonPropertyName("pk")]
    public string Pk { get; set; } = string.Empty;
    [JsonPropertyName("etag")]
    public string ETag { get; set; } = string.Empty;
    [JsonPropertyName("tenant")]
    public string Tenant { get; set; } = string.Empty;
    [JsonPropertyName("lob")]
    public string Lob { get; set; } = string.Empty;
    [JsonPropertyName("cacheKey")]
    public string CacheKey { get; set; } = string.Empty;
    [JsonPropertyName("graphKey")]
    public string GraphKey { get; set; } = string.Empty;

    public void PopulateCacheKey()
    {
      CacheKey = string.Empty + Doctype + "|" + Label;
    }

    public string CalculateGraphKey()
    {
      StringBuilder sb = new StringBuilder();
      sb.Append(Doctype);
      sb.Append("^");
      sb.Append(Label);
      sb.Append("^");
      sb.Append(Id);
      sb.Append("^");
      sb.Append(Pk);
      GraphKey = sb.ToString();
      return GraphKey;
    }

    protected override string GetPartitionKeyValue()
    {
      return Pk;
    }
  }
}