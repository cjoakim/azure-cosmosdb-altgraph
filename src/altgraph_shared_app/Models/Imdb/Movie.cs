using System.Text.Json.Serialization;
using Microsoft.Azure.CosmosRepository.Attributes;

namespace altgraph_shared_app.Models.Imdb
{
  public class Movie : AbstractDocument
  {
    [JsonPropertyName("tconst")]
    public string TConst { get; set; } = string.Empty;
    [JsonPropertyName("titleType")]
    public string TitleType { get; set; } = string.Empty;
    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;
    [JsonPropertyName("titleWords")]
    public List<string> TitleWords { get; set; } = new List<string>();
    //[JsonPropertyName("isAdult")]
    [JsonIgnore]
    public string IsAdult { get; set; } = string.Empty;
    [JsonPropertyName("year")]
    public int Year { get; set; }
    [JsonPropertyName("minutes")]
    public int Minutes { get; set; }
    [JsonPropertyName("people")]
    public HashSet<string> People { get; set; } = new HashSet<string>();

    public Movie()
    {
    }

    public Movie(string[] lineTokens)
    {
      if (lineTokens != null)
      {
        if (lineTokens.Length > 8)
        {
          Doctype = "movie";
          TConst = lineTokens[0].Trim();
          TitleType = lineTokens[1].Trim();
          Title = lineTokens[2].Trim();
          IsAdult = lineTokens[4].Trim();
          People = new HashSet<string>();
          try
          {
            Year = int.Parse(lineTokens[5].Trim());
            Minutes = int.Parse(lineTokens[7].Trim());
          }
          catch (ArgumentOutOfRangeException)
          {
            // ignore
          }
        }
      }
    }

    public void scrubValues()
    {

    }

    public void AddPerson(string nconst)
    {
      if (TConst != null)
      {
        People.Add(nconst.Trim().ToLower());
      }
    }

    public string? TitleWordsJoined()
    {
      if (TitleWords != null)
      {
        return string.Join(" ", TitleWords);
      }
      return null;
    }

    public bool Include(int minYear, int minMinutes)
    {
      if (Year < minYear)
      {
        return false;
      }
      if (Minutes < minMinutes)
      {
        return false;
      }
      if (IsAdult == null)
      {
        return false;
      }
      if (IsAdult.Equals("1"))
      {
        return false;
      }
      if (!TitleType.Equals("movie"))
      {
        return false;
      }
      return true;
    }
  }
}