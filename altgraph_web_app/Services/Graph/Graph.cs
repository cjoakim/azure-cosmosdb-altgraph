using System;
using System.Collections.Generic;

namespace altgraph_web_app.Services.Graph
{
  public class Graph
  {
    public string? RootKey { get; set; }
    public Dictionary<string, GraphNode>? GraphMap { get; set; }

    public long StartTime { get; set; } = 0;
    public long EndTime { get; set; } = 0;
    public long ElapsedMs { get; set; } = 0;

    public Graph()
    {
      StartTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
      GraphMap = new Dictionary<string, GraphNode>();
    }

    public void SetRootNode(string key)
    {
      RootKey = key;
      GraphNode node = new GraphNode(true, key);
      GraphMap[key] = node;
    }

    public int UpdateForLibrary(string subjectKey, string objectKey, string predicate)
    {
      int changeCount = 0;
      GraphNode subjectNode = null;
      GraphNode objectNode = null;

      if (GraphMap.ContainsKey(objectKey))
      {
        objectNode = GraphMap[objectKey];
      }
      else
      {
        objectNode = CreateNewNode(objectKey);
        changeCount++;
      }

      subjectNode = GraphMap[subjectKey];
      int addAdjResult = subjectNode.AddAdjacent(objectNode, predicate);
      changeCount = changeCount + addAdjResult;

      return changeCount;
    }

    public int UpdateForAuthor(string subjectKey, string objectKey, string predicate)
    {
      int changeCount = 0;
      GraphNode subjectNode = null;
      GraphNode objectNode = null;

      if (GraphMap.ContainsKey(objectKey))
      {
        objectNode = GraphMap[objectKey];
      }
      else
      {
        objectNode = CreateNewNode(objectKey);
        changeCount++;
      }

      subjectNode = GraphMap[subjectKey];
      int addAdjResult = subjectNode.AddAdjacent(objectNode, predicate);
      changeCount = changeCount + addAdjResult;

      return changeCount;
    }

    public GraphNode CreateNewNode(string key)
    {
      GraphNode node = new GraphNode(false, key);
      GraphMap[key] = node;
      return node;
    }

    public List<string> GetCurrentKeys()
    {
      List<string> values = new List<string>();
      var objArray = GraphMap.Keys;
      // for (int i = 0; i < objArray.Count; i++)
      // {
      //   values.Add(objArray[i].ToString());
      // }
      foreach (string key in GraphMap.Keys)
      {
        values.Add(key);
      }
      return values;
    }

    public void AddNode(string key)
    {

    }

    public long Finish()
    {
      EndTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
      ElapsedMs = EndTime - StartTime;
      // log.warn("finish() elapsedMs: " + elapsedMs);
      return ElapsedMs;
    }
  }
}