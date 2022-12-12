using System.Text.Json;
using altgraph_web_app.Models;
using altgraph_web_app.Options;
using altgraph_web_app.Services.Cache;
using altgraph_web_app.Services.Graph;
using altgraph_web_app.Services.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Azure.CosmosRepository;
using Microsoft.Extensions.Options;

namespace altgraph_web_app.Pages;

public class GraphModel : PageModel
{
  private readonly ILogger<GraphModel> _logger;
  [BindProperty]
  public string SubjectName { get; set; } = string.Empty;
  [BindProperty]
  public bool AuthorCheckBox { get; set; } = false;
  [BindProperty]
  public int GraphDepth { get; set; } = 1;
  [BindProperty]
  public string? CacheOpts { get; set; } = string.Empty;
  [BindProperty]
  public string? ElapsedMs { get; set; } = string.Empty;
  [BindProperty(SupportsGet = true)]
  public string? NodesCsv { get; set; } = string.Empty;
  [BindProperty(SupportsGet = true)]
  public string? EdgesCsv { get; set; } = string.Empty;
  [BindProperty(SupportsGet = true)]
  public string? LibraryAsJson { get; set; } = "{}";
  public string? GraphJson { get; set; } = string.Empty;
  private readonly LibraryRepository _libraryRepository;
  private readonly AuthorRepository _authorRepository;
  private readonly TripleRepository _tripleRepository;
  private readonly Cache _cache;
  private readonly CacheOptions _cacheOptions;
  private readonly PathsOptions _pathsOptions;

  public GraphModel(ILogger<GraphModel> logger, IRepository<Library> libraryRepository, IRepository<Author> authorRepository, IRepository<Triple> tripleRepository, Cache cache, IOptions<CacheOptions> cacheOptions, IOptions<PathsOptions> pathsOptions)
  {
    _logger = logger;
    _libraryRepository = new LibraryRepository(libraryRepository);
    _authorRepository = new AuthorRepository(authorRepository);
    _tripleRepository = new TripleRepository(tripleRepository);
    _cache = cache;
    _logger = logger;
    _cacheOptions = cacheOptions.Value;
    _pathsOptions = pathsOptions.Value;
  }

  public void OnGet()
  {
  }

  public async Task<IActionResult> OnPostAsync()
  {
    if (!ModelState.IsValid)
    {
      return Page();
    }
    try
    {
      if (AuthorCheckBox)
      {
        await HandleAuthorSearchAsync();
      }
      else
      {
        await HandleLibrarySearchAsync();
      }
    
      Task[] tasks = { GetNodesCsvAsync(), GetEdgesCsvAsync() };

    Task.WaitAll(tasks);
    }
    catch (Exception ex)
    {
      _logger.LogError(ex.Message);
    }

    return Page();
  }

  private async Task HandleAuthorSearchAsync()
  {
    try
    {
      string libName = SubjectName;
      Author? author = ReadAuthorByLabel(libName);
      if (author != null)
      {
        TripleQueryStruct tripleQueryStruct = await ReadTriplesAsync(UseCachedTriples(CacheOpts));
        GraphBuilder graphBuilder = new GraphBuilder(author, tripleQueryStruct, _logger);
        Graph graph = graphBuilder.BuildAuthorGraph(author);
        D3CsvBuilder d3CsvBuilder = new D3CsvBuilder(graph, _pathsOptions, _logger);
        await d3CsvBuilder.BuildBillOfMaterialCsvAsync(GraphDepth);
        d3CsvBuilder.Finish();
      }
    }
    catch (Exception ex)
    {
      _logger.LogError(ex.Message);
    }
  }

