using ShoppingSystem.DB_Seeder;

var builder = WebApplication.CreateBuilder(args);

var databasePath = Path.GetFullPath(Path.Combine(builder.Environment.ContentRootPath, "..", "DB", "shopping_system_local.db"));
var seedDataPath = Path.Combine(builder.Environment.ContentRootPath, "DB_Seeder", "DB_Seeder_Data");

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

await DatabaseSeeder.SeedIfEmptyAsync(databasePath, seedDataPath);

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
