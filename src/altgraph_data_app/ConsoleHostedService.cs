using altgraph_data_app.processor;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace altgraph_data_app
{
  internal sealed class ConsoleHostedService : IHostedService
  {
    private readonly ILogger _logger;
    private readonly IHostApplicationLifetime _appLifetime;
    private readonly NpmCosmosDbLoader _npmCosmosDbLoader;
    private readonly SdkBulkLoaderProcessor _sdkBulkLoaderProcessor;
    private readonly ImdbRawDataWranglerProcess _imdbRawDataWranglerProcess;
    private int? _exitCode;

    public ConsoleHostedService(
        ILogger<ConsoleHostedService> logger,
        IHostApplicationLifetime appLifetime, NpmCosmosDbLoader npmCosmosDbLoader, SdkBulkLoaderProcessor sdkBulkLoaderProcessor, ImdbRawDataWranglerProcess imdbRawDataWranglerProcess)
    {
      _logger = logger;
      _appLifetime = appLifetime;
      _npmCosmosDbLoader = npmCosmosDbLoader;
      _sdkBulkLoaderProcessor = sdkBulkLoaderProcessor;
      _imdbRawDataWranglerProcess = imdbRawDataWranglerProcess;
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
                case "npm_load_cosmos":
                  await _npmCosmosDbLoader.ProcessAsync();
                  break;
                case "imdb_wrangle_raw_data":
                  _imdbRawDataWranglerProcess.MinYear = int.Parse(Environment.GetCommandLineArgs()[2]);
                  _imdbRawDataWranglerProcess.MinMinutes = int.Parse(Environment.GetCommandLineArgs()[3]);
                  await _imdbRawDataWranglerProcess.ProcessAsync();
                  break;
                case "imdb_bulk_load_movies":
                case "imdb_bulk_load_people":
                case "imdb_bulk_load_small_triples":
                case "imdb_bulk_load_movies_seed":
                  _sdkBulkLoaderProcessor.LoadType = processType;
                  _sdkBulkLoaderProcessor.Container = Environment.GetCommandLineArgs()[2];
                  await _sdkBulkLoaderProcessor.ProcessAsync();
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