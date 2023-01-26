using altgraph_shared_app.Options;
using altgraph_shared_app.Services.Cache;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Azure.CosmosRepository;
using Microsoft.Extensions.Options;
using altgraph_shared_app.Repositories.Imdb;
using altgraph_shared_app.Models.Imdb;
using altgraph_shared_app.Services.Graph.v2;
using altgraph_shared_app.Services.Graph.v2.Structs;
using altgraph_shared_app.Util;
using altgraph_web_app.Models;
using System.ComponentModel.DataAnnotations;

namespace altgraph_web_app.Areas.ImdbGraph.Pages;

public class IndexModel : PageModel
{
  private readonly ILogger<IndexModel> _logger;
  [BindProperty(SupportsGet = true)]
  public FormFunctionEnum FormFunction { get; set; } = FormFunctionEnum.GraphStats;
  [BindProperty(SupportsGet = true)]
  public string Value1 { get; set; } = string.Empty;
  [BindProperty(SupportsGet = true)]
  public string? Value2 { get; set; } = string.Empty;
  [BindProperty(SupportsGet = true)]
  public string? ElapsedMs { get; set; } = string.Empty;
  [BindProperty(SupportsGet = true)]
  public string? EdgesStruct { get; set; } = string.Empty;
  [BindProperty(SupportsGet = true)]
  public long? GraphLoadingMaxProgress { get; set; } = 0;
  private long? graphLoadingProgress { get; set; } = 0;
  private IJGraph _jGraph;
  private readonly MovieRepository _movieRepository;
  private readonly PersonRepository _personRepository;
  private readonly ICache _cache;
  private readonly CacheOptions _cacheOptions;
  private readonly NpmPathsOptions _pathsOptions;
  private readonly ImdbOptions _imdbOptions;
  private readonly IMemoryStats _memoryStats;

  public IndexModel(ILogger<IndexModel> logger, IRepository<Movie> movieRepository, IRepository<Person> personRepository, ICache cache, IOptions<CacheOptions> cacheOptions, IOptions<NpmPathsOptions> pathsOptions, IOptions<ImdbOptions> imdbOptions, IJGraph jgraph, IMemoryStats memoryStats)
  {
    _logger = logger;
    _movieRepository = new MovieRepository(movieRepository);
    _personRepository = new PersonRepository(personRepository);
    _cache = cache;
    _logger = logger;
    _cacheOptions = cacheOptions.Value;
    _pathsOptions = pathsOptions.Value;
    _imdbOptions = imdbOptions.Value;
    _jGraph = jgraph;
    _jGraph.JGraphStartedLoadingProgress += JGraphStartedLoadingProgress;
    _jGraph.JGraphLoadingProgress += JGraphLoadingProgress;
    _jGraph.JGraphFinishedLoadingProgress += JGraphFinishedLoadingProgress;
    _memoryStats = memoryStats;

    int[] counts = _jGraph.GetVertexAndEdgeCounts();
    _logger.LogDebug($"jgraph vertices: {counts[0]}");
    _logger.LogDebug($"jgraph edges:    {counts[1]}");
  }

  public async Task<JsonResult> OnGetGraphStatsAsync(string flag)
  {
    _logger.LogDebug($"OnGetGraphStats");
    long startMs = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

    if (flag.Equals("reload", StringComparison.OrdinalIgnoreCase))
    {
      await _jGraph.RefreshAsync();
    }

    GraphStats graphStatsStruct = new GraphStats();
    int[] counts = _jGraph.GetVertexAndEdgeCounts();
    graphStatsStruct.VertexCount = counts[0];
    graphStatsStruct.EdgeCount = counts[1];
    graphStatsStruct.Epoch = _memoryStats.Epoch;
    graphStatsStruct.TotalMb = _memoryStats.TotalMb;
    graphStatsStruct.FreeMb = _memoryStats.FreeMb;
    graphStatsStruct.MaxMb = _memoryStats.MaxMb;
    graphStatsStruct.PctFree = _memoryStats.PctFree;
    graphStatsStruct.ElapsedMs = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - startMs;
    graphStatsStruct.RefreshDate = _jGraph.RefreshDate;
    graphStatsStruct.RefreshMs = _jGraph.RefreshMs;
    graphStatsStruct.RefreshSource = _jGraph.Source;

    return new JsonResult(graphStatsStruct);
  }

