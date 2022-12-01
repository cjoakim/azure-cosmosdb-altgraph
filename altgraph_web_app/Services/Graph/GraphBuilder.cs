using System;
using System.Collections.Generic;
using altgraph_web_app.Models;

namespace altgraph_web_app.Services.Graph
{
  public class GraphBuilder
  {
    public Entity? RootEntity { get; set; }  // the starting point of the graph
    public TripleQueryStruct? Struct { get; set; } // DB query results of the pertinent Triples
    public Graph Graph { get; set; } // the resulting graph given the above two
    public int structIterations { get; set; } = 0;

    public GraphBuilder(Entity rootEntity, TripleQueryStruct tripleQueryStruct)
    {
      RootEntity = rootEntity;
      Struct = tripleQueryStruct;
      Graph = new Graph();
      RootEntity.PopulateCacheKey();
    }

    public Graph BuildLibraryGraph(int maxIterations)
    {
      RootEntity.PopulateCacheKey();
      RootEntity.CalculateGraphKey();
      string rootKey = RootEntity.GraphKey;
      //log.warn("buildLibraryGraph, rootKey: " + rootKey);
      Graph.SetRootNode(rootKey);
      CollectLibraryGraph(maxIterations);  // iterate the triples and build the graph from them
      Graph.Finish();
      return Graph;
    }

    public Graph BuildAuthorGraph(Author author)
    {
      author.PopulateCacheKey();
      author.CalculateGraphKey();
      string authorLabel = author.Label;
      string rootKey = author.GraphKey;
      Graph.SetRootNode(author.GraphKey);
      //log.warn("buildAuthorGraph, author: " + authorLabel + ", rootKey: " + rootKey);

      for (int i = 0; i < Struct.Documents.Count; i++)
      {
        Triple t = Struct.Documents[i];
        List<string> tags = t.SubjectTags;
        for (int tidx = 0; tidx < tags.Count; tidx++)
        {
          string value = tags[tidx];
          if (value.StartsWith("author"))
          {
            if (value.Contains(authorLabel))
            {
              Graph.UpdateForAuthor(rootKey, t.SubjectKey, "author");
            }
          }
        }
      }
      Graph.Finish();
      return Graph;
    }

    private void CollectLibraryGraph(int maxIterations)
    {
      //log.warn("collectLibraryGraph, maxIterations: " + maxIterations);
      bool continueToCollect = true;
      int iterations = 0;
      int newNodesThisIteration = 0;

      while (continueToCollect)
      {
        iterations++;
        newNodesThisIteration = 0;
        List<string> currentKeys = Graph.GetCurrentKeys();
        for (int i = 0; i < Struct.Documents.Count; i++)
        {
          // match for subject key, add object key
          Triple t = Struct.Documents[i];
          if (t.SubjectType.Equals("library"))
          {
            if (t.ObjectType.Equals("library"))
            {
              string subjectKey = t.SubjectKey;
              if (currentKeys.Contains(subjectKey))
              {
                int changes = Graph.UpdateForLibrary(subjectKey, t.ObjectKey, t.Predicate);
                newNodesThisIteration = newNodesThisIteration + changes;
              }
            }
          }
        }
        // terminate the while-loop as necessary
        if (newNodesThisIteration < 1)
        {
          continueToCollect = false;
          //log.error("collect() terminating with no new nodes");
        }
        else
        {
          if (iterations >= maxIterations)
          {
            continueToCollect = false;  // possible infinite loop, eject!
            //log.error("collect() bailing out at maxIterations " + maxIterations);
          }
        }
      }
    }

    public string AsJson(bool pretty)
    {
      throw new NotImplementedException();
    }
  }
}