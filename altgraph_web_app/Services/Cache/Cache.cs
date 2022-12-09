using altgraph_web_app.Models;
using altgraph_web_app.Services.Graph;
using StackExchange.Redis;
using System.Text;
using System.Text.Json;

namespace altgraph_web_app.Services.Cache
{
  public class Cache
  {
    public string CacheMethod { get; set; } = "";
    public bool UseRedis { get; set; } = false;

    private IConnectionMultiplexer _redis;
    private IDatabase _db;
    private readonly IConfiguration _configuration;
    private readonly ILogger<Cache> _logger;

    public Cache(IConnectionMultiplexer redis, IConfiguration configuration, ILogger<Cache> logger)
    {
      _redis = redis;
      _db = _redis.GetDatabase();
      _configuration = configuration;
      _logger = logger;
      CacheMethod = _configuration["AppCacheMethod"];
      if (CacheMethod == "redis")
      {
        UseRedis = true;
      }
    }

    public async Task<bool> PutLibraryAsync(Library lib)
    {
      if (UseRedis)
      {
        string key = "library|" + lib.Name;
        try
        {
          await _db.StringSetAsync(key, JsonSerializer.Serialize(lib));
          return true;
        }
        catch (Exception e)
        {
          _logger.LogError(e, $"Error putting library {lib} in cache");
          return false;
        }
      }
      else
      {
        string filename = string.Format(_configuration["Paths:LibraryCacheFileTemplate"], lib.Name);
        try
        {
          Directory.CreateDirectory(Path.GetDirectoryName(filename));
          await File.WriteAllTextAsync(filename, JsonSerializer.Serialize<Library>(lib));
          return true;
        }
        catch (Exception ex)
        {
          _logger.LogError(ex, $"Error putting library {lib} in cache");
          return false;
        }
      }
    }

    public async Task<Library>? GetLibraryAsync(string libName)
    {
      if (UseRedis)
      {
        string key = "library|" + libName;
        string? json = await _db.StringGetAsync(key);
        if (json == null)
        {
          return null;
        }
        else
        {
          try
          {
            return JsonSerializer.Deserialize<Library>(json);
          }
          catch (Exception ex)
          {
            _logger.LogError(ex, $"Error getting library {libName} from cache");
            return null;
          }
        }
      }
      else
      {
        try
        {
          string cacheFilename = string.Format(_configuration["Paths:LibraryCacheFileTemplate"], libName);
          var json = await File.ReadAllTextAsync(cacheFilename);
          return JsonSerializer.Deserialize<Library>(json);
        }
        catch (Exception ex)
        {
          _logger.LogError(ex, $"Error getting library {libName} from cache");
          return null;
        }
      }
    }

    public async Task<bool> PutTriplesAsync(TripleQueryStruct tripleQueryStruct)
    {
      if (UseRedis)
      {
        string key = "triples";
        try
        {
          await _db.StringSetAsync(key, JsonSerializer.Serialize(tripleQueryStruct));
          return true;
        }
        catch (Exception e)
        {
          _logger.LogError(e, $"Error putting triples in cache");
          return false;
        }
      }
      else
      {
        string filename = _configuration["Paths:TripleQueryStructCacheFile"];
        try
        {
          Directory.CreateDirectory(Path.GetDirectoryName(filename));
          await File.WriteAllTextAsync(filename, JsonSerializer.Serialize<TripleQueryStruct>(tripleQueryStruct));
          return true;
        }
        catch (Exception ex)
        {
          _logger.LogError(ex, $"Error putting triples in cache");
          return false;
        }
      }
    }

    public async Task<TripleQueryStruct> GetTriplesAsync()
    {
      if (UseRedis)
      {
        string key = "triples";
        string? json = await _db.StringGetAsync(key);
        if (json == null)
        {
          return null;
        }
        else
        {
          try
          {
            return JsonSerializer.Deserialize<TripleQueryStruct>(json);
          }
          catch (Exception ex)
          {
            _logger.LogError(ex, $"Error getting triples from cache");
            return null;
          }
        }
      }
      else
      {
        try
        {
          string cacheFilename = _configuration["Paths:TripleQueryStructCacheFile"];
          var json = await File.ReadAllTextAsync(cacheFilename);
          return JsonSerializer.Deserialize<TripleQueryStruct>(json);
        }
        catch (Exception ex)
        {
          _logger.LogError(ex, $"Error getting triples from cache");
          return null;
        }
      }
    }
  }
}