using altgraph_web_app.Models;
using Microsoft.Azure.CosmosRepository;

namespace altgraph_web_app.Services.Repositories
{
  public class TripleRepository
  {
    public IRepository<Triple> Triples { get; set; }
    public TripleRepository(IRepository<Triple> triples)
    {
      Triples = triples;
    }
    public async Task<long> CountAllDocuments()
    {
      return await Triples.CountAsync();
    }

    public async Task<long> CountAllTriples()
    {
      return await Triples.CountAsync(x => x.Doctype == "triple");
    }

    public async Task<long> CountAllLibraries()
    {
      return await Triples.CountAsync(x => x.Doctype == "library");
    }

    public async Task<long> CountAllAuthors()
    {
      return await Triples.CountAsync(x => x.Doctype == "author");
    }

    public async Task<long> CountAllMaintainers()
    {
      return await Triples.CountAsync(x => x.Doctype == "maintainer");
    }

    public async Task<long> GetNumberOfDocsWithSubjectLabel(string subjectLabel)
    {
      return await Triples.CountAsync(x => x.SubjectLabel == subjectLabel);
    }

    public async Task<IEnumerable<Triple>> GetByDoctype(string doctype)
    {
      return await Triples.GetAsync(x => x.Doctype == doctype);
    }

    public async Task<IEnumerable<Triple>> GetByPkLobAndSubjects(string pk,      // "pk": "triple|123"
            string lob,
            string subjectType,
            string objectType)
    {
      return await Triples.GetAsync(x => x.Pk == pk && x.Lob == lob && x.SubjectType == subjectType && x.ObjectType == objectType);
    }


    public async Task<IEnumerable<Triple>> GetByTenantAndSubjectLabels(
            string tenant,
             List<String> subjectLabels)
    {
      return await Triples.GetAsync(x => x.Tenant == tenant && subjectLabels.Contains(x.SubjectLabel));
    }

    public async Task<IEnumerable<Triple>> GetByPkTenantAndSubjectTypes(
            string pk,
            string tenant,
            List<String> subjectTypes)
    {
      return await Triples.GetAsync(x => x.Pk == pk && x.Tenant == tenant && subjectTypes.Contains(x.SubjectType));
    }

    public async Task<IEnumerable<Triple>> FindByTenantAndLobAndSubjectLabelsIn(string tenant, string lob, List<string> values)
    {
      string pk = "triple|" + tenant;
      return await Triples.GetAsync(x => x.Pk == pk && values.Contains(x.SubjectLabel));
    }
  }
}