  public async Task<JsonResult?> OnGetImdbVertexAsync(string imdbConst)
  {
    _logger.LogDebug($"OnGetImdbVertexAsync, imdbConst: {imdbConst}");

    try
    {
      if (imdbConst.StartsWith("tt"))
      {
        Movie? m = await LookupMovieAsync(imdbConst);
        if (m != null)
        {
          return new JsonResult(m);
        }
      }
      else if (imdbConst.StartsWith("nm"))
      {
        Person? p = await LookupPersonAsync(imdbConst);
        if (p != null)
        {
          return new JsonResult(p);
        }
      }
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, ex.Message);
    }
    return null;
  }

  private async Task<Movie?> LookupMovieAsync(String imdbConst)
  {
    foreach (Movie movie in await _movieRepository.FindByIdAndPkAsync(imdbConst, imdbConst))
    {
      return movie;
    }

    return null;
  }

  private async Task<Person?> LookupPersonAsync(String imdbConst)
  {
    foreach (Person person in await _personRepository.FindByIdAndPkAsync(imdbConst, imdbConst))
    {
      return person;
    }

    return null;
  }

  public async Task<IActionResult> OnPostAsync()
  {
    ViewData["Method"] = "POST";
    if (!ModelState.IsValid)
    {
      return Page();
    }

    DateTime start = DateTime.Now;

    TranslateShortcutValues();

    _logger.LogDebug($"formObject, getFormFunction:     {FormFunction}");
    _logger.LogDebug($"formObject, getValue1:           {Value1}");
    _logger.LogDebug($"formObject, getValue2:           {Value2}");
    _logger.LogDebug($"formObject, getSessionId (form): {HttpContext.Session.Id}");

    bool directed = false;

    try
    {
      switch (FormFunction)
      {
        case FormFunctionEnum.GraphStats:
          break;
        case FormFunctionEnum.PageRank:
          if (!IsValue1AnInteger())
          {
            Value1 = "100";
          }
          directed = true;
          break;
        case FormFunctionEnum.Network:
          if (!IsValue2AnInteger())
          {
            Value2 = "1";
          }
          break;
        case FormFunctionEnum.ShortestPath:
          break;
        default:
          throw new NotImplementedException(FormFunction.ToString());
      }
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, ex.Message);
    }

    if (_jGraph.Graph == null)
    {
      await _jGraph.RefreshAsync(directed);
    }

    DateTime end = DateTime.Now;

    ElapsedMs = $"{Math.Round((end - start).TotalMilliseconds)} ms";

    return Page();
  }

  public JsonResult? OnGetStarNetwork(string vertex, string degree)
  {
    _logger.LogDebug($"OnGetStarNetwork, vertex: {vertex}, degree: {degree}");

    if (degree != null)
    {
      JStarNetwork? star = _jGraph.StarNetworkFor(vertex, int.Parse(degree));
      if (star != null)
      {
        return new JsonResult(star.AsEdgesStruct());
      }
    }
    return null;
  }

  public JsonResult? OnGetShortestPath(string v1, string v2)
  {
    _logger.LogDebug($"OnGetShortestPath, v1: {v1}, v2: {v2}");

    EdgesStruct? edgesStruct =
                _jGraph.GetShortestPathAsEdgesStruct(v1, v2);

    if (edgesStruct != null)
    {
      return new JsonResult(edgesStruct);
    }
    return null;
  }

  public JsonResult? OnGetPageRanks(string count)
  {
    _logger.LogDebug($"OnGetPageRanks, count: {count}");

    if (count != null)
    {
      List<JRank>? ranks = _jGraph.SortedPageRanks(int.Parse(count));

      return new JsonResult(ranks);
    }
    return null;
  }

  public JsonResult? OnGetProgress()
  {
    _logger.LogDebug($"OnGetProgress, getSessionId (form): {HttpContext.Session.Id}");

    return new JsonResult(graphLoadingProgress);
  }

  public void TranslateShortcutValues()
  {
    if (Value1.Equals("kb", StringComparison.InvariantCultureIgnoreCase))
    {
      Value1 = ImdbConstants.PERSON_KEVIN_BACON;
    }
    if (Value1.Equals("cr", StringComparison.InvariantCultureIgnoreCase))
    {
      Value1 = ImdbConstants.PERSON_CHARLOTTE_RAMPLING;
    }
    if (Value1.Equals("jr", StringComparison.InvariantCultureIgnoreCase))
    {
      Value1 = ImdbConstants.PERSON_JULIA_ROBERTS;
    }
    if (Value1.Equals("jl", StringComparison.InvariantCultureIgnoreCase))
    {
      Value1 = ImdbConstants.PERSON_JENNIFER_LAWRENCE;
    }
    if (Value1.Equals("fl", StringComparison.InvariantCultureIgnoreCase))
    {
      Value1 = ImdbConstants.MOVIE_FOOTLOOSE;
    }

    if (Value2 != null)
    {
      if (Value2.Equals("kb", StringComparison.InvariantCultureIgnoreCase))
      {
        Value2 = ImdbConstants.PERSON_KEVIN_BACON;
      }
      if (Value2.Equals("cr", StringComparison.InvariantCultureIgnoreCase))
      {
        Value2 = ImdbConstants.PERSON_CHARLOTTE_RAMPLING;
      }
      if (Value2.Equals("jr", StringComparison.InvariantCultureIgnoreCase))
      {
        Value2 = ImdbConstants.PERSON_JULIA_ROBERTS;
      }
      if (Value2.Equals("jl", StringComparison.InvariantCultureIgnoreCase))
      {
        Value2 = ImdbConstants.PERSON_JENNIFER_LAWRENCE;
      }
      if (Value2.Equals("fl", StringComparison.InvariantCultureIgnoreCase))
      {
        Value2 = ImdbConstants.MOVIE_FOOTLOOSE;
      }
    }
  }

  public bool IsValue1AnInteger()
  {
    try
    {
      int.Parse(Value1);
      return true;
    }
    catch (FormatException)
    {
      return false;
    }
  }

  public bool IsValue2AnInteger()
  {
    if (Value2 == null)
    {
      return false;
    }
    try
    {
      int.Parse(Value2);
      return true;
    }
    catch (FormatException)
    {
      return false;
    }
  }

  public String ReloadFlag()
  {
    if (Value1.ToLower().Contains("reload"))
    {
      return "reload";
    }
    return "no";
  }

  private void JGraphFinishedLoadingProgress(object? sender, JGraphFinishedLoadingProgressEventArgs e)
  {
    graphLoadingProgress = e.Count;
  }

  private void JGraphLoadingProgress(object? sender, JGraphLoadingProgressEventArgs e)
  {
    graphLoadingProgress += e.Progress;
  }

  private void JGraphStartedLoadingProgress(object? sender, JGraphStartedLoadingProgressEventArgs e)
  {
    graphLoadingProgress = 0;
    GraphLoadingMaxProgress = e.MaxCount;
  }
}
