using altgraph_web_app.Models;
using altgraph_web_app.Services.Cache;
using altgraph_web_app.Services.Graph;
using altgraph_web_app.Services.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Azure.CosmosRepository;
using System.IO;

namespace altgraph_web_app.Pages;

public class GraphModel : PageModel
{
  private readonly ILogger<GraphModel> _logger;
  private static int DEFAULT_DEPTH = 1;
  [BindProperty]
  public string SubjectName { get; set; }
  [BindProperty]
  public bool AuthorCheckBox { get; set; } = false;
  [BindProperty]
  public string? GraphDepth { get; set; } = "1";
  [BindProperty]
  public string? CacheOpts { get; set; }
  [BindProperty]
  public string? ElapsedMs { get; set; }
  [BindProperty]
  public string? SessionId { get; set; }
  [BindProperty(SupportsGet = true)]
  public string? NodesCsv { get; set; }
  [BindProperty(SupportsGet = true)]
  public string? EdgesCsv { get; set; }

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
      //log.error("non-integer depth value: " + graphDepth + ".  returning the default");
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
        await HandleAuthorSearch();
      }
      else
      {
        await HandleLibrarySearch();
      }
    }
    catch (Exception ex)
    {

    }

    return RedirectToPage("Graph");
  }

  private async Task HandleAuthorSearch()
  {
    try
    {
      string libName = SubjectName;
      Author author = ReadAuthorByLabel(libName, SessionId, UseCachedLibrary(CacheOpts));
      if (author != null)
      {
        TripleQueryStruct tripleQueryStruct = ReadTriples(UseCachedTriples(CacheOpts), SessionId);
        GraphBuilder graphBuilder = new GraphBuilder(author, tripleQueryStruct);
        Graph graph = graphBuilder.BuildAuthorGraph(author);
        D3CsvBuilder d3CsvBuilder = new D3CsvBuilder(graph, _configuration);
        d3CsvBuilder.BuildBillOfMaterialCsv(SessionId, GetDepthAsInt());
        d3CsvBuilder.Finish();
      }
    }
    catch (Exception ex)
    {

    }
  }

  private async Task HandleLibrarySearch()
  {
    try
    {
      string libName = SubjectName;
      Library library = ReadLibrary(libName, SessionId, UseCachedLibrary(CacheOpts));
      if (library != null)
      {
        TripleQueryStruct tripleQueryStruct = ReadTriples(UseCachedTriples(CacheOpts), SessionId);
        GraphBuilder graphBuilder = new GraphBuilder(library, tripleQueryStruct);
        Graph graph = graphBuilder.BuildLibraryGraph(GetDepthAsInt());
        D3CsvBuilder d3CsvBuilder = new D3CsvBuilder(graph, _configuration);
        d3CsvBuilder.BuildBillOfMaterialCsv(SessionId, GetDepthAsInt());
        d3CsvBuilder.Finish();
      }
    }
    catch (Exception ex)
    {

    }
  }

  private Library ReadLibrary(string libName, string sessionId, bool useCache)
  {
    Library lib = null;

    if (useCache)
    {
      lib = _cache.GetLibrary(libName);
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
        _cache.PutLibrary(lib);
      }
      catch (Exception ex)
      {

      }
    }
    return lib;
  }

  private Author ReadAuthorByLabel(string label, string sessionId, bool useCache)
  {
    Author author = null;
    IEnumerable<Author> authors = _authorRepository.FindByLabel(label).Result;
    foreach (Author a in authors)
    {
      author = a;
    }
    return author;
  }

  private TripleQueryStruct ReadTriples(bool useCache, string sessionId)
  {
    string cacheFilename = GetTripleQueryStructCacheFilename(sessionId);
    if (useCache)
    {
      TripleQueryStruct returnValue = _cache.GetTriples();
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
      _cache.PutTriples(tripleQueryStruct);
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

  private string GetLibraryCacheFilename(string libName, string sessionId)
  {
    return "data/cache/" + libName + ".json";
  }

  private string GetTripleQueryStructCacheFilename(string sessionId)
  {
    return "data/cache/TripleQueryStruct.json";
  }
}
