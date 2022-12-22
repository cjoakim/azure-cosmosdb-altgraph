using altgraph_shared_app.Options;
using altgraph_shared_app.Services.Graph;

namespace altgraph_web_app.Services.Graph
{
  public class D3CsvBuilder
  {
    public altgraph_shared_app.Services.Graph.Graph? Graph { get; set; }
    public List<string> NodesCsvLines { get; set; } = new List<string>();
    public List<string> EdgeCsvLines { get; set; } = new List<string>();
    public Dictionary<string, string> CollectedNodesHash { get; set; } = new Dictionary<string, string>();
    public int CollectedNodesCount { get; set; }
    public Dictionary<string, string> CollectedEdgesHash { get; set; } = new Dictionary<string, string>();
    public int CollectedEdgesCount { get; set; }
    public string NodesCsvFile { get; set; } = string.Empty;
    public string EdgesCsvFile { get; set; } = string.Empty;
    private int iterationCount = 0;
    private readonly ILogger _logger;

    public D3CsvBuilder(altgraph_shared_app.Services.Graph.Graph g, PathsOptions pathsOptions, ILogger logger)
    {
      Graph = g;
      _logger = logger;
      NodesCsvFile = pathsOptions.NodesCsvFile;
      EdgesCsvFile = pathsOptions.EdgesCsvFile;
    }
    public async Task BuildBillOfMaterialCsvAsync(int depth)
    {
      CollectDataFromGraph(depth);
      BuildNodesCsv();
      BuildEdgesCsv();
      await WriteCsvFilesAsync();
    }

    private void CollectDataFromGraph(int depth)
    {
      bool continueToCollect = true;

      if (Graph == null)
      {
        throw new NullReferenceException("D3CsvBuilder.Graph is NULL");
      }

      string rootKey = Graph.RootKey;
      _logger.LogDebug($"collectLibrariesDataFromGraph rootKey: {rootKey}");

      string rootLib = ExtractNameFromKey(rootKey);
      _logger.LogDebug($"collectLibrariesDataFromGraph rootLib: {rootLib}");

      string rootEdgeKey = EdgeKey(rootLib, rootLib);
      _logger.LogDebug($"collectLibrariesDataFromGraph rootHashKey: {rootEdgeKey}");

      CollectedNodesHash[rootKey] = "pending";
      CollectedEdgesHash[rootEdgeKey] = string.Empty;

      while (continueToCollect)
      {
        iterationCount++;

        var currentKeys = CollectedNodesHash.Keys.ToList();
        for (int i = 0; i < currentKeys.Count; i++)
        {
          string currKey = currentKeys[i];
          string currVal = CollectedNodesHash[currKey];
          string currLib = ExtractNameFromKey(currKey);

          if (currVal.Equals("pending", StringComparison.OrdinalIgnoreCase))
          {
            GraphNode node = Graph.GraphMap[currKey];
            var dependencyKeys = node.AdjacentNodes.Keys.ToList();
            for (int d = 0; d < dependencyKeys.Count; d++)
            {
              string depKey = dependencyKeys[d];
              if (depKey.StartsWith("library"))
              {
                if (CollectedNodesHash.ContainsKey(depKey))
                {
                  _logger.LogDebug($"already in collectedNodesHash: {depKey}");
                }
                else
                {
                  CollectedNodesHash[depKey] = "pending";
                  string depLib = ExtractNameFromKey(depKey);
                  string depEdgeKey = EdgeKey(currLib, depLib);
                  CollectedEdgesHash[depEdgeKey] = string.Empty;
                }
              }
            }
            CollectedNodesHash[currKey] = "processed";
          }
        }

        // terminate the while-loop as necessary
        if (iterationCount == depth)
        {
          continueToCollect = false;
          _logger.LogError($"buildBillOfMaterialCsv while loop terminating at depth: {iterationCount}");
        }
        if (iterationCount >= 99)
        {
          continueToCollect = false;  // possible infinite loop, eject!
          _logger.LogError($"buildBillOfMaterialCsv while loop bailing out at iterationCount {iterationCount}");
        }
      }
    }

    private void BuildNodesCsv()
    {
      NodesCsvLines.Add("name,type,adjCount");  // header row
      List<string> keys = SortedArray(CollectedNodesHash.Keys.ToArray());
      for (int i = 0; i < keys.Count; i++)
      {
        string key = keys[i];
        string libName = ExtractNameFromKey(key);

        if (Graph == null)
        {
          throw new NullReferenceException("D3CsvBuilder.Graph is NULL");
        }

        GraphNode node = Graph.GraphMap[key];
        NodesCsvLines.Add(libName + ",library," + node.AdjacentNodes.Count);
      }
      _logger.LogDebug($"buildNodesCsv count: {NodesCsvLines.Count}");
    }

    private void BuildEdgesCsv()
    {
      EdgeCsvLines.Add("source,target,weight");  // header row
      List<string> keys = SortedArray(CollectedEdgesHash.Keys.ToArray());
      for (int i = 0; i < keys.Count; i++)
      {
        string key = keys[i];
        string[] tokens = key.Split(" ");
        EdgeCsvLines.Add(tokens[0] + "," + tokens[1] + ",1");
      }
      _logger.LogDebug($"buildEdgesCsv count: {EdgeCsvLines.Count}");
    }

    private static List<string> SortedArray(string[] array)
    {
      List<string> strings = new List<string>();
      for (int i = 0; i < array.Length; i++)
      {
        strings.Add(array[i]);
      }
      strings.Sort();
      return strings;
    }

    private static string EdgeKey(string parentLibName, string childLibName)
    {
      return parentLibName + " " + childLibName;
    }

    private string ExtractNameFromKey(string key)
    {
      string name = key.Replace("^", ":")
                .Split(":")[1]
                .Replace(" ", "_");

      _logger.LogDebug($"extractNameFromKey: {key} -> {name}");
      // library^express^bf8cff83-5f7c-4995-8484-d2f405bcbce7^express -> express
      // author^TJ Holowaychuk <tj@vision-media.ca>^54dff427-35de-4a13-bcad-b3e4124b303a^TJ Holowaychuk <tj@vision-media.ca> -> TJ Holowaychuk <tj@vision-media.ca>

      return name;
    }

    private async Task WriteCsvFilesAsync()
    {
      string? nodesCsvDirectory = Path.GetDirectoryName(NodesCsvFile);
      if (nodesCsvDirectory != null)
      {
        Directory.CreateDirectory(nodesCsvDirectory);
      }

      string? edgesCsvDirectory = Path.GetDirectoryName(EdgesCsvFile);
      if (edgesCsvDirectory != null)
      {
        Directory.CreateDirectory(edgesCsvDirectory);
      }

      Task[] tasks = { File.WriteAllLinesAsync(NodesCsvFile, NodesCsvLines), File.WriteAllLinesAsync(EdgesCsvFile, EdgeCsvLines) };
      await Task.WhenAll(tasks);
    }

    public void Finish()
    {
      Graph = null;
      CollectedNodesCount = CollectedNodesHash.Count;
      CollectedEdgesCount = CollectedEdgesHash.Count;
    }
  }
}