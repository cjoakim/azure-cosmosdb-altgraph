using Microsoft.Extensions.Logging;

namespace altgraph_shared_app.Services.Graph
{
  public class Graph
  {
    public string RootKey { get; set; } = string.Empty;
    public Dictionary<string, GraphNode> GraphMap { get; set; } = new Dictionary<string, GraphNode>();
    public long StartTime { get; set; }
    public long EndTime { get; set; }
    public long ElapsedMs { get; set; }
    private readonly ILogger _logger;

    public Graph(ILogger logger)
    {
      _logger = logger;
      StartTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
    }

    public void SetRootNode(string key)
    {
      RootKey = key;
      GraphNode node = new GraphNode(key, _logger);
      GraphMap[key] = node;
    }

    public int UpdateForLibrary(string subjectKey, string objectKey, string predicate)
    {
      int changeCount = 0;
      GraphNode subjectNode;
      GraphNode objectNode;

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
      changeCount += addAdjResult;

      return changeCount;
    }

    public int UpdateForAuthor(string subjectKey, string objectKey, string predicate)
    {
      int changeCount = 0;
      GraphNode subjectNode;
      GraphNode objectNode;

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
      changeCount += addAdjResult;

      return changeCount;
    }

    public GraphNode CreateNewNode(string key)
    {
      GraphNode node = new GraphNode(key, _logger);
      GraphMap[key] = node;
      return node;
    }

    public List<string> GetCurrentKeys()
    {
      return GraphMap.Keys.ToList();
    }

    public long Finish()
    {
      EndTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
      ElapsedMs = EndTime - StartTime;
      _logger.LogDebug($"finish() elapsedMs: {ElapsedMs}");
      return ElapsedMs;
    }
  }
}