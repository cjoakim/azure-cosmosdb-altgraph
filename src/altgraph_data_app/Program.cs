using System.Reflection;
using altgraph_data_app;
using altgraph_data_app.processor;
using altgraph_shared_app.Models;
using altgraph_shared_app.Options;
using altgraph_shared_app.Services.Cache;
using altgraph_shared_app.Services.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StackExchange.Redis;

string? directoryName = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

await Host.CreateDefaultBuilder(args)
.ConfigureAppConfiguration((env, config) =>
{
  var appAssembly = Assembly.Load(new AssemblyName(env.HostingEnvironment.ApplicationName));
  if (appAssembly != null)
  {
    config.AddUserSecrets(appAssembly, optional: true);
  }
})
.UseContentRoot(directoryName != null ? directoryName : string.Empty)
.ConfigureLogging(logging =>
{

})
.ConfigureServices((hostContext, services) =>
{
  services.Configure<CacheOptions>(builder.Configuration.GetSection(CacheOptions.Cache));
  services.Configure<CosmosOptions>(builder.Configuration.GetSection(CosmosOptions.Cosmos));
  services.Configure<NpmPathsOptions>(builder.Configuration.GetSection(NpmPathsOptions.NpmPaths));
  services.Configure<ImdbPathsOptions>(builder.Configuration.GetSection(ImdbPathsOptions.ImdbPaths));
  services.Configure<RedisOptions>(builder.Configuration.GetSection(RedisOptions.Redis));
  services.Configure<ImdbOptions>(builder.Configuration.GetSection(ImdbOptions.Imdb));
  services.Configure<NpmOptions>(builder.Configuration.GetSection(NpmOptions.Npm));

  services.AddCosmosRepository(options =>
{
  CosmosOptions? cosmosOptions = hostContext.Configuration.GetSection(CosmosOptions.Cosmos).Get<CosmosOptions>();
  if (cosmosOptions != null)
  {
    options.CosmosConnectionString = cosmosOptions.ConnectionString;
    options.DatabaseId = cosmosOptions.DatabaseId;
    options.ContainerPerItemType = true;
    options.AllowBulkExecution = true;
  }
  else
  {
    throw new ArgumentNullException("CosmosOptions in appsettings.json cannot be null.");
  }
});
  services.AddSingleton<Cache>();
  services.AddSingleton<NpmCosmosDbLoader>();
  services.AddHostedService<ConsoleHostedService>();
})
            .RunConsoleAsync();