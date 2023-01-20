using altgraph_shared_app.Models.Npm;
using altgraph_shared_app.Options;
using altgraph_shared_app.Services.Repositories.Npm;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.CosmosRepository;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace altgraph_data_app.processor
{
  public class NpmCosmosDbLoader : IConsoleAppProcess
  {
    private readonly ILogger<NpmCosmosDbLoader> _logger;
    private readonly AuthorRepository _authorRepository;
    private readonly LibraryRepository _libraryRepository;
    private readonly MaintainerRepository _maintainerRepository;
    private readonly TripleRepository _tripleRepository;
    private readonly NpmPathsOptions _npmPathsOptions;

    public NpmCosmosDbLoader(
        ILogger<NpmCosmosDbLoader> logger,
        IRepository<Author> authorRepository,
        IRepository<Library> libraryRepository,
        IRepository<Maintainer> maintainerRepository,
        IRepository<Triple> tripleRepository,
        IOptions<NpmPathsOptions> npmPathsOptions)
    {
      _logger = logger;
      _authorRepository = new AuthorRepository(authorRepository);
      _libraryRepository = new LibraryRepository(libraryRepository);
      _maintainerRepository = new MaintainerRepository(maintainerRepository);
      _tripleRepository = new TripleRepository(tripleRepository);
      _npmPathsOptions = npmPathsOptions.Value;
    }

    public async Task ProcessAsync()
    {
      _logger.LogInformation("Starting CosmosDbLoader");

      Task[] tasks = new Task[]
      {
        Load<Author>(_npmPathsOptions.AuthorsFile, _authorRepository.Authors, "Authors"),
        Load<Maintainer>(_npmPathsOptions.MaintainersFile, _maintainerRepository.Maintainers, "Maintainers"),
        Load<Library>(_npmPathsOptions.LibrariesFile, _libraryRepository.Libraries, "Libraries"),
        Load<Triple>(_npmPathsOptions.TriplesFile, _tripleRepository.Triples, "Triples")
      };

      await Task.WhenAll(tasks);
    }

    private async Task Load<T>(string path, IRepository<T> repository, string documentType) where T : NpmDocument
    {
      _logger.LogInformation($"Loading {documentType}...");

      List<T>? documents = new List<T>();

      try
      {
        documents = JsonSerializer.Deserialize<List<T>>(File.ReadAllText(path), new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        _logger.LogInformation($"Loaded {(documents != null ? documents.Count : 0)} of type {documentType}");
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, $"Failed to load {documentType}.");
        throw;
      }

      if (documents != null)
      {
        List<Task> tasks = new List<Task>(documents.Count);
        foreach (T document in documents)
        {
          tasks.Add(repository.CreateAsync(document).AsTask().ContinueWith(itemResponse =>
          {
            if (!itemResponse.IsCompletedSuccessfully)
            {
              AggregateException innerExceptions = itemResponse.Exception.Flatten();
              if (innerExceptions.InnerExceptions.FirstOrDefault(innerEx => innerEx is CosmosException) is CosmosException cosmosException)
              {
                _logger.LogDebug($"Received {cosmosException.StatusCode} ({cosmosException.Message}).");
              }
              else
              {
                _logger.LogDebug($"Exception {innerExceptions.InnerExceptions.FirstOrDefault()}.");
              }
            }
          }));
        }

        await Task.WhenAll(tasks);

        _logger.LogInformation($"Added {documents.Count} documents of type {documentType} to database.");
      }
    }
  }
}