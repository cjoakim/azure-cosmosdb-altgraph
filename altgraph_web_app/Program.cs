using altgraph_web_app.Services.Cache;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(builder.Configuration["Redis:ConnectionString"]));
builder.Services.AddCosmosRepository(options =>
{
  options.CosmosConnectionString = builder.Configuration["Cosmos:ConnectionString"];
  options.ContainerId = builder.Configuration["Cosmos:ContainerId"];
  options.DatabaseId = builder.Configuration["Cosmos:DatabaseId"];
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
