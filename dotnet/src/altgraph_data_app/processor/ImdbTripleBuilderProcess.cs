using System.Text.Json;
using altgraph_data_app.common.io;
using altgraph_shared_app;
using altgraph_shared_app.Models.Imdb;
using altgraph_shared_app.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace altgraph_data_app.processor
{
  public class ImdbTripleBuilderProcess : IConsoleAppProcess
  {
    private long tripleLinesWritten = 0;
    private long startMs = 0;
    private Dictionary<string, Movie> movies = new Dictionary<string, Movie>();
    private Dictionary<string, Person> people = new Dictionary<string, Person>();
    private ILogger<ImdbTripleBuilderProcess> _logger;
    private ImdbPathsOptions _imdbPathsOptions;
    private ConsoleAppProcess _consoleAppProcess;

    public ImdbTripleBuilderProcess(ILogger<ImdbTripleBuilderProcess> logger, IOptions<ImdbPathsOptions> imdbPathsOptions, ConsoleAppProcess consoleAppProcess)
    {
      _logger = logger;
      _imdbPathsOptions = imdbPathsOptions.Value;
      _consoleAppProcess = consoleAppProcess;
    }
    public async Task ProcessAsync()
    {
      startMs = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
      movies = await _consoleAppProcess.ReadMovieDocumentsAsync();
      people = await _consoleAppProcess.ReadPeopleDocumentsAsync();

      using (StreamWriter streamWriter = new StreamWriter(_imdbPathsOptions.SmallTriplesDocumentsFile))
      {
        foreach (Person person in people.Values)
        {
          string nconst = person.NConst;
          foreach (string title in person.Titles)
          {
            Movie movie = movies[title];

            //create the person-in-movie Triple
            SmallTriple t = new SmallTriple();
            t.SetCosmosDbSmallTripleCoordinateAttributes();
            t.SubjectType = person.Doctype;
            t.SubjectIdPk = person.NConst;
            t.Predicate = Constants.PREDICATE_IN_MOVIE;
            t.ObjectType = movie.Doctype;
            t.ObjectIdPk = movie.TConst;
            await streamWriter.WriteLineAsync(JsonSerializer.Serialize(t));

            //create the movie-has-person Triple
            t = new SmallTriple();
            t.SetCosmosDbSmallTripleCoordinateAttributes();
            t.SubjectType = movie.Doctype;
            t.SubjectIdPk = movie.TConst;
            t.Predicate = Constants.PREDICATE_HAS_PERSON;
            t.ObjectType = person.Doctype;
            t.ObjectIdPk = person.NConst;
            await streamWriter.WriteLineAsync(JsonSerializer.Serialize(t));
          }
        }
      }
      DisplayEojTotals();
    }

    private void DisplayEojTotals()
    {
      double elapsedMs = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - startMs;
      double elapsedSec = elapsedMs / 1000;
      double elapsedMin = elapsedSec / 60;

      _logger.LogInformation("EOJ Totals");
      _logger.LogInformation($"Movies Size: {movies.Count}");
      _logger.LogInformation($"People Size: {people.Count}");
      _logger.LogInformation($"Triples Written: {tripleLinesWritten}");
      _logger.LogInformation($"Elapsed Time: {elapsedMs} ms");
      _logger.LogInformation($"Elapsed Time: {elapsedMin} min");
    }
  }
}