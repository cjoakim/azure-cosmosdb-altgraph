using altgraph_shared_app.Models;
using Microsoft.Azure.CosmosRepository;

namespace altgraph_shared_app.Services.Repositories
{
  public class MaintainerRepository
  {
    public IRepository<Maintainer> Maintainers { get; private set; }
    public MaintainerRepository(IRepository<Maintainer> maintainers)
    {
      Maintainers = maintainers;
    }
  }
}