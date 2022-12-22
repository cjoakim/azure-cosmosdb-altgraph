using System;
using System.Collections.Generic;
using altgraph_shared_app.Models;
using altgraph_shared_app.Models.Npm;

namespace altgraph_shared_app.Services.Graph
{
  public class TripleQueryStruct
  {
    public string StructType { get; set; } = typeof(TripleQueryStruct).Name;
    public string ContainerName { get; set; } = string.Empty;
    public string Sql { get; set; } = string.Empty;
    public long StartTime { get; set; } = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
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
      StartTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
    }

    public void IncrementPageCount()
    {
      PageCount++;
    }

    public void IncrementRuCharge(double ru)
    {
      RequestCharge += ru;
    }

    public void AddDocument(Triple t)
    {
      Documents.Add(t);
    }

    public long Start()
    {
      StartTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
      return StartTime;
    }
    public long Stop()
    {
      EndTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
      ElapsedMs = EndTime - StartTime;
      DocumentCount = Documents.Count;
      return ElapsedMs;
    }

    public Triple? Find(string id, string tenant)
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
  }
}