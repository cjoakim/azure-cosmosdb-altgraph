using System;
using System.Collections.Generic;

namespace altgraph_web_app.Services.Graph
{
  public class GraphNode
  {
    public string? TripleKey { get; set; }

    public int? AdjacentNodeCount { get; set; } = 0;

    public Dictionary<string, string>? AdjacentNodes { get; set; } = new Dictionary<string, string>();

    public GraphNode(string key)
    {
      TripleKey = key;
    }

    public GraphNode(bool root, string key)
    {
      TripleKey = key;
    }

    public int AddAdjacent(GraphNode neighbor, string predicate)
    {
      if (AdjacentNodes.ContainsKey(neighbor.TripleKey))
      {
        //log.warn("addAdjacent()_present: " + neighbor.getTripleKey() + " to " + this.getTripleKey());
        return 0;
      }
      else
      {
        //log.warn("addAdjacent()_adding:  " + neighbor.getTripleKey() + " to " + this.getTripleKey());
        AdjacentNodes[neighbor.TripleKey] = predicate;
        AdjacentNodeCount = AdjacentNodes.Count;
        return 1;
      }
    }

    public string AsJson(bool pretty)
    {
      throw new NotImplementedException();
      // try {
      //     ObjectMapper mapper = new ObjectMapper();
      //     if (pretty) {
      //         return mapper.writerWithDefaultPrettyPrinter().writeValueAsString(this);
      //     }
      //     else {
      //         return mapper.writeValueAsString(this);
      //     }
      // }
      // catch (JsonProcessingException e) {
      //     return null;
      // }
    }
  }
}