using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using altgraph_data_app;
using altgraph_data_app.processor;
using altgraph_shared_app.Models;
using altgraph_shared_app.Options;
using altgraph_shared_app.Services.Cache;
using altgraph_shared_app.Services.Graph.v2;
using altgraph_shared_app.Services.Repositories;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Fluent;
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
  services.Configure<CacheOptions>(hostContext.Configuration.GetSection(CacheOptions.Cache));
  services.Configure<CosmosOptions>(hostContext.Configuration.GetSection(CosmosOptions.Cosmos));
  services.Configure<NpmPathsOptions>(hostContext.Configuration.GetSection(NpmPathsOptions.NpmPaths));
  services.Configure<ImdbPathsOptions>(hostContext.Configuration.GetSection(ImdbPathsOptions.ImdbPaths));
  services.Configure<RedisOptions>(hostContext.Configuration.GetSection(RedisOptions.Redis));
  services.Configure<ImdbOptions>(hostContext.Configuration.GetSection(ImdbOptions.Imdb));
  services.Configure<NpmOptions>(hostContext.Configuration.GetSection(NpmOptions.Npm));

  CosmosOptions? cosmosOptions = hostContext.Configuration.GetSection(CosmosOptions.Cosmos).Get<CosmosOptions>();

  services.AddCosmosRepository(options =>
{
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
  services.AddSingleton(s =>
  {
    if (cosmosOptions != null)
    {
      JsonSerializerOptions jsonSerializerOptions = new JsonSerializerOptions()
      {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
      };
      CosmosSystemTextJsonSerializer cosmosSystemTextJsonSerializer = new CosmosSystemTextJsonSerializer(jsonSerializerOptions);

      return new CosmosClientBuilder(cosmosOptions.ConnectionString)
        .WithApplicationPreferredRegions(cosmosOptions.PreferredLocations)
        .WithConsistencyLevel(ConsistencyLevel.Session)
        .WithBulkExecution(true)
        .WithCustomSerializer(cosmosSystemTextJsonSerializer)
        .WithContentResponseOnWrite(true)
        .Build();
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