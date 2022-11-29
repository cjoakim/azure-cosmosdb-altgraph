using altgraph_web_app.Models;

namespace altgraph_web_app.Services.Graph
{
  public class TripleQueryStruct
  {

    public String StructType { get; set; } = typeof(TripleQueryStruct).Name;
    public String ContainerName { get; set; }
    public String Sql { get; set; }
    public long StartTime { get; set; }// = System.DateTime.Now.currentTimeMillis();
    public long EndTime { get; set; }
    public long ElapsedMs { get; set; }
    public long PageCount { get; set; }
    public double RequestCharge { get; set; }

    public long DocumentCount { get; set; }
    public List<Triple> Documents { get; set; } = new List<Triple>();

    public void Reset(bool newList)
    {
      if (newList)
      {
        Documents = new List<Triple>();
      }
      //startTime = System.currentTimeMillis();
    }

    public void IncrementPageCount()
    {
      PageCount++;
    }

    public void IncrementRuCharge(double ru)
    {
      RequestCharge = RequestCharge + ru;
    }

    public void AddDocument(Triple t)
    {
      Documents.Add(t);
    }

    public long Start()
    {
      throw new NotImplementedException();
      // startTime = System.currentTimeMillis();
      // return startTime;
    }
    public long Stop()
    {
      throw new NotImplementedException();
      // endTime = System.currentTimeMillis();
      // elapsedMs = endTime - startTime;
      // documentCount = documents.size();
      // return elapsedMs;
    }

    public Triple Find(String id, String pk, String tenant)
    {

      for (int i = 0; i < Documents.Count; i++)
      {
        Triple t = Documents[i];
        if (t.Id.Equals(id))
        {

          if (t.Tenant.Equals(tenant))
          {
            return t;
          }
        }
      }
      return null;
    }

    public String AsJson(bool pretty)
    {

      throw new NotImplementedException();
    }
  }
}