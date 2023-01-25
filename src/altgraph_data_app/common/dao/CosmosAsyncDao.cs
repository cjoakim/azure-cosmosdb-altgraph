using System.Net;
using altgraph_shared_app.Models;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;

namespace altgraph_data_app.common.dao
{
  public class CosmosAsyncDao
  {
    private CosmosClient _cosmosClient;
    private Database? _database = null;
    private Container? _container = null;
    private ILogger<CosmosAsyncDao> _logger;
    private string _currentContainerName = "";
    public string CurrentDbName { get; set; } = "";
    public CosmosAsyncDao(CosmosClient cosmosClient, ILogger<CosmosAsyncDao> logger)
    {
      _cosmosClient = cosmosClient;
      _logger = logger;
    }

    public CosmosClient Initialize()
    {
      _database = _cosmosClient.GetDatabase(CurrentDbName);
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
        _container = _database?.GetContainer(c);
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
      //int count = 0;
      int maxConcurrency = 100;

      using (SemaphoreSlim concurrencySemaphore = new SemaphoreSlim(maxConcurrency))
      {
        foreach (T item in items)
        {
          // if (count % 10000 == 0)
          // {
          //   _logger.LogInformation($"Processed {count} items out of {items.Count}.");
          // }

          concurrencySemaphore.Wait();

          // try
          // {
          //   await _container.CreateItemAsync<T>(item, new PartitionKey(item.Pk));
          // }
          // catch (CosmosException ex) when (ex.StatusCode != HttpStatusCode.Conflict)
          // {
          //   _logger.LogError($"Exception {ex.Message}.");
          // }
          // catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.Conflict)
          // {
          //   //do nothing
          // }
          // catch (Exception ex)
          // {
          //   _logger.LogError($"Exception {ex.Message}.");
          // }

          //count++;
          tasks.Add(_container.CreateItemAsync(item, new PartitionKey(item.Pk))
            .ContinueWith(itemResponse =>
          {
            // if (!itemResponse.IsCompletedSuccessfully)
            // {
            //   AggregateException innerExceptions = itemResponse.Exception.Flatten();
            //   if (innerExceptions.InnerExceptions.FirstOrDefault(innerEx => innerEx is CosmosException) is CosmosException cosmosException)
            //   {
            //     Console.WriteLine($"Received {cosmosException.StatusCode} ({cosmosException.Message}).");
            //   }
            //   else
            //   {
            //     Console.WriteLine($"Exception {innerExceptions.InnerExceptions.FirstOrDefault()}.");
            //   }
            // }
            concurrencySemaphore.Release();
          }));
        }

        await Task.WhenAll(tasks);
      }
    }
  }
}