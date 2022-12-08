using System;
using System.Collections.Generic;

namespace altgraph_web_app.Services.Graph
{
  public class GraphNode
  {
    public string TripleKey { get; set; }
    public int AdjacentNodeCount { get; set; }
    public Dictionary<string, string> AdjacentNodes { get; set; } = new Dictionary<string, string>();
    private readonly ILogger _logger;
    public GraphNode(string key, ILogger logger)
    {
      TripleKey = key;
      _logger = logger;
    }

    public GraphNode(bool root, string key, ILogger logger)
    {
      TripleKey = key;
      _logger = logger;
    }

    public int AddAdjacent(GraphNode neighbor, string predicate)
    {
      if (AdjacentNodes.ContainsKey(neighbor.TripleKey))
      {
        _logger.LogWarning($"addAdjacent()_present: {neighbor.TripleKey} to {this.TripleKey}");
        return 0;
      }
      else
      {
        _logger.LogWarning($"addAdjacent()_adding:  {neighbor.TripleKey} to {this.TripleKey}");
        AdjacentNodes[neighbor.TripleKey] = predicate;
        AdjacentNodeCount = AdjacentNodes.Count;
        return 1;
      }
    }
  }
}