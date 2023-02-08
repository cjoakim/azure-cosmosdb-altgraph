using System.Text.Json.Serialization;
using Microsoft.Azure.CosmosRepository.Attributes;

namespace altgraph_shared_app.Models.Imdb
{
  public class Principal : AbstractDocument
  {
    [JsonPropertyName("tconst")]
    public string TConst { get; set; } = string.Empty;
    [JsonPropertyName("nconst")]
    public string NConst { get; set; } = string.Empty;
    [JsonPropertyName("category")]
    public string Category { get; set; } = string.Empty;

    public Principal()
    {
    }

    public Principal(string[] lineTokens)
    {
      if (lineTokens != null)
      {
        if (lineTokens.Length > 5)
        {
          Doctype = "principal";
          TConst = lineTokens[0].Trim().ToLower();
          NConst = lineTokens[2].Trim().ToLower();
          Category = lineTokens[3].Trim().ToLower();
        }
      }
    }

    public bool IsValid()
    {
      if (TConst == null)
      {
        return false;
      }
      if (NConst == null)
      {
        return false;
      }
      if (TConst.Length < 8)
      { // tt7600874
        return false;
      }
      if (NConst.Length < 8)
      { // tt7600874
        return false;
      }
      return true;
    }

    public void ScrubValues()
    {

    }

    public bool HasNecessaryRole()
    {
      if (Category == null)
      {
        return false;
      }
      if (Category.Contains("actress"))
      {
        return true;
      }
      if (Category.Contains("actor"))
      {
        return true;
      }
      if (Category.Contains("director"))
      {
        return true;
      }
      return false;
    }
  }
}