using altgraph_shared_app.Models;
using Microsoft.Azure.Cosmos;

namespace altgraph_data_app.common.dao
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

    public async Task BulkLoadAsync<T>(List<T> items) where T : AbstractDocument
    {
      if (_container == null)
      {
        throw new ArgumentException("Container not initialized");
      }

      List<Task> tasks = new List<Task>(items.Count);
      foreach (T item in items)
      {
        tasks.Add(_container.CreateItemAsync(item, new PartitionKey(item.Pk))
          .ContinueWith(itemResponse =>
        {
          if (!itemResponse.IsCompletedSuccessfully)
          {
            AggregateException innerExceptions = itemResponse.Exception.Flatten();
            if (innerExceptions.InnerExceptions.FirstOrDefault(innerEx => innerEx is CosmosException) is CosmosException cosmosException)
            {
              Console.WriteLine($"Received {cosmosException.StatusCode} ({cosmosException.Message}).");
            }
            else
            {
              Console.WriteLine($"Exception {innerExceptions.InnerExceptions.FirstOrDefault()}.");
            }
          }
        }));
      }

      await Task.WhenAll(tasks);
    }
  }
}