using altgraph_shared_app.Models.Npm;
using altgraph_shared_app.Options;
using altgraph_shared_app.Services.Cache;
using altgraph_shared_app.Services.Graph.v1;
using altgraph_web_app.Services.Graph;
using altgraph_shared_app.Services.Repositories.Npm;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Azure.CosmosRepository;
using Microsoft.Extensions.Options;
using altgraph_shared_app;

namespace altgraph_web_app.Areas.NpmGraph.Pages;

public class IndexModel : PageModel
{
  private readonly ILogger<IndexModel> _logger;
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
  private readonly LibraryRepository _libraryRepository;
  private readonly AuthorRepository _authorRepository;
  private readonly TripleRepository _tripleRepository;
  private readonly ICache _cache;
  private readonly CacheOptions _cacheOptions;
  private readonly NpmPathsOptions _pathsOptions;
  private readonly NpmOptions _npmOptions;

  public IndexModel(ILogger<IndexModel> logger, IRepository<Library> libraryRepository, IRepository<Author> authorRepository, IRepository<Triple> tripleRepository, ICache cache, IOptions<CacheOptions> cacheOptions, IOptions<NpmPathsOptions> pathsOptions, IOptions<NpmOptions> npmOptions)
  {
    _logger = logger;
    _libraryRepository = new LibraryRepository(libraryRepository);
    _authorRepository = new AuthorRepository(authorRepository);
    _tripleRepository = new TripleRepository(tripleRepository);
    _cache = cache;
    _cacheOptions = cacheOptions.Value;
    _pathsOptions = pathsOptions.Value;
    _npmOptions = npmOptions.Value;
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

    DateTime start = DateTime.Now;

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
    }
    catch (Exception ex)
    {
      _logger.LogError(ex.Message);
    }

    DateTime end = DateTime.Now;

    ElapsedMs = $"{Math.Round((end - start).TotalMilliseconds)} ms";

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

    string lob = Constants.LOB_NPM_LIBRARIES;
    string subject = "library";
    TripleQueryStruct tripleQueryStruct = new();
    tripleQueryStruct.Sql = "dynamic";
    tripleQueryStruct.Start();

    string pk = "triple|" + _npmOptions.DefaultTenant;
    IEnumerable<Triple> triples = _tripleRepository.GetByPkLobAndSubjectsAsync(pk, lob, subject, subject).Result;
    foreach (Triple triple in triples)
    {
      Triple t = triple;
      t.SetKeyFields();
      tripleQueryStruct.Documents.Add(t);
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

  public async Task<IActionResult> OnGetNodesCsvAsync()
  {
    return Content(await ReadCsvAsync(_pathsOptions.NodesCsvFile), "text/csv");
  }

  public async Task<IActionResult> OnGetEdgesCsvAsync()
  {
    return Content(await ReadCsvAsync(_pathsOptions.EdgesCsvFile), "text/csv");
  }

  public async Task<JsonResult?> OnGetLibraryAsJson(string libraryName)
  {
    _logger.LogDebug($"getLibraryAsJson, libraryName: {libraryName}");

    Library? library = await ReadLibraryAsync(libraryName, false);
    if (library != null)
    {
      try
      {
        return new JsonResult(library);
      }
      catch (Exception e)
      {
        _logger.LogError(e.StackTrace);
        return null;
      }
    }
    else
    {
      return null;
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
