using System.Text.Json;
using altgraph_web_app.Models;
using altgraph_web_app.Services.Cache;
using altgraph_web_app.Services.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.CosmosRepository;

namespace altgraph_web_app.API
{
  [ApiController]
  [Route("[controller]")]
  public class GraphApiController : ControllerBase
  {
    private readonly ILogger<GraphApiController> _logger;
    private readonly IConfiguration _configuration;

    public GraphApiController(ILogger<GraphApiController> logger, IConfiguration configuration)
    {
      _logger = logger;
      _configuration = configuration;
    }
    // [HttpGet("NodesCsv")]
    // public async void NodesCsv()
    // {
    //   string csv = ReadCsv(_configuration["Paths:NodesCsvFile"]);
    //   HttpContext.Response.ContentType = "text/plain; charset=utf-8";
    //   HttpContext.Response.Headers.Add("Cache-Control", "max-age=0, must-revalidate, no-transform");
    //   await HttpContext.Response.WriteAsync(csv);
    // }
    // [HttpGet("EdgesCsv")]
    // public async void EdgesCsv()
    // {
    //   string csv = ReadCsv(_configuration["Paths:EdgesCsvFile"]);
    //   HttpContext.Response.ContentType = "text/plain; charset=utf-8";
    //   HttpContext.Response.Headers.Add("Cache-Control", "max-age=0, must-revalidate, no-transform");
    //   await HttpContext.Response.WriteAsync(csv);
    // }

    // [HttpGet("GetLibraryAsJson/{libraryName}")]
    // public async Task<string> GetLibraryAsJsonAsync(string libraryName)
    // {
    //   _logger.LogWarning($"getLibraryAsJson, libraryName: {libraryName}");

    //   Library library = await ReadLibraryAsync(libraryName, sessionId, false, cache, libraryRepository, configuration);
    //   if (library != null)
    //   {
    //     try
    //     {
    //       return JsonSerializer.Serialize<Library>(library);
    //     }
    //     catch (Exception e)
    //     {
    //       _logger.LogError(e.StackTrace);
    //       return "{}";
    //     }
    //   }
    //   else
    //   {
    //     return "{}";
    //   }
    // }

    // public async Task<Library> ReadLibraryAsync(string libName, bool useCache, Cache cache, LibraryRepository libraryRepository, IConfiguration configuration)
    // {
    //   Library? lib = null;

    //   if (useCache)
    //   {
    //     lib = await cache.GetLibraryAsync(libName);
    //     if (lib != null)
    //     {
    //       return lib;
    //     }
    //   }

    //   IEnumerable<Library> libraries = libraryRepository.FindByPkAndTenantAndDoctype(libName, configuration["Tenant"], "library").Result;
    //   foreach (Library library in libraries)
    //   {
    //     lib = library;
    //     lib.GraphKey = lib.CalculateGraphKey();
    //     try
    //     {
    //       await cache.PutLibraryAsync(lib);
    //     }
    //     catch (Exception ex)
    //     {

    //     }
    //   }
    //   return lib;
    // }

    // private string ReadCsv(string path)
    // {
    //   if (System.IO.File.Exists(path))
    //   {
    //     return System.IO.File.ReadAllText(path);
    //   }
    //   else
    //   {
    //     return "";
    //   }
    // }
  }
}