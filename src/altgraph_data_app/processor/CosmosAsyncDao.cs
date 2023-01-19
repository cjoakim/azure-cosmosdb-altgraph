using Microsoft.Azure.Cosmos;

namespace altgraph_data_app.processor
{
  public class CosmosAsyncDao
  {
    private CosmosClient _cosmosClient;
    private Database? _database = null;
    private Container? _container = null;
    private string _currentContainerName = "";
    private string _currentDbName = "";
    public CosmosAsyncDao(CosmosClient cosmosClient)
    {
      _cosmosClient = cosmosClient;
    }

    public CosmosClient Initialize()
    {
      _database = _cosmosClient.GetDatabase(_currentDbName);
      return _cosmosClient;
    }

    public void Close()
    {
      _cosmosClient.Dispose();
    }

    public string CurrentContainerName()
    {
      return _currentContainerName;
    }

    public void CurrentContainer(string c)
    {
      if (_currentContainerName.Equals(c, StringComparison.OrdinalIgnoreCase))
      {
        return;
      }
      else
      {
        _container = _database?.GetContainer(_currentContainerName);
        _currentContainerName = c;
      }
    }
  }
}