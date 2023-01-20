using System.Text.Json;
using altgraph_data_app.common.io;
using altgraph_shared_app.Models.Imdb;
using altgraph_shared_app.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace altgraph_data_app.processor
{
  public abstract class AbstractConsoleAppProcess<T> : IConsoleAppProcess
  {
    public abstract Task ProcessAsync();
    private readonly ILogger<T> _logger;
    private readonly JsonLoader _loader;
    private readonly ImdbPathsOptions _imdbPathsOptions;

    protected AbstractConsoleAppProcess(ILogger<T> logger, IOptions<ImdbPathsOptions> imdbPathsOptions, JsonLoader loader)
    {
      _logger = logger;
      _imdbPathsOptions = imdbPathsOptions.Value;
      _loader = loader;
    }

    protected async Task<Dictionary<string, Movie>> ReadMovieDocumentsAsync()
    {
      return await _loader.ReadMovieDocumentsAsync();
    }

    protected async Task<Dictionary<string, Person>> ReadPeopleDocumentsAsync()
    {
      return await _loader.ReadPeopleDocumentsAsync();
    }

    protected async Task<List<SmallTriple>> ReadSmallTriplesDocumentsAsync()
    {
      return await _loader.ReadSmallTriplesDocumentsAsync();
    }

    protected async Task<List<SeedDocument>> ReadIndexDocumentsAsync()
    {
      return await _loader.ReadIndexDocumentsAsync();
    }

    protected async void WriteMoviesOfInterest(Dictionary<string, Movie> movies)
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

    protected async void WritePeopleOfInterest(Dictionary<string, Person> people)
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

    // protected async void WriteMovieMapFileAsync(Dictionary<string, Movie> movies)
    // {
    //   throw new NotImplementedException();
    // }

    // protected async void WritePeopleMapFileAsync(Dictionary<string, Person> people)
    // {
    //   throw new NotImplementedException();
    // }
  }
}