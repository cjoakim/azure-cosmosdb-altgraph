using System.Linq.Expressions;
using altgraph_shared_app.Models;
using altgraph_shared_app.Models.Npm;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.CosmosRepository;
using Microsoft.Azure.CosmosRepository.Builders;
using Microsoft.Azure.CosmosRepository.Paging;

namespace altgraph_shared_app.Services.Repositories.Npm
{
  public class AuthorRepository
  {
    public IRepository<Author> Authors { get; private set; }
    public AuthorRepository(IRepository<Author> authors)
    {
      Authors = authors;
    }
    public async Task<IEnumerable<Author>> FindByLabelAsync(string label)
    {
      return await Authors.GetAsync(x => x.Label == label);
    }
  }
}