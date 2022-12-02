using altgraph_web_app.Models;
using altgraph_web_app.Services.Graph;
using altgraph_web_app.Services.IO;
using StackExchange.Redis;
using System.Text.Json;

namespace altgraph_web_app.Services.Cache
{
  public class Cache
  {
    public string CacheMethod { get; set; }
    public bool UseRedis { get; set; }

    private IConnectionMultiplexer _redis;
    private IDatabase _db;

    private FileUtil _fileUtil = new FileUtil();

    public Cache(IConnectionMultiplexer redis)
    {
      _redis = redis;
      _db = _redis.GetDatabase();
    }

    public bool PutLibrary(Library lib)
    {
      if (UseRedis)
      {
        string key = "library|" + lib.Name;
        try
        {
          _db.StringSet(key, JsonSerializer.Serialize(lib));
          return true;
        }
        catch (Exception e)
        {
          return false;
        }
      }
      else
      {
        string filename = GetLibraryCacheFilename(lib.Name);
        try
        {
          _fileUtil.WriteJson(lib, filename, true, true);
          return true;
        }
        catch (Exception ex)
        {
          return false;
        }
      }
    }

    public Library GetLibrary(string libName)
    {
      if (UseRedis)
      {
        string key = "library|" + libName;
        string json = _db.StringGet(key);
        try
        {
          return JsonSerializer.Deserialize<Library>(json);
        }
        catch (Exception ex)
        {
          return null;
        }
      }
      else
      {
        try
        {
          string cacheFilename = GetLibraryCacheFilename(libName);
          return _fileUtil.ReadLibrary(cacheFilename);
        }
        catch (Exception ex)
        {
          return null;
        }
      }
    }

    public bool PutTriples(TripleQueryStruct tripleQueryStruct)
    {
      if (UseRedis)
      {
        string key = "triples";
        try
        {
          _db.StringSet(key, JsonSerializer.Serialize(tripleQueryStruct));
          return true;
        }
        catch (Exception e)
        {
          return false;
        }
      }
      else
      {
        string filename = GetTripleQueryStructCacheFilename();
        try
        {
          _fileUtil.WriteJson(tripleQueryStruct, filename, true, true);
          return true;
        }
        catch (Exception ex)
        {
          return false;
        }
      }
    }

    public TripleQueryStruct GetTriples()
    {
      if (UseRedis)
      {
        string key = "triples";
        string json = _db.StringGet(key);
        try
        {
          return JsonSerializer.Deserialize<TripleQueryStruct>(json);
        }
        catch (Exception ex)
        {
          return null;
        }
      }
      else
      {
        try
        {
          string cacheFilename = GetTripleQueryStructCacheFilename();
          return _fileUtil.ReadTripleQueryStruct(cacheFilename);
        }
        catch (Exception ex)
        {
          return null;
        }
      }
    }

    private string GetLibraryCacheFilename(string libName)
    {
      return "data/cache/" + libName + ".json";
    }

    private string GetTripleQueryStructCacheFilename()
    {
      return "data/cache/TripleQueryStruct.json";
    }
  }
}