using System.Text.Json.Serialization;
using Microsoft.Azure.CosmosRepository.Attributes;

namespace altgraph_shared_app.Models.Imdb
{
  [Container(Constants.IMDB_GRAPH_CONTAINER_NAME)]
  [PartitionKeyPath(Constants.PARTITION_KEY)]
  public class Person : AbstractDocument
  {
    [JsonPropertyName("nconst")]
    public string NConst { get; set; } = string.Empty;
    [JsonPropertyName("primaryName")]
    public string PrimaryName { get; set; } = string.Empty;
    [JsonPropertyName("primaryNameWords")]
    public List<string> PrimaryNameWords { get; set; } = new List<string>();
    [JsonPropertyName("birthYear")]
    public string BirthYear { get; set; } = string.Empty;
    [JsonPropertyName("deathYear")]
    public string DeathYear { get; set; } = string.Empty;
    [JsonPropertyName("primaryProfession")]
    public string PrimaryProfession { get; set; } = string.Empty;
    [JsonPropertyName("primaryProfessionWords")]
    public List<string> PrimaryProfessionWords { get; set; } = new List<string>();
    [JsonPropertyName("knownForTitles")]
    public HashSet<string> Titles { get; set; } = new HashSet<string>();
  [JsonPropertyName("titleCount")]
    public string TitleCount { get; set; } = "0";

    public Person()
    {

    }

    public Person(string[] lineTokens)
    {
      if (lineTokens != null)
      {
        if (lineTokens.Length > 5)
        {
          Doctype = "person";
          NConst = lineTokens[0].Trim();
          PrimaryName = lineTokens[1].Trim();
          BirthYear = lineTokens[2].Trim();
          DeathYear = lineTokens[3].Trim();
          PrimaryProfession = lineTokens[4].Trim();
          Titles = new HashSet<string>();

          //                token: 0 -> nconst
          //                token: 1 -> primaryName
          //                token: 2 -> birthYear
          //                token: 3 -> deathYear
          //                token: 4 -> primaryProfession
          //                token: 5 -> knownForTitles
          //                lineNumber: 2>>> nm0000001      Fred Astaire    1899    1987    soundtrack,actor,miscellaneous  tt0031983,tt0050419,tt0053137,tt0072308 <<<< tokens:6
          //                token: 0 -> nm0000001
          //                token: 1 -> Fred Astaire
          //                token: 2 -> 1899
          //                token: 3 -> 1987
          //                token: 4 -> soundtrack,actor,miscellaneous
          //                token: 5 -> tt0031983,tt0050419,tt0053137,tt0072308
        }
      }
    }

    public bool IsValid()
    {
      if (NConst == null)
      {
        return false;
      }
      if (PrimaryName == null)
      {
        return false;
      }
      if (NConst.Length < 8)
      {
        return false;
      }
      if (PrimaryName.Equals("\\\\N"))
      {
        return false;
      }
      if (PrimaryName.Length < 4)
      {
        return false;
      }
      return true;
    }

    public void ScrubValues()
    {
      if (IsValid())
      {
        if (BirthYear.Contains("\\N"))
        {
          BirthYear = "";
        }
        if (DeathYear.Contains("\\N"))
        {
          DeathYear = "";
        }
        if (PrimaryProfession.Contains("\\N"))
        {  // the raw data Contains lowercase valid values
          PrimaryName = "";
        }
      }
    }

    public string? PrimaryNameWordsJoined()
    {
      if (PrimaryNameWords != null)
      {
        return string.Join(" ", PrimaryNameWords);
      }
      return null;
    }

    public bool IsActorActressDirector()
    {
      if (PrimaryProfession != null)
      {
        if (PrimaryProfession.Contains("actor"))
        {
          return true;
        }
        if (PrimaryProfession.Contains("actress"))
        {
          return true;
        }
        if (PrimaryProfession.Contains("director"))
        {
          return true;
        }
      }
      return false;
    }

    public string GetShortInfo()
    {
      return "" + NConst + "," + PrimaryName;
    }

    public void AddTitle(string tconst)
    {
      if (tconst != null)
      {
        Titles.Add(tconst.Trim().ToLower());
        TitleCount = "" + Titles.Count;
      }
    }
  }
}