using System.Globalization;
using Microsoft.AspNetCore.Localization;
using WarehouseRequisition.Common;
using WarehouseRequisition.Configuration;
using WarehouseRequisition.Data;
using WarehouseRequisition.Repositories;
using WarehouseRequisition.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

// Prototype configuration: local JSON snapshot storage and the base URL encoded in QR codes.
builder.Services.Configure<StorageOptions>(builder.Configuration.GetSection(StorageOptions.SectionName));
builder.Services.Configure<FulfillmentOptions>(builder.Configuration.GetSection(FulfillmentOptions.SectionName));

// The user interface is Spanish (Mexico); all source code remains in English.
var culture = new CultureInfo("es-MX");
builder.Services.AddRequestLocalization(options =>
{
    options.DefaultRequestCulture = new RequestCulture(culture);
    options.SupportedCultures = [culture];
    options.SupportedUICultures = [culture];
});

// Persistence layer (replace with EF Core + PostgreSQL by swapping these registrations).
builder.Services.AddSingleton<InMemoryDataStore>();
builder.Services.AddSingleton<IDataStorePersistence, JsonFileDataStorePersistence>();
builder.Services.AddSingleton<IDataStoreSeeder, DataStoreSeeder>();
builder.Services.AddSingleton<IRequisitionRepository, InMemoryRequisitionRepository>();

// Application services.
builder.Services.AddScoped<ICatalogService, CatalogService>();
builder.Services.AddScoped<IPartService, PartService>();
builder.Services.AddScoped<IRequisitionService, RequisitionService>();
builder.Services.AddScoped<IMaterialGenerationService, MockMaterialGenerationService>();
builder.Services.AddScoped<IQrCodeService, QrCodeService>();
builder.Services.AddScoped<IBarcodeService, BarcodeService>();
builder.Services.AddScoped<ICurrentUserService, MockCurrentUser>();

var app = builder.Build();

using (var seedScope = app.Services.CreateScope())
{
    seedScope.ServiceProvider.GetRequiredService<IDataStoreSeeder>().SeedIfEmpty();
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseRequestLocalization();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
