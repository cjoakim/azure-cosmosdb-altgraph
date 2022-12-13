using altgraph_shared_app.Models;
using Microsoft.Azure.CosmosRepository;

namespace altgraph_shared_app.Services.Repositories
{
  public class LibraryRepository
  {
    IRepository<Library> Libraries { get; set; }
    public LibraryRepository(IRepository<Library> libraries)
    {
      Libraries = libraries;
    }
    public async Task<IEnumerable<Library>> FindByPkAndTenantAsync(string pk, string tenant)
    {
      return await Libraries.GetAsync(x => x.Pk == pk && x.Tenant == tenant);
    }

    public async Task<IEnumerable<Library>> FindByPkAndTenantAndDoctypeAsync(string pk, string tenant, string doctype)
    {
      return await Libraries.GetAsync(x => x.Pk == pk && x.Tenant == tenant && x.Doctype == doctype);
    }
  }
}