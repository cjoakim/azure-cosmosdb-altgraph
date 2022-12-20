using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Microsoft.Azure.CosmosRepository;

namespace altgraph_shared_app.Models
{
  public class Triple : Item
  {
    [JsonPropertyName("pk")]
    public string Pk { get; set; } = string.Empty;
    [JsonPropertyName("etag")]
    public string ETag { get; set; } = string.Empty;
    [JsonPropertyName("tenant")]
    public string Tenant { get; set; } = string.Empty;
    [JsonPropertyName("lob")]
    public string Lob { get; set; } = string.Empty;
    [JsonPropertyName("doctype")]
    public string Doctype { get; set; } = string.Empty;
    [JsonPropertyName("subjectType")]
    public string SubjectType { get; set; } = string.Empty;
    [JsonPropertyName("subjectLabel")]
    public string SubjectLabel { get; set; } = string.Empty;
    [JsonPropertyName("subjectId")]
    public string SubjectId { get; set; } = string.Empty;
    [JsonPropertyName("subjectPk")]
    public string SubjectPk { get; set; } = string.Empty;
    [JsonPropertyName("subjectKey")]
    public string SubjectKey { get; set; } = string.Empty;
    [JsonPropertyName("subjectTags")]
    public List<string> SubjectTags { get; set; } = new List<string>();
    [JsonPropertyName("predicate")]
    public string Predicate { get; set; } = string.Empty;
    [JsonPropertyName("objectType")]
    public string ObjectType { get; set; } = string.Empty;
    [JsonPropertyName("objectLabel")]
    public string ObjectLabel { get; set; } = string.Empty;
    [JsonPropertyName("objectId")]
    public string ObjectId { get; set; } = string.Empty;
    [JsonPropertyName("objectPk")]
    public string ObjectPk { get; set; } = string.Empty;
    [JsonPropertyName("objectKey")]
    public string ObjectKey { get; set; } = string.Empty;
    [JsonPropertyName("objectTags")]
    public List<string> ObjectTags { get; set; } = new List<string>();

    public void SetKeyFields()
    {
      SubjectKey = $"{SubjectType}^{SubjectLabel}^{SubjectId}^{SubjectPk}";
      ObjectKey = $"{ObjectType}^{ObjectLabel}^{ObjectId}^{ObjectPk}";
    }

    public void AddSubjectTag(string tag)
    {
      if (tag != null)
      {
        SubjectTags.Add(tag.Trim());
      }
    }

    public void AddObjectTag(string tag)
    {
      if (tag != null)
      {
        ObjectTags.Add(tag.Trim());
      }
    }
    protected override string GetPartitionKeyValue()
    {
      return Pk;
    }
  }
}