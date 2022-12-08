namespace altgraph_web_app.Services.Graph
{

  public class D3CsvBuilder
  {
    public Graph? Graph { get; set; }
    public List<string> NodesCsvLines { get; set; } = new List<string>();
    public List<string> EdgeCsvLines { get; set; } = new List<string>();
    public Dictionary<string, string> CollectedNodesHash { get; set; } = new Dictionary<string, string>();
    public int CollectedNodesCount { get; set; }
    public Dictionary<string, string> CollectedEdgesHash { get; set; } = new Dictionary<string, string>();
    public int CollectedEdgesCount { get; set; }
    public string NodesCsvFile { get; set; } = "";
    public string EdgesCsvFile { get; set; } = "";
    private int iterationCount = 0;
    private readonly IConfiguration? _configuration;
    private readonly ILogger _logger;

    public D3CsvBuilder(Graph g, IConfiguration configuration, ILogger logger)
    {
      Graph = g;
      _configuration = configuration;
      _logger = logger;
      NodesCsvFile = _configuration["Paths:NodesCsvFile"];
      EdgesCsvFile = _configuration["Paths:EdgesCsvFile"];
    }

    public void BuildBillOfMaterialCsv(string sessionId, int depth)
    {
      CollectDataFromGraph(depth);
      BuildNodesCsv();
      BuildEdgesCsv();
      WriteCsvFilesAsync();
    }

    private void CollectDataFromGraph(int depth)
    {
      bool continueToCollect = true;
      string rootKey = Graph.RootKey;
      _logger.LogWarning($"collectLibrariesDataFromGraph rootKey: {rootKey}");

      string rootLib = ExtractNameFromKey(rootKey);
      _logger.LogWarning($"collectLibrariesDataFromGraph rootLib: {rootLib}");

      string rootEdgeKey = EdgeKey(rootLib, rootLib);
      _logger.LogWarning($"collectLibrariesDataFromGraph rootHashKey: {rootEdgeKey}");

      CollectedNodesHash[rootKey] = "pending";
      CollectedEdgesHash[rootEdgeKey] = "";

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
                  _logger.LogWarning($"already in collectedNodesHash: {depKey}");
                }
                else
                {
                  CollectedNodesHash[depKey] = "pending";
                  string depLib = ExtractNameFromKey(depKey);
                  string depEdgeKey = EdgeKey(currLib, depLib);
                  CollectedEdgesHash[depEdgeKey] = "";
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
        GraphNode node = Graph.GraphMap[key];
        NodesCsvLines.Add(libName + ",library," + node.AdjacentNodes.Count);
      }
      _logger.LogWarning($"buildNodesCsv count: {NodesCsvLines.Count}");
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
      _logger.LogWarning($"buildEdgesCsv count: {EdgeCsvLines.Count}");
    }

    private List<string> SortedArray(string[] array)
    {
      List<string> strings = new List<string>();
      for (int i = 0; i < array.Length; i++)
      {
        strings.Add((string)array[i]);
      }
      strings.Sort();
      return strings;
    }

    private string EdgeKey(string parentLibName, string childLibName)
    {
      return parentLibName + " " + childLibName;
    }

    private string ExtractNameFromKey(string key)
    {
      string name = key.Replace("^", ":").Split(":")[1].Replace(" ", "_");

      _logger.LogWarning($"extractNameFromKey: {key} -> {name}");
      // library^express^bf8cff83-5f7c-4995-8484-d2f405bcbce7^express -> express
      // author^TJ Holowaychuk <tj@vision-media.ca>^54dff427-35de-4a13-bcad-b3e4124b303a^TJ Holowaychuk <tj@vision-media.ca> -> TJ Holowaychuk <tj@vision-media.ca>

      return name;
    }

    private async void WriteCsvFilesAsync()
    {
      // GRAPH_NODES_CSV_FILE
      Directory.CreateDirectory(Path.GetDirectoryName(NodesCsvFile));
      Directory.CreateDirectory(Path.GetDirectoryName(EdgesCsvFile));

      await File.WriteAllLinesAsync(NodesCsvFile, NodesCsvLines);
      await File.WriteAllLinesAsync(EdgesCsvFile, EdgeCsvLines);
    }

    public void Finish()
    {
      Graph = null;
      CollectedNodesCount = CollectedNodesHash.Count();
      CollectedEdgesCount = CollectedEdgesHash.Count();
    }
  }

}