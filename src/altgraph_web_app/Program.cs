using altgraph_shared_app.Models;
using altgraph_shared_app.Options;
using altgraph_shared_app.Services.Cache;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<CacheOptions>(builder.Configuration.GetSection(CacheOptions.Cache));
builder.Services.Configure<CosmosOptions>(builder.Configuration.GetSection(CosmosOptions.Cosmos));
builder.Services.Configure<PathsOptions>(builder.Configuration.GetSection(PathsOptions.Paths));
builder.Services.Configure<RedisOptions>(builder.Configuration.GetSection(RedisOptions.Redis));

builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(builder.Configuration.GetSection(RedisOptions.Redis).Get<RedisOptions>().ConnectionString));
builder.Services.AddCosmosRepository(options =>
{
  options.CosmosConnectionString = builder.Configuration.GetSection(CosmosOptions.Cosmos).Get<CosmosOptions>().ConnectionString;
  options.ContainerId = builder.Configuration.GetSection(CosmosOptions.Cosmos).Get<CosmosOptions>().ContainerId;
  options.DatabaseId = builder.Configuration.GetSection(CosmosOptions.Cosmos).Get<CosmosOptions>().DatabaseId;
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
builder.Services.AddSingleton<Cache>();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
  options.IdleTimeout = TimeSpan.FromMinutes(30);
  options.Cookie.HttpOnly = true;
  options.Cookie.IsEssential = true;
});

// Add services to the container.
builder.Services.AddRazorPages();

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

app.Run();
