using Microsoft.AspNetCore.Mvc;

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
    [HttpGet("NodesCsv")]
    public async void NodesCsv()
    {
      string csv = ReadCsv(_configuration["NodesCsvFile"]);
      HttpContext.Response.ContentType = "text/plain; charset=utf-8";
      HttpContext.Response.Headers.Add("Cache-Control", "max-age=0, must-revalidate, no-transform");
      await HttpContext.Response.WriteAsync(csv);
    }
    [HttpGet("EdgesCsv")]
    public async void EdgesCsv()
    {
      string csv = ReadCsv(_configuration["EdgesCsvFile"]);
      HttpContext.Response.ContentType = "text/plain; charset=utf-8";
      HttpContext.Response.Headers.Add("Cache-Control", "max-age=0, must-revalidate, no-transform");
      await HttpContext.Response.WriteAsync(csv);
    }

    private string ReadCsv(string path)
    {
      if (System.IO.File.Exists(path))
      {
        return System.IO.File.ReadAllText(path);
      }
      else
      {
        return "";
      }
    }
  }
}