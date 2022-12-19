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

await Host.CreateDefaultBuilder(args)
.ConfigureAppConfiguration((env, config) =>
{
  var appAssembly = Assembly.Load(new AssemblyName(env.HostingEnvironment.ApplicationName));
  if (appAssembly != null)
  {
    config.AddUserSecrets(appAssembly, optional: true);
  }
})
.UseContentRoot(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location))
.ConfigureLogging(logging =>
{

})
.ConfigureServices((hostContext, services) =>
{
  services.Configure<CacheOptions>(hostContext.Configuration.GetSection(CacheOptions.Cache));
  services.Configure<CosmosOptions>(hostContext.Configuration.GetSection(CosmosOptions.Cosmos));
  services.Configure<PathsOptions>(hostContext.Configuration.GetSection(PathsOptions.Paths));
  services.Configure<RedisOptions>(hostContext.Configuration.GetSection(RedisOptions.Redis));

  services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(hostContext.Configuration.GetSection(RedisOptions.Redis).Get<RedisOptions>().ConnectionString));
  services.AddCosmosRepository(options =>
  {
    options.CosmosConnectionString = hostContext.Configuration.GetSection(CosmosOptions.Cosmos).Get<CosmosOptions>().ConnectionString;
    options.ContainerId = hostContext.Configuration.GetSection(CosmosOptions.Cosmos).Get<CosmosOptions>().ContainerId;
    options.DatabaseId = hostContext.Configuration.GetSection(CosmosOptions.Cosmos).Get<CosmosOptions>().DatabaseId;
    options.ContainerPerItemType = true;
    options.ContainerBuilder.Configure<Author>(containerOptions =>
    {
      containerOptions.WithContainer("altgraph");
      containerOptions.WithPartitionKey("/pk");
    });
    options.ContainerBuilder.Configure<Library>(containerOptions =>
    {
      containerOptions.WithContainer("altgraph");
      containerOptions.WithPartitionKey("/pk");
    });
    options.ContainerBuilder.Configure<Maintainer>(containerOptions =>
    {
      containerOptions.WithContainer("altgraph");
      containerOptions.WithPartitionKey("/pk");
    });
    options.ContainerBuilder.Configure<Triple>(containerOptions =>
    {
      containerOptions.WithContainer("altgraph");
      containerOptions.WithPartitionKey("/pk");
    });
  });
  services.AddSingleton<Cache>();
  services.AddSingleton<NpmCosmosDbLoader>();
  services.AddHostedService<ConsoleHostedService>();
})
            .RunConsoleAsync();