using DB_Seeder;
using DB_Seeder.Configuration;
using DB_Seeder.DependencyInjection;
using ShoppingSystem.Services;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("LocalSqlite")
    ?? throw new InvalidOperationException("Connection string 'LocalSqlite' is not configured.");
var resolvedConnectionString = SqliteConnectionStringResolver.Resolve(
    connectionString,
    builder.Environment.ContentRootPath);
var seedDataPath = Path.GetFullPath(Path.Combine(builder.Environment.ContentRootPath, "..", "DB_Seeder", "SeedData"));

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddShoppingSqliteSeeder(resolvedConnectionString);
builder.Services.AddScoped<IOrderFormService, OrderFormService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IOrderDetailsService, OrderDetailsService>();

var app = builder.Build();

await using (var scope = app.Services.CreateAsyncScope())
{
    var seeder = scope.ServiceProvider.GetRequiredService<IDatabaseSeeder>();
    await seeder.SeedIfEmptyAsync(seedDataPath);
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
