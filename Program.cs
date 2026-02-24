using DrozdovLaw.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();
builder.Services.AddSingleton<ContentService>();

var app = builder.Build();
if (!app.Environment.IsDevelopment()) app.UseExceptionHandler("/Home/Error");
app.UseStaticFiles();
app.UseRouting();

app.MapControllerRoute("case-detail", "Cases/{slug}",
    defaults: new { controller = "Case", action = "Detail" });
app.MapControllerRoute("default", "{controller=Case}/{action=List}/{id?}");

app.Run();
