using altgraph_shared_app.Models.Imdb;
using altgraph_shared_app.Models.Npm;
using altgraph_shared_app.Options;
using altgraph_shared_app.Services.Cache;
using altgraph_shared_app.Services.Graph.v2;
using altgraph_shared_app.Util;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<CacheOptions>(builder.Configuration.GetSection(CacheOptions.Cache));
builder.Services.Configure<CosmosOptions>(builder.Configuration.GetSection(CosmosOptions.Cosmos));
builder.Services.Configure<PathsOptions>(builder.Configuration.GetSection(PathsOptions.Paths));
builder.Services.Configure<RedisOptions>(builder.Configuration.GetSection(RedisOptions.Redis));
builder.Services.Configure<ImdbOptions>(builder.Configuration.GetSection(ImdbOptions.Imdb));

RedisOptions? redisOptions = builder.Configuration.GetSection(RedisOptions.Redis).Get<RedisOptions>();
if (redisOptions != null)
{
  builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(redisOptions.ConnectionString));
}
else
{
  throw new ArgumentNullException("RedisOptions in appsettings.json cannot be null.");
}
builder.Services.AddCosmosRepository(options =>
{
  CosmosOptions? cosmosOptions = builder.Configuration.GetSection(CosmosOptions.Cosmos).Get<CosmosOptions>();
  if (cosmosOptions != null)
  {
    options.CosmosConnectionString = cosmosOptions.ConnectionString;
    options.DatabaseId = cosmosOptions.DatabaseId;
    options.ContainerPerItemType = true;
    options.ContainerBuilder.Configure<Author>(containerOptions =>
    {
      containerOptions.WithServerlessThroughput();
    });
    options.ContainerBuilder.Configure<Library>(containerOptions =>
    {
      containerOptions.WithServerlessThroughput();
    });
    options.ContainerBuilder.Configure<Maintainer>(containerOptions =>
    {
      containerOptions.WithServerlessThroughput();
    });
    options.ContainerBuilder.Configure<Triple>(containerOptions =>
    {
      containerOptions.WithServerlessThroughput();
    });
    options.ContainerBuilder.Configure<Movie>(containerOptions =>
    {
      containerOptions.WithServerlessThroughput();
    });
    options.ContainerBuilder.Configure<Person>(containerOptions =>
    {
      containerOptions.WithServerlessThroughput();
    });
  }
  else
  {
    throw new ArgumentNullException("CosmosOptions in appsettings.json cannot be null.");
  }
});
builder.Services.AddScoped<IMemoryStats, MemoryStats>();
builder.Services.AddSingleton<ICache, Cache>();
builder.Services.AddTransient<IJGraphBuilder, JGraphBuilder>();
builder.Services.AddSingleton<IJGraph, JGraph>();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
  options.IdleTimeout = TimeSpan.FromMinutes(30);
  options.Cookie.HttpOnly = true;
  options.Cookie.IsEssential = true;
});

// Add services to the container.
builder.Services.AddRazorPages();

builder.Services.AddHealthChecks();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
  app.UseExceptionHandler("/Error");
  // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
  app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();

app.UseAuthorization();

app.MapRazorPages();

app.MapHealthChecks("/healthz");

app.Run();
