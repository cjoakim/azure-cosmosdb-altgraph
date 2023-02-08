using altgraph_data_app.common.dao;
using altgraph_data_app.common.io;
using altgraph_shared_app.Models.Imdb;
using altgraph_shared_app.Options;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace altgraph_data_app.processor
{
  public class SdkBulkLoaderProcessor : IConsoleAppProcess
  {
    private readonly ILogger<SdkBulkLoaderProcessor> _logger;
    private readonly ImdbPathsOptions _imdbPathsOptions;
    private readonly ImdbOptions _imdbOptions;
    private readonly CosmosOptions _cosmosOptions;
    private readonly ConsoleAppProcess _consoleAppProcess;
    private CosmosClient _cosmosClient;
    private CosmosAsyncDao _cosmosAsyncDao;
    public string Container { get; set; } = string.Empty;
    public string LoadType { get; set; } = string.Empty;

    public SdkBulkLoaderProcessor(ILogger<SdkBulkLoaderProcessor> logger, IOptions<ImdbPathsOptions> imdbPathsOptions, CosmosClient cosmosClient, IOptions<ImdbOptions> imdbOptions, ConsoleAppProcess consoleAppProcess, IOptions<CosmosOptions> cosmosOptions, CosmosAsyncDao cosmosAsyncDao)
    {
      _logger = logger;
      _imdbPathsOptions = imdbPathsOptions.Value;
      _imdbOptions = imdbOptions.Value;
      _cosmosOptions = cosmosOptions.Value;
      _cosmosClient = cosmosClient;
      _cosmosAsyncDao = cosmosAsyncDao;
      _cosmosAsyncDao.CurrentDbName = _cosmosOptions.DatabaseId;
      _consoleAppProcess = consoleAppProcess;
    }
    public async Task ProcessAsync()
    {
      _cosmosAsyncDao.Initialize();
      _cosmosAsyncDao.CurrentContainer(Container);

      switch (LoadType)
      {
        case "imdb_bulk_load_movies":
          await BulkLoadImdbMoviesAsync();
          break;
        case "imdb_bulk_load_people":
          await BulkLoadImdbPeopleAsync();
          break;
        case "imdb_bulk_load_small_triples":
          await BulkLoadImdbSmallTriplesAsync();
          break;
        case "imdb_bulk_load_movies_seed":
          await BulkLoadImdbMovieSeedAsync();
          break;
        default:
          throw new ArgumentException("Invalid load type");
      }
      _cosmosAsyncDao.Close();
    }

    private async Task BulkLoadImdbMoviesAsync()
    {
      _logger.LogInformation("Bulk loading {0} into {1}", LoadType, Container);
      List<Movie> items = await _consoleAppProcess.ReadMovieDocumentsAsync().ContinueWith(t => t.Result.Values.Cast<Movie>().ToList());
      _logger.LogInformation("Read {0} documents from disk", items.Count);
      long startMs = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
      await _cosmosAsyncDao.BulkLoadAsync<Movie>(items);
      long elapsedMs = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - startMs;
      _logger.LogInformation("Bulk load complete in {0} ms", elapsedMs);
    }
    private async Task BulkLoadImdbPeopleAsync()
    {
      _logger.LogInformation("Bulk loading {0} into {1}", LoadType, Container);
      List<Person> items = await _consoleAppProcess.ReadPeopleDocumentsAsync().ContinueWith(t => t.Result.Values.Cast<Person>().ToList());
      _logger.LogInformation("Read {0} documents from disk", items.Count);
      long startMs = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
      await _cosmosAsyncDao.BulkLoadAsync<Person>(items);
      long elapsedMs = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - startMs;
      _logger.LogInformation("Bulk load complete in {0} ms", elapsedMs);
    }
    private async Task BulkLoadImdbSmallTriplesAsync()
    {
      _logger.LogInformation("Bulk loading {0} into {1}", LoadType, Container);
      List<SmallTriple> items = await _consoleAppProcess.ReadSmallTriplesDocumentsAsync();
      _logger.LogInformation("Read {0} documents from disk", items.Count);
      long startMs = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
      await _cosmosAsyncDao.BulkLoadAsync<SmallTriple>(items);
      long elapsedMs = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - startMs;
      _logger.LogInformation("Bulk load complete in {0} ms", elapsedMs);
    }
    private async Task BulkLoadImdbMovieSeedAsync()
    {
      _logger.LogInformation("Bulk loading {0} into {1}", LoadType, Container);
      List<SeedDocument> items = await _consoleAppProcess.ReadIndexDocumentsAsync();
      _logger.LogInformation("Read {0} documents from disk", items.Count);
      long startMs = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
      await _cosmosAsyncDao.BulkLoadAsync<SeedDocument>(items);
      long elapsedMs = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - startMs;
      _logger.LogInformation("Bulk load complete in {0} ms", elapsedMs);
    }
  }
}