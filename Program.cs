using DrozdovLaw.Data;
using DrozdovLaw.Interfaces;
using DrozdovLaw.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddControllersWithViews();
//builder.Services.AddSingleton<ContentService>();
builder.Services.AddHttpClient<ITranslationService, TranslationService>();
builder.Services.AddScoped<IBlockService, BlockService>();
builder.Services.AddScoped<ISectionService, SectionService>();
builder.Services.AddScoped<IPreviewService, PreviewService>();
builder.Services.AddScoped<ICaseTemplateBuilder, CaseTemplateBuilder>();
builder.Services.AddHttpClient<ITranslationService, TranslationService>();
builder.Services.AddScoped<ILanguageService, LanguageService>();
builder.Services.AddScoped<JsonMigrator>();
var app = builder.Build();
//using (var scope = app.Services.CreateScope())
//{
//    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
//    // Применяем EF миграции (если есть)
//    await dbContext.Database.MigrateAsync();

//    // Проверяем, есть ли уже данные (по наличию секций)
//    //if (!await dbContext.Sections.AnyAsync())
//    //{
//    var migrator = scope.ServiceProvider.GetRequiredService<JsonMigrator>();
//    var jsonPath = Path.Combine(app.Environment.ContentRootPath, "seed-data.json");
//    if (File.Exists(jsonPath))
//    {
//        await migrator.MigrateAsync(jsonPath);
//    }
//    else
//    {
//        // Опционально: логируем, что файл не найден
//        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
//        logger.LogWarning("Seed data file not found at {Path}", jsonPath);
//    }
//}
////}
if (!app.Environment.IsDevelopment()) app.UseExceptionHandler("/Home/Error");
app.UseStaticFiles();
app.UseRouting();

app.MapControllerRoute("case-detail", "Cases/{slug}",
    defaults: new { controller = "Case", action = "Detail" });
app.MapControllerRoute("default", "{controller=Case}/{action=List}/{id?}");

app.Run();
