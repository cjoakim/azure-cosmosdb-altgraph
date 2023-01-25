using System.Text.Json;
using altgraph_shared_app.Models.Imdb;
using altgraph_shared_app.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace altgraph_data_app.common.io
{
  public class JsonLoader
  {
    private readonly ILogger<JsonLoader> _logger;
    private readonly ImdbPathsOptions _imdbPathsOptions;
    public JsonLoader(ILogger<JsonLoader> logger, IOptions<ImdbPathsOptions> imdbPathsOptions)
    {
      _logger = logger;
      _imdbPathsOptions = imdbPathsOptions.Value;
    }

    public async Task<Dictionary<string, Movie>> ReadMovieDocumentsAsync()
    {
      Dictionary<string, Movie> movies = new Dictionary<string, Movie>();
      using (StreamReader reader = File.OpenText(_imdbPathsOptions.MoviesDocumentsFile))
      {
        string? line = null;
        while ((line = await reader.ReadLineAsync()) != null)
        {
          if (line != null)
          {
            Movie? movie = JsonSerializer.Deserialize<Movie>(line);
            if (movie != null)
            {
              movies.Add(movie.TConst, movie);
            }
          }
        }
      }

      return movies;
    }

    public async Task<Dictionary<string, Person>> ReadPeopleDocumentsAsync()
    {
      Dictionary<string, Person> people = new Dictionary<string, Person>();
      using (StreamReader reader = File.OpenText(_imdbPathsOptions.PeopleDocumentsFile))
      {
        string? line = null;
        while ((line = await reader.ReadLineAsync()) != null)
        {
          Person? person = JsonSerializer.Deserialize<Person>(line);
          if (person != null)
          {
            people.Add(person.NConst, person);
          }
        }
      }

      return people;
    }

    public async Task<List<SmallTriple>> ReadSmallTriplesDocumentsAsync()
    {
      List<SmallTriple> smallTriples = new List<SmallTriple>();
      using (StreamReader reader = File.OpenText(_imdbPathsOptions.SmallTriplesDocumentsFile))
      {
        string? line = null;
        while ((line = await reader.ReadLineAsync()) != null)
        {
          SmallTriple? smallTriple = JsonSerializer.Deserialize<SmallTriple>(line);
          if (smallTriple != null)
          {
            smallTriples.Add(smallTriple);
          }
        }
      }

      return smallTriples;
    }

    public async Task<List<SeedDocument>> ReadIndexDocumentsAsync()
    {
      List<SeedDocument> seedDocuments = new List<SeedDocument>();
      using (StreamReader reader = File.OpenText(_imdbPathsOptions.MoviesSeedFile))
      {
        string? line = null;
        while ((line = await reader.ReadLineAsync()) != null)
        {
          SeedDocument? seedDocument = JsonSerializer.Deserialize<SeedDocument>(line);
          if (seedDocument != null)
          {
            seedDocuments.Add(seedDocument);
          }
        }
      }

      return seedDocuments;
    }
  }
}