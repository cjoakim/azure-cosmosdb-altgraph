using altgraph_web_app.Models;
using Microsoft.Azure.CosmosRepository;

namespace altgraph_web_app.Services.Repositories
{
  public class MaintainerRepository
  {
    public IRepository<Maintainer>? Maintainers { get; set; }
    public MaintainerRepository(IRepository<Maintainer> maintainers)
    {
      Maintainers = maintainers;
    }
  }
}