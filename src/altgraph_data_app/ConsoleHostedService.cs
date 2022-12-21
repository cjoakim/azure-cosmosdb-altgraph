using altgraph_data_app.processor;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace altgraph_data_app
{
  internal sealed class ConsoleHostedService : IHostedService
  {
    private readonly ILogger _logger;
    private readonly IHostApplicationLifetime _appLifetime;
    private readonly NpmCosmosDbLoader _cosmosDbLoader;
    private int? _exitCode;

    public ConsoleHostedService(
        ILogger<ConsoleHostedService> logger,
        IHostApplicationLifetime appLifetime, NpmCosmosDbLoader cosmosDbLoader)
    {
      _logger = logger;
      _appLifetime = appLifetime;
      _cosmosDbLoader = cosmosDbLoader;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
      _logger.LogInformation($"Starting with arguments: {string.Join(" ", Environment.GetCommandLineArgs())}");

      _appLifetime.ApplicationStarted.Register(() =>
      {
        Task.Run(async () =>
          {
            try
            {
              string processType = Environment.GetCommandLineArgs()[1];

              switch (processType)
              {
                case "load_cosmos":
                  await _cosmosDbLoader.ProcessAsync();
                  break;
                default:
                  throw new NotImplementedException($"Process type {processType} not implemented");
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