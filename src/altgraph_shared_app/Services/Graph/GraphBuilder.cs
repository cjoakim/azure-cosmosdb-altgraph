using System;
using System.Collections.Generic;
using altgraph_shared_app.Models;
using Microsoft.Extensions.Logging;

namespace altgraph_shared_app.Services.Graph
{
  public class GraphBuilder
  {
    public NpmDocument RootEntity { get; set; }  // the starting point of the graph
    public TripleQueryStruct Struct { get; set; } // DB query results of the pertinent Triples
    public Graph Graph { get; set; } // the resulting graph given the above two
    public int structIterations { get; set; }
    private readonly ILogger _logger;

    public GraphBuilder(NpmDocument rootEntity, TripleQueryStruct tripleQueryStruct, ILogger logger)
    {
      _logger = logger;
      RootEntity = rootEntity;
      Struct = tripleQueryStruct;
      RootEntity.PopulateCacheKey();
      Graph = new Graph(logger);
    }

    public Graph BuildLibraryGraph(int maxIterations)
    {
      RootEntity.PopulateCacheKey();
      RootEntity.CalculateGraphKey();
      string rootKey = RootEntity.GraphKey;
      _logger.LogDebug($"buildLibraryGraph, rootKey: {rootKey}");
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
      _logger.LogDebug($"buildAuthorGraph, author: {authorLabel}, rootKey: {rootKey}");

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
      _logger.LogDebug($"collectLibraryGraph, maxIterations: {maxIterations}");
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
                newNodesThisIteration += changes;
              }
            }
          }
        }
        // terminate the while-loop as necessary
        if (newNodesThisIteration < 1)
        {
          continueToCollect = false;
          _logger.LogError("collect() terminating with no new nodes");
        }
        else
        {
          if (iterations >= maxIterations)
          {
            continueToCollect = false;  // possible infinite loop, eject!
            _logger.LogError($"collect() bailing out at maxIterations {maxIterations}");
          }
        }
      }
    }
  }
}