using altgraph_shared_app.Models.Imdb;
using Microsoft.Azure.CosmosRepository;

namespace altgraph_shared_app.Repositories.Imdb
{
  public class PersonRepository
  {
    public IRepository<Person> Persons { get; private set; }
    public PersonRepository(IRepository<Person> persons)
    {
      Persons = persons;
    }

    public async Task<IEnumerable<Person>> FindByIdAndPkAsync(string id, string pk)
    {
      return await Persons.GetAsync(x => x.Id == id && x.Pk == pk);
    }
  }
}
