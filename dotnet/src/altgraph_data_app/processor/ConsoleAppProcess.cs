using System.Text.Json;
using altgraph_data_app.common.io;
using altgraph_shared_app.Models.Imdb;
using altgraph_shared_app.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace altgraph_data_app.processor
{
  public class ConsoleAppProcess
  {
    private readonly ILogger<ConsoleAppProcess> _logger;
    private readonly JsonLoader _loader;
    private readonly ImdbPathsOptions _imdbPathsOptions;

    public ConsoleAppProcess(ILogger<ConsoleAppProcess> logger, IOptions<ImdbPathsOptions> imdbPathsOptions, JsonLoader loader)
    {
      _logger = logger;
      _imdbPathsOptions = imdbPathsOptions.Value;
      _loader = loader;
    }

    public async Task<Dictionary<string, Movie>> ReadMovieDocumentsAsync()
    {
      return await _loader.ReadMovieDocumentsAsync();
    }

    public async Task<Dictionary<string, Person>> ReadPeopleDocumentsAsync()
    {
      return await _loader.ReadPeopleDocumentsAsync();
    }

    public async Task<List<SmallTriple>> ReadSmallTriplesDocumentsAsync()
    {
      return await _loader.ReadSmallTriplesDocumentsAsync();
    }

    public async Task<List<SeedDocument>> ReadIndexDocumentsAsync()
    {
      return await _loader.ReadIndexDocumentsAsync();
    }

    public async void WriteMoviesOfInterest(Dictionary<string, Movie> movies)
    {
      string path = _imdbPathsOptions.MoviesOfInterestFile;
      _logger.LogDebug("WriteMoviesOfInterest, path: {0}", path);
      try
      {
        using (StreamWriter sw = new StreamWriter(path))
        {
          foreach (var movie in movies)
          {
            await sw.WriteLineAsync(JsonSerializer.Serialize(movie.Value));
            await sw.WriteLineAsync("---");
          }
        }
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "WriteMoviesOfInterest, path: {0}", path);
      }
    }

    public async void WritePeopleOfInterest(Dictionary<string, Person> people)
    {
      string path = _imdbPathsOptions.PeopleOfInterestFile;
      _logger.LogDebug("WritePeopleOfInterest, path: {0}", path);
      try
      {
        using (StreamWriter sw = new StreamWriter(path))
        {
          foreach (var person in people)
          {
            await sw.WriteLineAsync(JsonSerializer.Serialize(person.Value));
            await sw.WriteLineAsync("---");
          }
        }
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "WritePeopleOfInterest, path: {0}", path);
      }
    }

    // public async void WriteMovieMapFileAsync(Dictionary<string, Movie> movies)
    // {
    //   throw new NotImplementedException();
    // }

    // public async void WritePeopleMapFileAsync(Dictionary<string, Person> people)
    // {
    //   throw new NotImplementedException();
    // }
  }
}