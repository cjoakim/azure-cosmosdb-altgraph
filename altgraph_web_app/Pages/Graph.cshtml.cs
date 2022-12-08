using System.Text.Json;
using altgraph_web_app.Models;
using altgraph_web_app.Services.Cache;
using altgraph_web_app.Services.Graph;
using altgraph_web_app.Services.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Azure.CosmosRepository;

namespace altgraph_web_app.Pages;

public class GraphModel : PageModel
{
  private readonly ILogger<GraphModel> _logger;
  private static int DEFAULT_DEPTH = 1;
  [BindProperty]
  public string SubjectName { get; set; } = "";
  [BindProperty]
  public bool AuthorCheckBox { get; set; } = false;
  [BindProperty]
  public string GraphDepth { get; set; } = "1";
  [BindProperty]
  public string? CacheOpts { get; set; } = "";
  [BindProperty]
  public string? ElapsedMs { get; set; } = "";
  [BindProperty]
  public string? SessionId { get; set; } = "";
  [BindProperty(SupportsGet = true)]
  public string? NodesCsv { get; set; } = "";
  [BindProperty(SupportsGet = true)]
  public string? EdgesCsv { get; set; } = "";
  [BindProperty(SupportsGet = true)]
  public string? LibraryAsJson { get; set; } = "{}";
  public string? GraphJson { get; set; } = "";
  private LibraryRepository _libraryRepository;
  private AuthorRepository _authorRepository;
  private TripleRepository _tripleRepository;
  private Cache _cache;
  private IConfiguration _configuration;

  public GraphModel(ILogger<GraphModel> logger, IRepository<Library> libraryRepository, IRepository<Author> authorRepository, IRepository<Triple> tripleRepository, Cache cache, IConfiguration configuration)
  {
    _logger = logger;
    _libraryRepository = new LibraryRepository(libraryRepository);
    _authorRepository = new AuthorRepository(authorRepository);
    _tripleRepository = new TripleRepository(tripleRepository);
    _cache = cache;
    _logger = logger;
    _configuration = configuration;
  }

  public void OnGet()
  {
    SubjectName = "";
    AuthorCheckBox = false;
    GraphDepth = "1";
    CacheOpts = "";
    ElapsedMs = "";
    SessionId = HttpContext.Session.Id;
  }

  public int GetDepthAsInt()
  {
    try
    {
      return int.Parse(GraphDepth);
    }
    catch (Exception e)
    {
      _logger.LogError($"non-integer depth value: {GraphDepth}. returning the default");
      return DEFAULT_DEPTH;
    }
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
    }
    catch (Exception ex)
    {

    }

    Task[] tasks = { GetNodesCsvAsync(), GetEdgesCsvAsync() };

    Task.WaitAll(tasks);

    return Page();
  }

  private async Task HandleAuthorSearchAsync()
  {
    try
    {
      string libName = SubjectName;
      Author author = ReadAuthorByLabel(libName, UseCachedLibrary(CacheOpts));
      if (author != null)
      {
        TripleQueryStruct tripleQueryStruct = await ReadTriplesAsync(UseCachedTriples(CacheOpts), SessionId);
        GraphBuilder graphBuilder = new GraphBuilder(author, tripleQueryStruct, _logger);
        Graph graph = graphBuilder.BuildAuthorGraph(author);
        D3CsvBuilder d3CsvBuilder = new D3CsvBuilder(graph, _configuration, _logger);
        d3CsvBuilder.BuildBillOfMaterialCsv(SessionId, GetDepthAsInt());
        d3CsvBuilder.Finish();
      }
    }
    catch (Exception ex)
    {

    }
  }

  private async Task HandleLibrarySearchAsync()
  {
    try
    {
      string libName = SubjectName;
      Library library = await ReadLibraryAsync(libName, UseCachedLibrary(CacheOpts));
      if (library != null)
      {
        TripleQueryStruct tripleQueryStruct = await ReadTriplesAsync(UseCachedTriples(CacheOpts), SessionId);
        GraphBuilder graphBuilder = new GraphBuilder(library, tripleQueryStruct, _logger);
        Graph graph = graphBuilder.BuildLibraryGraph(GetDepthAsInt());
        D3CsvBuilder d3CsvBuilder = new D3CsvBuilder(graph, _configuration, _logger);
        d3CsvBuilder.BuildBillOfMaterialCsv(SessionId, GetDepthAsInt());
        d3CsvBuilder.Finish();
      }
    }
    catch (Exception ex)
    {

    }
  }

  private Author ReadAuthorByLabel(string label, bool useCache)
  {
    Author? author = null;
    IEnumerable<Author> authors = _authorRepository.FindByLabel(label).Result;
    foreach (Author a in authors)
    {
      author = a;
    }
    return author;
  }

  private async Task<TripleQueryStruct> ReadTriplesAsync(bool useCache, string sessionId)
  {
    string cacheFilename = _configuration["Paths:TripleQueryCacheStructCacheFile"];
    if (useCache)
    {
      TripleQueryStruct returnValue = await _cache.GetTriplesAsync();
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

    string pk = "triple|" + _configuration["Tenant"];
    IEnumerable<Triple> triples = _tripleRepository.GetByPkLobAndSubjects(pk, lob, subject, subject).Result;
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

    }
    return tripleQueryStruct;
  }

  private bool UseCachedLibrary(string cacheOpts)
  {
    if (cacheOpts == null)
    {
      return false;
    }

    return cacheOpts.ToUpper().Contains("L");
  }

  private bool UseCachedTriples(string cacheOpts)
  {
    if (cacheOpts == null)
    {
      return false;
    }

    return cacheOpts.ToUpper().Contains("T");
  }

  private async Task GetNodesCsvAsync()
  {
    NodesCsv = await ReadCsvAsync(_configuration["Paths:NodesCsvFile"]);
  }

  private async Task GetEdgesCsvAsync()
  {
    EdgesCsv = await ReadCsvAsync(_configuration["Paths:EdgesCsvFile"]);
  }

  public async void OnGetLibraryAsJsonAsync(string libraryName)
  {
    _logger.LogWarning($"getLibraryAsJson, libraryName: {libraryName}");

    Library library = await ReadLibraryAsync(libraryName, false);
    if (library != null)
    {
      try
      {
        LibraryAsJson = JsonSerializer.Serialize<Library>(library);
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

  private async Task<Library> ReadLibraryAsync(string libName, bool useCache)
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

    IEnumerable<Library> libraries = _libraryRepository.FindByPkAndTenantAndDoctype(libName, _configuration["Tenant"], "library").Result;
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

      }
    }
    return lib;
  }

  private async Task<string> ReadCsvAsync(string path)
  {
    if (System.IO.File.Exists(path))
    {
      return await System.IO.File.ReadAllTextAsync(path);
    }
    else
    {
      return "";
    }
  }
}
