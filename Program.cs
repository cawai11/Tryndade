using Microsoft.AspNetCore.Authentication.Cookies;
using Tryndade.Services; // Ajuste o namespace conforme seu projeto

var builder = WebApplication.CreateBuilder(args);

// Adicionar serviços ao contêiner
builder.Services.AddControllersWithViews();

builder.Services.AddHttpContextAccessor();

// Configurar a autenticação por cookies
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";
    });

// Registrar o ApiService
var apiSettings = builder.Configuration.GetSection("ApiSettings");
builder.Services.AddHttpClient<ApiService>(client =>
{
    client.BaseAddress = new Uri(apiSettings["BaseUrl"]);
});

var app = builder.Build();

// Configurar o pipeline de middleware
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
