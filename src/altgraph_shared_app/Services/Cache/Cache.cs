using altgraph_shared_app.Options;
using altgraph_shared_app.Services.Graph.v1;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System.Text.Json;
using altgraph_shared_app.Models.Npm;

namespace altgraph_shared_app.Services.Cache
{


  public class Cache : ICache
  {
    public string CacheMethod { get; set; } = string.Empty;
    public bool UseRedis { get; set; } = false;
    private IConnectionMultiplexer _redis;
    private readonly IDatabase _db;
    private readonly CacheOptions _cacheOptions;
    private readonly PathsOptions _pathsOptions;
    private readonly ILogger<Cache> _logger;

    public Cache(IConnectionMultiplexer redis, IOptions<CacheOptions> cacheOptions, IOptions<PathsOptions> pathsOptions, ILogger<Cache> logger)
    {
      _redis = redis;
      _db = _redis.GetDatabase();
      _cacheOptions = cacheOptions.Value;
      _pathsOptions = pathsOptions.Value;
      _logger = logger;
      CacheMethod = _cacheOptions.AppCacheMethod;
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
        string filename = string.Format(_pathsOptions.LibraryCacheFileTemplate, lib.Name);
        try
        {
          string? directoryName = Path.GetDirectoryName(filename);

          if (directoryName != null)
          {
            Directory.CreateDirectory(directoryName);
            await File.WriteAllTextAsync(filename, JsonSerializer.Serialize(lib));
          }

          return true;
        }
        catch (Exception ex)
        {
          _logger.LogError(ex, $"Error putting library {lib} in cache");
          return false;
        }
      }
    }

    public async Task<Library?> GetLibraryAsync(string libName)
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
          string cacheFilename = string.Format(_pathsOptions.LibraryCacheFileTemplate, libName);
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
        string filename = _pathsOptions.TripleQueryStructCacheFile;
        try
        {
          string? directoryName = Path.GetDirectoryName(filename);
          if (directoryName != null)
          {
            Directory.CreateDirectory(directoryName);
            await File.WriteAllTextAsync(filename, JsonSerializer.Serialize(tripleQueryStruct));
          }
          return true;
        }
        catch (Exception ex)
        {
          _logger.LogError(ex, $"Error putting triples in cache");
          return false;
        }
      }
    }

    public async Task<TripleQueryStruct?> GetTriplesAsync()
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
          var json = await File.ReadAllTextAsync(_pathsOptions.TripleQueryStructCacheFile);
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