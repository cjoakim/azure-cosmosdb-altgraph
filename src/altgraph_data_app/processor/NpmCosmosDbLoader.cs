using altgraph_shared_app.Models;
using altgraph_shared_app.Options;
using altgraph_shared_app.Services.Repositories;
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
    private readonly PathsOptions _pathsOptions;
    private bool doWrites = true;
    private const int COSMOS_DB_BATCH_LIMIT = 100;

    public NpmCosmosDbLoader(
        ILogger<NpmCosmosDbLoader> logger,
        IRepository<Author> authorRepository,
        IRepository<Library> libraryRepository,
        IRepository<Maintainer> maintainerRepository,
        IRepository<Triple> tripleRepository,
        IOptions<PathsOptions> pathsOptions)
    {
      _logger = logger;
      _authorRepository = new AuthorRepository(authorRepository);
      _libraryRepository = new LibraryRepository(libraryRepository);
      _maintainerRepository = new MaintainerRepository(maintainerRepository);
      _tripleRepository = new TripleRepository(tripleRepository);
      _pathsOptions = pathsOptions.Value;
    }

    public async Task ProcessAsync()
    {
      _logger.LogInformation("Starting CosmosDbLoader");

      Task[] tasks = new Task[]
      {
        Load<Author>(_pathsOptions.AuthorsFile, _authorRepository.Authors, "Authors"),
        Load<Maintainer>(_pathsOptions.MaintainersFile, _maintainerRepository.Maintainers, "Maintainers"),
        Load<Library>(_pathsOptions.LibrariesFile, _libraryRepository.Libraries, "Libraries"),
        Load<Triple>(_pathsOptions.TriplesFile, _tripleRepository.Triples, "Triples")
      };

      await Task.WhenAll(tasks);
    }

    private async Task Load<T>(string path, IRepository<T> repository, string documentType) where T : IItem
    {
      _logger.LogInformation($"Loading {documentType}...");

      try
      {
        List<T> documents = JsonSerializer.Deserialize<List<T>>(File.ReadAllText(path), new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        List<IEnumerable<T>> cosmosBatches = BuildChunksWithLinqAndYield(documents, COSMOS_DB_BATCH_LIMIT).ToList();

        _logger.LogInformation($"Loaded {documents.Count} of type {documentType}");

        // foreach (IEnumerable<T> batch in cosmosBatches)
        // {
        //   await repository.CreateAsBatchAsync(batch);
        // }
        foreach (T document in documents)
        {
          await repository.CreateAsync(document);
        }
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, $"Failed to load {documentType}.");
      }
    }

    private IEnumerable<IEnumerable<T>> BuildChunksWithLinqAndYield<T>(List<T> fullList, int batchSize)
    {
      int total = 0;
      while (total < fullList.Count)
      {
        yield return fullList.Skip(total).Take(batchSize);
        total += batchSize;
      }
    }
  }
}