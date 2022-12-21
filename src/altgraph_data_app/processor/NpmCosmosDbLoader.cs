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
        throw ex;
      }

      if (documents != null)
      {
        foreach (T document in documents)
        {
          if (!await repository.ExistsAsync(document.Id, document.Pk))
          {
            try
            {
              //TODO: Replace with batch (need to debug input data)
              await repository.CreateAsync(document);
            }
            catch (Exception ex)
            {
              _logger.LogError(ex, $"Failed to load {documentType}. ${JsonSerializer.Serialize(document)}");
            }
          }
        }

        _logger.LogInformation($"Added {documents.Count} documents of type {documentType} to database.");
      }
    }
  }
}