using DrozdovLaw.Interfaces;
using DrozdovLaw.Repositories;
using DrozdovLaw.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();
//builder.Services.AddSingleton<ContentService>();
builder.Services.AddScoped<IContentRepository, JsonContentRepository>();
builder.Services.AddScoped<IBlockService, BlockService>();
builder.Services.AddScoped<ICaseService, CaseService>();
builder.Services.AddScoped<ICaseTemplateBuilder, CaseTemplateBuilder>();
builder.Services.AddScoped<IPreviewService, PreviewService>();
var app = builder.Build();
if (!app.Environment.IsDevelopment()) app.UseExceptionHandler("/Home/Error");
app.UseStaticFiles();
app.UseRouting();

app.MapControllerRoute("case-detail", "Cases/{slug}",
    defaults: new { controller = "Case", action = "Detail" });
app.MapControllerRoute("default", "{controller=Case}/{action=List}/{id?}");

app.Run();