  private async Task HandleLibrarySearchAsync()
  {
    try
    {
      string libName = SubjectName;
      Library? library = await ReadLibraryAsync(libName, UseCachedLibrary(CacheOpts));
      if (library != null)
      {
        TripleQueryStruct tripleQueryStruct = await ReadTriplesAsync(UseCachedTriples(CacheOpts));
        GraphBuilder graphBuilder = new GraphBuilder(library, tripleQueryStruct, _logger);
        Graph graph = graphBuilder.BuildLibraryGraph(GraphDepth);
        D3CsvBuilder d3CsvBuilder = new D3CsvBuilder(graph, _pathsOptions, _logger);
        await d3CsvBuilder.BuildBillOfMaterialCsvAsync(GraphDepth);
        d3CsvBuilder.Finish();
      }
    }
    catch (Exception ex)
    {
      _logger.LogError(ex.Message);
    }
  }

  private Author? ReadAuthorByLabel(string label)
  {
    Author? author = null;
    IEnumerable<Author> authors = _authorRepository.FindByLabelAsync(label).Result;
    foreach (Author a in authors)
    {
      author = a;
    }
    return author;
  }

  private async Task<TripleQueryStruct> ReadTriplesAsync(bool useCache)
  {
    if (useCache)
    {
      TripleQueryStruct? returnValue = await _cache.GetTriplesAsync();
      if (returnValue != null)
      {
        return returnValue;
      }
    }

    string lob = "npm";
    string subject = "library";
    TripleQueryStruct tripleQueryStruct = new TripleQueryStruct();
    tripleQueryStruct.Sql = "dynamic";
    tripleQueryStruct.Start();

    string pk = "triple|" + _cacheOptions.Tenant;
    IEnumerable<Triple> triples = _tripleRepository.GetByPkLobAndSubjectsAsync(pk, lob, subject, subject).Result;
    foreach (Triple triple in triples)
    {
      Triple t = triple;
      t.SetKeyFields();
      tripleQueryStruct.AddDocument(t);
    }
    tripleQueryStruct.Stop();
    try
    {
      await _cache.PutTriplesAsync(tripleQueryStruct);
    }
    catch (Exception ex)
    {
      _logger.LogError(ex.Message);
    }
    return tripleQueryStruct;
  }

  private static bool UseCachedLibrary(string? cacheOpts)
  {
    if (cacheOpts == null)
    {
      return false;
    }

    return cacheOpts.ToUpper().Contains('L');
  }

  private static bool UseCachedTriples(string? cacheOpts)
  {
    if (cacheOpts == null)
    {
      return false;
    }

    return cacheOpts.ToUpper().Contains('T');
  }

  private async Task GetNodesCsvAsync()
  {
    NodesCsv = await ReadCsvAsync(_pathsOptions.NodesCsvFile);
  }

  private async Task GetEdgesCsvAsync()
  {
    EdgesCsv = await ReadCsvAsync(_pathsOptions.EdgesCsvFile);
  }

  public async Task OnGetLibraryAsJsonAsync(string libraryName)
  {
    _logger.LogWarning($"getLibraryAsJson, libraryName: {libraryName}");

    Library? library = await ReadLibraryAsync(libraryName, false);
    if (library != null)
    {
      try
      {
        LibraryAsJson = JsonSerializer.Serialize(library);
      }
      catch (Exception e)
      {
        _logger.LogError(e.StackTrace);
        LibraryAsJson = "{}";
      }
    }
    else
    {
      LibraryAsJson = "{}";
    }
  }

  private async Task<Library?> ReadLibraryAsync(string libName, bool useCache)
  {
    Library? lib = null;

    if (useCache)
    {
      lib = await _cache.GetLibraryAsync(libName);
      if (lib != null)
      {
        return lib;
      }
    }

    IEnumerable<Library> libraries = _libraryRepository.FindByPkAndTenantAndDoctypeAsync(libName, _cacheOptions.Tenant, "library").Result;
    foreach (Library library in libraries)
    {
      lib = library;
      lib.GraphKey = lib.CalculateGraphKey();
      try
      {
        await _cache.PutLibraryAsync(lib);
      }
      catch (Exception ex)
      {
        _logger.LogError(ex.Message);
      }
    }
    return lib;
  }

  private static async Task<string> ReadCsvAsync(string path)
  {
    if (System.IO.File.Exists(path))
    {
      return await System.IO.File.ReadAllTextAsync(path);
    }
    else
    {
      return string.Empty;
    }
  }
}
