using System.Text.Json;
using altgraph_data_app.common.io;
using altgraph_shared_app;
using altgraph_shared_app.Models.Imdb;
using altgraph_shared_app.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace altgraph_data_app.processor
{
  public class ImdbRawDataWranglerProcess : IConsoleAppProcess
  {
    private readonly ImdbPathsOptions _imdbPathsOptions;
    private readonly ILogger<ImdbRawDataWranglerProcess> _logger;
    private readonly ConsoleAppProcess _consoleAppProcess;
    private long totalMovieCount = 0;
    private long totalPersonCount = 0;
    private long totalPrincipalCount = 0;
    private long titleToPersonCount = 0;
    private long startMs = 0;
    private Dictionary<string, Movie> movies = new Dictionary<string, Movie>();
    private Dictionary<string, Person> people = new Dictionary<string, Person>();
    private HashSet<string> principalSet = new HashSet<string>();

    public int MinYear { get; set; } = 0;
    public int MinMinutes { get; set; } = 0;

    public ImdbRawDataWranglerProcess(ILogger<ImdbRawDataWranglerProcess> logger, IOptions<ImdbPathsOptions> imdbPathsOptions, ConsoleAppProcess consoleAppProcess)
    {
      _logger = logger;
      _imdbPathsOptions = imdbPathsOptions.Value;
      _consoleAppProcess = consoleAppProcess;
    }

    public async Task ProcessAsync()
    {
      startMs = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

      await ReadFilterMoviesAsync();

      await ReadIdentifyPrincipalsInMoviesAsync();

      await ReadNamesOfPrincipalsAsync();

      await AssociateTitlesToPeopleAsync();

      await WriteMoviesAsync();

      await WritePeopleAsync();

      await CreateWriteMovieSeedAsync();

      DisplayEojTotals();
    }

    private async Task ReadFilterMoviesAsync()
    {
      string path = _imdbPathsOptions.RawTitleBasicsFile;
      _logger.LogDebug("ReadFilterMoviesAsync, path: {0}", path);

      try
      {
        using (StreamReader sr = new StreamReader(path))
        {
          string? line = null;
          while ((line = await sr.ReadLineAsync()) != null)
          {
            Movie? movie = JsonSerializer.Deserialize<Movie>(line);
            if (movie != null && movie.Include(MinYear, MinMinutes))
            {
              movie.SetCosmosDbCoordinateAttributes(movie.TConst, Constants.DOCTYPE_MOVIE);
              movies.Add(movie.TConst, movie);
              totalMovieCount++;
            }
          }
        }
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "ReadFilterMoviesAsync, path: {0}", path);
      }
    }

    private async Task ReadIdentifyPrincipalsInMoviesAsync()
    {
      string path = _imdbPathsOptions.RawTitlePrincipalsFile;
      _logger.LogDebug("ReadIdentifyPrincipalsInMoviesAsync, path: {0}", path);

      try
      {
        using (StreamReader sr = new StreamReader(path))
        {
          string? line = null;
          while ((line = await sr.ReadLineAsync()) != null)
          {
            Principal? principal = JsonSerializer.Deserialize<Principal>(line);
            if (principal != null)
            {
              principal.Doctype = Constants.DOCTYPE_PRINCIPAL;
              totalPrincipalCount++;

              if (principal.IsValid() && principal.HasNecessaryRole() && movies.ContainsKey(principal.TConst))
              {
                principalSet.Add(principal.NConst);
              }
            }
          }
        }
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "ReadFilterMoviesAsync, path: {0}", path);
      }
    }

    private async Task ReadNamesOfPrincipalsAsync()
    {
      string path = _imdbPathsOptions.RawNameBasicsFile;
      _logger.LogDebug("ReadIdentifyPrincipalsInMoviesAsync, path: {0}", path);

      try
      {
        using (StreamReader sr = new StreamReader(path))
        {
          string? line = null;
          while ((line = await sr.ReadLineAsync()) != null)
          {
            Person? person = JsonSerializer.Deserialize<Person>(line);
            if (person != null)
            {
              person.Doctype = Constants.DOCTYPE_PRINCIPAL;
              totalPrincipalCount++;

              if (person.IsValid() && principalSet.Contains(person.NConst))
              {
                person.ScrubValues();
                person.SetCosmosDbCoordinateAttributes(person.NConst, Constants.DOCTYPE_PERSON);
                people.Add(person.NConst, person);
              }
            }
          }
        }
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "ReadIdentifyPrincipalsInMoviesAsync, path: {0}", path);
      }
    }

    private async Task WriteMoviesAsync()
    {
      string path = _imdbPathsOptions.MoviesDocumentsFile;
      _logger.LogDebug("WriteMoviesAsync, path: {0}", path);
      try
      {
        using (StreamWriter sw = new StreamWriter(path))
        {
          foreach (var movie in movies)
          {
            await sw.WriteLineAsync(JsonSerializer.Serialize(movie.Value));
          }
        }
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "WriteMoviesAsync, path: {0}", path);
      }
    }

    private async Task WritePeopleAsync()
    {
      string path = _imdbPathsOptions.PeopleDocumentsFile;
      _logger.LogDebug("WritePeopleAsync, path: {0}", path);
      try
      {
        using (StreamWriter sw = new StreamWriter(path))
        {
          foreach (var people in people)
          {
            await sw.WriteLineAsync(JsonSerializer.Serialize(people.Value));
          }
        }
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "WritePeopleAsync, path: {0}", path);
      }
    }

    private async Task AssociateTitlesToPeopleAsync()
    {
      string path = _imdbPathsOptions.RawTitlePrincipalsFile;
      _logger.LogDebug("ReadFilterMoviesAsync, path: {0}", path);

      try
      {
        using (StreamReader sr = new StreamReader(path))
        {
          string? line = null;
          while ((line = await sr.ReadLineAsync()) != null)
          {
            Principal? principal = JsonSerializer.Deserialize<Principal>(line);
            if (principal != null && principal.IsValid() && people.ContainsKey(principal.NConst) && movies.ContainsKey(principal.TConst))
            {
              Person person = people[principal.NConst];
              Movie movie = movies[principal.TConst];
              movie.AddPerson(principal.NConst);
              person.AddTitle(principal.TConst);
              titleToPersonCount++;
            }
          }
        }
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "ReadFilterMoviesAsync, path: {0}", path);
      }
    }

    private async Task CreateWriteMovieSeedAsync()
    {
      string path = _imdbPathsOptions.MoviesSeedFile;
      _logger.LogDebug("CreateWriteMovieSeedAsync, path: {0}", path);

      List<SeedDocument> indexDocs = new List<SeedDocument>();
      foreach (var movie in movies)
      {
        Movie relatedMovie = movies[movie.Key];
        SeedDocument seedDoc = new SeedDocument(Constants.DOCTYPE_MOVIE_SEED, relatedMovie.Id, relatedMovie.Pk);
        foreach (var person in relatedMovie.People)
        {
          seedDoc.AddAdjacentVertex(person);
        }
        indexDocs.Add(seedDoc);
      }

      try
      {
        using (StreamWriter sw = new StreamWriter(path))
        {
          foreach (var indexDoc in indexDocs)
          {
            await sw.WriteLineAsync(JsonSerializer.Serialize(indexDoc));
          }
        }
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "CreateWriteMovieSeedAsync, path: {0}", path);
      }
    }

    private void DisplayEojTotals()
    {
      double elapsedMs = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - startMs;
      double elapsedSec = elapsedMs / 1000;
      double elapsedMin = elapsedSec / 60;

      _logger.LogInformation("EOJ Totals:");
      _logger.LogInformation($"Min Year: ${MinYear}");
      _logger.LogInformation($"Min Minutes: ${MinMinutes}");
      _logger.LogInformation($"Total Movie Count: ${totalMovieCount}");
      _logger.LogInformation($"Included Movie Count: ${movies.Count}");
      _logger.LogInformation($"Total Principal Count: ${totalPrincipalCount}");
      _logger.LogInformation($"Included Principal Count: ${principalSet.Count}");
      _logger.LogInformation($"Total Person Count: ${totalPersonCount}");
      _logger.LogInformation($"Included Person Count: ${people.Count}");
      _logger.LogInformation($"Included Movies + People: ${people.Count + movies.Count}");
      _logger.LogInformation($"Title to Person Count: ${titleToPersonCount}");
      _logger.LogInformation($"Elapsed Time (ms): ${elapsedMs}");
      _logger.LogInformation($"Elapsed Time (min): ${elapsedMin}");
    }
  }
}