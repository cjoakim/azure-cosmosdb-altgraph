using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using altgraph_data_app;
using altgraph_data_app.common.io;
using altgraph_data_app.processor;
using altgraph_shared_app.Models.Imdb;
using altgraph_shared_app.Models.Npm;
using altgraph_shared_app.Options;
using altgraph_shared_app.Services.Cache;
using altgraph_shared_app.Services.Graph.v2;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Fluent;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

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
  NpmOptions? npmOptions = hostContext.Configuration.GetSection(NpmOptions.Npm).Get<NpmOptions>();
  ImdbOptions? imdbOptions = hostContext.Configuration.GetSection(ImdbOptions.Imdb).Get<ImdbOptions>();

  services.AddCosmosRepository(options =>
{
  if (cosmosOptions != null)
  {
    options.CosmosConnectionString = cosmosOptions.ConnectionString;
    options.DatabaseId = cosmosOptions.DatabaseId;
    options.ContainerPerItemType = true;
    options.AllowBulkExecution = true;
    if (cosmosOptions != null && npmOptions != null && imdbOptions != null)
    {
      options.CosmosConnectionString = cosmosOptions.ConnectionString;
      options.DatabaseId = cosmosOptions.DatabaseId;
      options.ContainerPerItemType = true;
      options.ContainerBuilder.Configure<Author>(containerOptions =>
      {
        containerOptions.WithServerlessThroughput();
        containerOptions.WithContainer(npmOptions.ContainerName);
        containerOptions.WithPartitionKey(npmOptions.PartitionKey);
      });
      options.ContainerBuilder.Configure<Library>(containerOptions =>
      {
        containerOptions.WithServerlessThroughput();
        containerOptions.WithContainer(npmOptions.ContainerName);
        containerOptions.WithPartitionKey(npmOptions.PartitionKey);
      });
      options.ContainerBuilder.Configure<Maintainer>(containerOptions =>
      {
        containerOptions.WithServerlessThroughput();
        containerOptions.WithContainer(npmOptions.ContainerName);
        containerOptions.WithPartitionKey(npmOptions.PartitionKey);
      });
      options.ContainerBuilder.Configure<Triple>(containerOptions =>
      {
        containerOptions.WithServerlessThroughput();
        containerOptions.WithContainer(npmOptions.ContainerName);
        containerOptions.WithPartitionKey(npmOptions.PartitionKey);
      });
      options.ContainerBuilder.Configure<Movie>(containerOptions =>
      {
        containerOptions.WithServerlessThroughput();
        containerOptions.WithContainer(imdbOptions.GraphContainerName);
        containerOptions.WithPartitionKey(imdbOptions.PartitionKey);
      });
      options.ContainerBuilder.Configure<Person>(containerOptions =>
      {
        containerOptions.WithServerlessThroughput();
        containerOptions.WithContainer(imdbOptions.GraphContainerName);
        containerOptions.WithPartitionKey(imdbOptions.PartitionKey);
      });
    }
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
  services.AddTransient<JsonLoader>();
  services.AddTransient<ConsoleAppProcess>();
  services.AddSingleton<NpmCosmosDbLoader>();
  services.AddSingleton<SdkBulkLoaderProcessor>();
  services.AddSingleton<ImdbRawDataWranglerProcess>();
  services.AddSingleton<ImdbTripleBuilderProcess>();
  services.AddHostedService<ConsoleHostedService>();
})
            .RunConsoleAsync();