namespace altgraph_web_app.Models
{
  public class Triple
  {
    public string? Id { get; set; }
    public string? Pk { get; set; }
    public string? ETag { get; set; }
    public string? Tenant { get; set; }
    public string? Lob { get; set; }
    public string? Doctype { get; set; }
    public string? SubjectType { get; set; }
    public string? SubjectLabel { get; set; }
    public string? SubjectId { get; set; }
    public string? SubjectPk { get; set; }
    public string? SubjectKey { get; set; }
    public List<string>? SubjectTags { get; set; }
    public string? Predicate { get; set; }

    public string? ObjectType { get; set; }
    public string? ObjectLabel { get; set; }
    public string? ObjectId { get; set; }
    public string? ObjectPk { get; set; }
    public string? ObjectKey { get; set; }
    public List<string>? ObjectTags { get; set; }

    public void setKeyFields()
    {
      SubjectKey = SubjectType + "^" + SubjectLabel + "^" + SubjectId + "^" + SubjectPk;
      ObjectKey = ObjectType + "^" + ObjectLabel + "^" + ObjectId + "^" + ObjectPk;
    }

    public void AddSubjectTag(String tag)
    {

      if (tag != null)
      {
        SubjectTags.Add(tag.Trim());
      }
    }

    public void AddObjectTag(String tag)
    {

      if (tag != null)
      {
        ObjectTags.Add(tag.Trim());
      }
    }

    public String AsJson(bool pretty)
    {
      throw new Exception();
    }
  }
}