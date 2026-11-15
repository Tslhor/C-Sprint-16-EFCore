using DB_Seeder;
using DB_Seeder.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

var databasePath = Path.GetFullPath(Path.Combine(builder.Environment.ContentRootPath, "..", "DB", "shopping_system_local.db"));
var seedDataPath = Path.GetFullPath(Path.Combine(builder.Environment.ContentRootPath, "..", "DB_Seeder", "SeedData"));

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddShoppingSqliteSeeder($"Data Source={databasePath}");

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
