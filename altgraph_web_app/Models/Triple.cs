using System;
using System.Collections.Generic;
using Microsoft.Azure.CosmosRepository;

namespace altgraph_web_app.Models
{
  public class Triple : Item
  {
    public string Pk { get; set; } = string.Empty;
    public string ETag { get; set; } = string.Empty;
    public string Tenant { get; set; } = string.Empty;
    public string Lob { get; set; } = string.Empty;
    public string Doctype { get; set; } = string.Empty;
    public string SubjectType { get; set; } = string.Empty;
    public string SubjectLabel { get; set; } = string.Empty;
    public string SubjectId { get; set; } = string.Empty;
    public string SubjectPk { get; set; } = string.Empty;
    public string SubjectKey { get; set; } = string.Empty;
    public List<string> SubjectTags { get; set; } = new List<string>();
    public string Predicate { get; set; } = string.Empty;

    public string ObjectType { get; set; } = string.Empty;
    public string ObjectLabel { get; set; } = string.Empty;
    public string ObjectId { get; set; } = string.Empty;
    public string ObjectPk { get; set; } = string.Empty;
    public string ObjectKey { get; set; } = string.Empty;
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
  }
}