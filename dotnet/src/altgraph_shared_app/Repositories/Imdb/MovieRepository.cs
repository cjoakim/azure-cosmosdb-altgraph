using altgraph_shared_app.Models.Imdb;
using Microsoft.Azure.CosmosRepository;

namespace altgraph_shared_app.Repositories.Imdb
{
  public class MovieRepository
  {
    public IRepository<Movie> Movies { get; private set; }
    public MovieRepository(IRepository<Movie> movies)
    {
      Movies = movies;
    }
    public async Task<IEnumerable<Movie>> FindByIdAndPkAsync(string id, string pk)
    {
      return await Movies.GetAsync(x => x.Id == id && x.Pk == pk);
    }
  }
}
