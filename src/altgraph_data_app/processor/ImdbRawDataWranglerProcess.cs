using System.Text.Json;
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
    private long _totalMovieCount = 0;
    private long _totalPersonCount = 0;
    private long _totalPrincipalCount = 0;
    private long _titleToPersonCount = 0;
    private long _startMs = 0;
    private Dictionary<string, Movie> _movies = new Dictionary<string, Movie>();
    private Dictionary<string, Person> _people = new Dictionary<string, Person>();
    private HashSet<string> _principalSet = new HashSet<string>();

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
      _startMs = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

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
      _logger.LogDebug($"ReadFilterMoviesAsync, path: {path}");

      bool isFirstLine = true;

      try
      {
        using (StreamReader sr = new StreamReader(path))
        {
          string? line = null;
          while ((line = await sr.ReadLineAsync()) != null)
          {
            if (isFirstLine)
            {
              isFirstLine = false;
              continue;
            }
            string[] lineTokens = line.Split("\t");
            Movie movie = new Movie(lineTokens);
            if (movie.Include(MinYear, MinMinutes))
            {
              movie.SetCosmosDbCoordinateAttributes(movie.TConst, Constants.DOCTYPE_MOVIE);
              _movies.Add(movie.TConst, movie);
              _totalMovieCount++;
            }
          }
        }
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, $"ReadFilterMoviesAsync, path: {path}");
      }
    }

    private async Task ReadIdentifyPrincipalsInMoviesAsync()
    {
      string path = _imdbPathsOptions.RawTitlePrincipalsFile;
      _logger.LogDebug($"ReadIdentifyPrincipalsInMoviesAsync, path: {path}");

      bool isFirstLine = true;

      try
      {
        using (StreamReader sr = new StreamReader(path))
        {
          string? line = null;
          while ((line = await sr.ReadLineAsync()) != null)
          {
            if (isFirstLine)
            {
              isFirstLine = false;
              continue;
            }

            string[] lineTokens = line.Split("\t");
            Principal principal = new Principal(lineTokens);
            principal.Doctype = Constants.DOCTYPE_PRINCIPAL;
            _totalPrincipalCount++;

            if (principal.IsValid() && principal.HasNecessaryRole() && _movies.ContainsKey(principal.TConst))
            {
              _principalSet.Add(principal.NConst);
            }
          }
        }
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, $"ReadFilterMoviesAsync, path: {path}");
      }
    }

    private async Task ReadNamesOfPrincipalsAsync()
    {
      string path = _imdbPathsOptions.RawNameBasicsFile;
      _logger.LogDebug($"ReadIdentifyPrincipalsInMoviesAsync, path: {path}");

      bool isFirstLine = true;

      try
      {
        using (StreamReader sr = new StreamReader(path))
        {
          string? line = null;
          while ((line = await sr.ReadLineAsync()) != null)
          {
            if (isFirstLine)
            {
              isFirstLine = false;
              continue;
            }

            string[] lineTokens = line.Split("\t");
            Person person = new Person(lineTokens);
            person.Doctype = Constants.DOCTYPE_PRINCIPAL;
            _totalPersonCount++;

            if (person.IsValid() && _principalSet.Contains(person.NConst))
            {
              person.ScrubValues();
              person.SetCosmosDbCoordinateAttributes(person.NConst, Constants.DOCTYPE_PERSON);
              _people.Add(person.NConst, person);
            }
          }
        }
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, $"ReadIdentifyPrincipalsInMoviesAsync, path: {path}");
      }
    }

    private async Task WriteMoviesAsync()
    {
      string path = _imdbPathsOptions.MoviesDocumentsFile;
      _logger.LogDebug($"WriteMoviesAsync, path: {path}");
      try
      {
        using (StreamWriter sw = new StreamWriter(path))
        {
          foreach (var movie in _movies)
          {
            await sw.WriteLineAsync(JsonSerializer.Serialize(movie.Value));
          }
        }
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, $"WriteMoviesAsync, path: {path}");
      }
    }

    private async Task WritePeopleAsync()
    {
      string path = _imdbPathsOptions.PeopleDocumentsFile;
      _logger.LogDebug($"WritePeopleAsync, path: {path}");
      try
      {
        using (StreamWriter sw = new StreamWriter(path))
        {
          foreach (var people in _people)
          {
            await sw.WriteLineAsync(JsonSerializer.Serialize(people.Value));
          }
        }
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, $"WritePeopleAsync, path: {path}");
      }
    }

    private async Task AssociateTitlesToPeopleAsync()
    {
      string path = _imdbPathsOptions.RawTitlePrincipalsFile;
      _logger.LogDebug($"ReadFilterMoviesAsync, path: {path}");

      bool isFirstLine = true;

      try
      {
        using (StreamReader sr = new StreamReader(path))
        {
          string? line = null;
          while ((line = await sr.ReadLineAsync()) != null)
          {
            if (isFirstLine)
            {
              isFirstLine = false;
              continue;
            }

            string[] lineTokens = line.Split("\t");
            Principal? principal = new Principal(lineTokens);
            if (principal.IsValid() && _people.ContainsKey(principal.NConst) && _movies.ContainsKey(principal.TConst))
            {
              Person person = _people[principal.NConst];
              Movie movie = _movies[principal.TConst];
              movie.AddPerson(principal.NConst);
              person.AddTitle(principal.TConst);
              _titleToPersonCount++;
            }
          }
        }
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, $"ReadFilterMoviesAsync, path: {path}");
      }
    }

    private async Task CreateWriteMovieSeedAsync()
    {
      string path = _imdbPathsOptions.MoviesSeedFile;
      _logger.LogDebug($"CreateWriteMovieSeedAsync, path: {path}");

      List<SeedDocument> indexDocs = new List<SeedDocument>();
      foreach (var movie in _movies)
      {
        Movie relatedMovie = _movies[movie.Key];
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
        _logger.LogError(ex, $"CreateWriteMovieSeedAsync, path: {path}");
      }
    }

    private void DisplayEojTotals()
    {
      double elapsedMs = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - _startMs;
      double elapsedSec = elapsedMs / 1000;
      double elapsedMin = elapsedSec / 60;

      _logger.LogInformation("EOJ Totals:");
      _logger.LogInformation($"Min Year: {MinYear}");
      _logger.LogInformation($"Min Minutes: {MinMinutes}");
      _logger.LogInformation($"Total Movie Count: {_totalMovieCount}");
      _logger.LogInformation($"Included Movie Count: {_movies.Count}");
      _logger.LogInformation($"Total Principal Count: {_totalPrincipalCount}");
      _logger.LogInformation($"Included Principal Count: {_principalSet.Count}");
      _logger.LogInformation($"Total Person Count: {_totalPersonCount}");
      _logger.LogInformation($"Included Person Count: {_people.Count}");
      _logger.LogInformation($"Included Movies + People: {_people.Count + _movies.Count}");
      _logger.LogInformation($"Title to Person Count: {_titleToPersonCount}");
      _logger.LogInformation($"Elapsed Time (ms): {elapsedMs}");
      _logger.LogInformation($"Elapsed Time (min): {elapsedMin}");
    }
  }
}