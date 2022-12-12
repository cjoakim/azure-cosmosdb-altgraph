using System;
using System.Text;
using Microsoft.Azure.CosmosRepository;

namespace altgraph_web_app.Models
{
  public class Entity : Item
  {
    public string Doctype { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public string Pk { get; set; } = string.Empty;
    public string ETag { get; set; } = string.Empty;
    public string Tenant { get; set; } = string.Empty;
    public string Lob { get; set; } = string.Empty;
    public string CacheKey { get; set; } = string.Empty;
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