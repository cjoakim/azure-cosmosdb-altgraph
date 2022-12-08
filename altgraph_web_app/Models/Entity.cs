using System;
using System.Text;
using Microsoft.Azure.CosmosRepository;

namespace altgraph_web_app.Models
{
  public class Entity : Item
  {
    public string Doctype { get; set; } = "";
    public string Label { get; set; } = "";
    public string Pk { get; set; } = "";
    public string ETag { get; set; } = "";
    public string Tenant { get; set; } = "";
    public string Lob { get; set; } = "";
    public string CacheKey { get; set; } = "";
    public string GraphKey { get; set; } = "";

    public void PopulateCacheKey()
    {
      CacheKey = "" + Doctype + "|" + Label;
    }

    public string CalculateGraphKey()
    {
      StringBuilder sb = new StringBuilder();
      sb.Append(this.Doctype);
      sb.Append("^");
      sb.Append(this.Label);
      sb.Append("^");
      sb.Append(this.Id);
      sb.Append("^");
      sb.Append(this.Pk);
      GraphKey = sb.ToString();
      return GraphKey;
    }

    protected override string GetPartitionKeyValue()
    {
      return Pk;
    }
  }
}