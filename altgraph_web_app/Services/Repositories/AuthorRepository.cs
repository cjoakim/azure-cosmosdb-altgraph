using System.Linq.Expressions;
using altgraph_web_app.Models;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.CosmosRepository;
using Microsoft.Azure.CosmosRepository.Builders;
using Microsoft.Azure.CosmosRepository.Paging;

namespace altgraph_web_app.Services.Repositories
{
  public class AuthorRepository
  {
    public IRepository<Author>? Authors { get; set; }

  }
}