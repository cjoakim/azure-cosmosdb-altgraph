using altgraph_shared_app.Models.Npm;
using altgraph_shared_app.Services.Graph.v1;

namespace altgraph_shared_app.Services.Cache
{
  public interface ICache
  {
    string CacheMethod { get; set; }
    bool UseRedis { get; set; }

    Task<Library?> GetLibraryAsync(string libName);
    Task<TripleQueryStruct?> GetTriplesAsync();
    Task<bool> PutLibraryAsync(Library lib);
    Task<bool> PutTriplesAsync(TripleQueryStruct tripleQueryStruct);
  }
}