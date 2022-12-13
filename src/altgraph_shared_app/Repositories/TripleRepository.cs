using altgraph_shared_app.Models;
using Microsoft.Azure.CosmosRepository;

namespace altgraph_shared_app.Services.Repositories
{
  public class TripleRepository
  {
    public IRepository<Triple> Triples { get; set; }
    public TripleRepository(IRepository<Triple> triples)
    {
      Triples = triples;
    }
    public async Task<long> CountAllDocumentsAsync()
    {
      return await Triples.CountAsync();
    }

    public async Task<long> CountAllTriplesAsync()
    {
      return await Triples.CountAsync(x => x.Doctype == "triple");
    }

    public async Task<long> CountAllLibrariesAsync()
    {
      return await Triples.CountAsync(x => x.Doctype == "library");
    }

    public async Task<long> CountAllAuthorsAsync()
    {
      return await Triples.CountAsync(x => x.Doctype == "author");
    }

    public async Task<long> CountAllMaintainersAsync()
    {
      return await Triples.CountAsync(x => x.Doctype == "maintainer");
    }

    public async Task<long> GetNumberOfDocsWithSubjectLabelAsync(string subjectLabel)
    {
      return await Triples.CountAsync(x => x.SubjectLabel == subjectLabel);
    }

    public async Task<IEnumerable<Triple>> GetByDoctypeAsync(string doctype)
    {
      return await Triples.GetAsync(x => x.Doctype == doctype);
    }

    public async Task<IEnumerable<Triple>> GetByPkLobAndSubjectsAsync(string pk,      // "pk": "triple|123"
            string lob,
            string subjectType,
            string objectType)
    {
      return await Triples.GetAsync(x => x.Pk == pk && x.Lob == lob && x.SubjectType == subjectType && x.ObjectType == objectType);
    }

    public async Task<IEnumerable<Triple>> GetByTenantAndSubjectLabelsAsync(
            string tenant,
             List<String> subjectLabels)
    {
      return await Triples.GetAsync(x => x.Tenant == tenant && subjectLabels.Contains(x.SubjectLabel));
    }

    public async Task<IEnumerable<Triple>> GetByPkTenantAndSubjectTypesAsync(
            string pk,
            string tenant,
            List<String> subjectTypes)
    {
      return await Triples.GetAsync(x => x.Pk == pk && x.Tenant == tenant && subjectTypes.Contains(x.SubjectType));
    }

    public async Task<IEnumerable<Triple>> FindByTenantAndLobAndSubjectLabelsInAsync(string tenant, string lob, List<string> values)
    {
      string pk = "triple|" + tenant;
      return await Triples.GetAsync(x => x.Pk == pk && values.Contains(x.SubjectLabel));
    }
  }
}