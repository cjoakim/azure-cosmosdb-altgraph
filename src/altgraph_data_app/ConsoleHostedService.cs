using altgraph_data_app.processor;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace altgraph_data_app
{
  internal sealed class ConsoleHostedService : IHostedService
  {
    private readonly ILogger _logger;
    private readonly IHostApplicationLifetime _appLifetime;
    private readonly CosmosDbLoader _cosmosDbLoader;
    private int? _exitCode;

    public ConsoleHostedService(
        ILogger<ConsoleHostedService> logger,
        IHostApplicationLifetime appLifetime, CosmosDbLoader cosmosDbLoader)
    {
      _logger = logger;
      _appLifetime = appLifetime;
      _cosmosDbLoader = cosmosDbLoader;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
      _logger.LogDebug($"Starting with arguments: {string.Join(" ", Environment.GetCommandLineArgs())}");

      _appLifetime.ApplicationStarted.Register(() =>
      {
        Task.Run(async () =>
          {
            try
            {
              string processType = Environment.GetCommandLineArgs()[1];

              switch (processType)
              {
                case "transform_raw_data":
                  break;
                case "load_cosmos":
                  await _cosmosDbLoader.ProcessAsync();
                  break;
                case "springdata_queries":
                  break;
                case "dao_queries":
                  break;
                case "build_graph":
                  break;
                case "build_d3_csv":
                  break;
                case "test_cache":
                  break;
                case "test_redis":
                  break;
                default:
                  break;
              }
              _exitCode = 0;
            }
            catch (Exception ex)
            {
              _logger.LogError(ex, "Unhandled exception!");
              _exitCode = 1;
            }
            finally
            {
              // Stop the application once the work is done
              _appLifetime.StopApplication();
            }
          });
      });

      return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
      _logger.LogDebug($"Exiting with return code: {_exitCode}");

      // Exit code may be null if the user cancelled via Ctrl+C/SIGTERM
      Environment.ExitCode = _exitCode.GetValueOrDefault(-1);
      return Task.CompletedTask;
    }
  }
}