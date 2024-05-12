using System.Net;
using System.Security.Cryptography;
using System.Text.Json;
using acme.Data;
using acme.Models;
using acme.Services;
using Auth0.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using NuGet.Common;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services

.AddAuth0WebAppAuthentication(options =>
{
    options.Domain = builder.Configuration["Auth0:Domain"];
    options.ClientId = builder.Configuration["Auth0:ClientId"];
    options.ClientSecret = builder.Configuration["Auth0:ClientSecret"];

    options.Scope = "openid profile email";


})
.WithAccessToken(options =>
{
    options.Audience = builder.Configuration["Auth0:Audience"];
});
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.Listen(IPAddress.Any, 5001, listenOptions =>
    {
        listenOptions.UseHttps("/etc/letsencrypt/live/5starhealth.care/certificate.pfx", "Sirius880108");
    });
});
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedProto;
    // Set known networks/proxies if necessary
    options.KnownNetworks.Clear();
    options.KnownProxies.Clear();
});
builder.Services.AddHttpClient();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Tiempo de expiración de la sesión
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
builder.Services.AddDataProtection()
    .PersistKeysToDbContext<AppDbContext>();

builder.Services.AddControllersWithViews();

builder.Services.AddTransient<IFileUploadService, FileUploadLocalService>();
builder.WebHost.UseUrls("http://localhost:5000", "https://localhost:5001");

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

app.UseMiddleware<ReverseProxyMiddleware>();
//app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();
app.UseSession();
//app.UseEndpoints(endpoints =>
//{
//    endpoints.MapGet("/login", async context =>
//    {
//        // URL de autenticación de Auth0
//        var authUrl = $"https://{builder.Configuration["Auth0:Domain"]}/authorize?" +
//                      $"response_type=code&" +
//                      $"client_id={builder.Configuration["Auth0:ClientId"]}&" +
//                      $"redirect_uri={Uri.EscapeDataString("https://localhost:7095/callback")}&" +
//                      $"scope=openid%20profile%20email" +  // Asegúrate de que los espacios en 'scope' están codificados como %20
//                      $"audience={Uri.EscapeDataString(builder.Configuration["Auth0:Audience"])}&" +
//                      $"state={Uri.EscapeDataString(generateState())}"; // Función para generar un valor de 'state'
//        context.Response.Redirect(authUrl);
//    });

    

//    endpoints.MapGet("/callback", async context =>
//    {
//        var code = context.Request.Query["code"].ToString();
//        if (string.IsNullOrEmpty(code))
//        {
//            context.Response.Redirect("/Error");
//            return;
//        }

//        // Intercambio del código por un token de acceso
//        var client = context.RequestServices.GetRequiredService<IHttpClientFactory>().CreateClient();
//        var tokenResponse = await client.PostAsync($"https://{builder.Configuration["Auth0:Domain"]}/oauth/token", new FormUrlEncodedContent(new[]
//        {
//        new KeyValuePair<string, string>("grant_type", "authorization_code"),
//        new KeyValuePair<string, string>("client_id", builder.Configuration["Auth0:ClientId"]),
//        new KeyValuePair<string, string>("client_secret", builder.Configuration["Auth0:ClientSecret"]),
//        new KeyValuePair<string, string>("code", code),
//        new KeyValuePair<string, string>("redirect_uri", "https://localhost:7095/callback")
//        }));

//        if (tokenResponse.IsSuccessStatusCode)
//        {
//            var tokenJson = await tokenResponse.Content.ReadAsStringAsync();
//            Console.WriteLine("Token Json is " + tokenJson);
//            var tokens = JsonConvert.DeserializeObject<ResponseToken>(tokenJson);
//            var accessToken = tokens.AccessToken;
//            Console.WriteLine("Token Access is " + tokenJson);

//            //Session
//            context.Session.SetString("AccessToken", accessToken);

//            //Cookie
//            var cookieOptions = new CookieOptions
//            {
//                HttpOnly = true, // Importante para la seguridad, previene el acceso a la cookie a través de JavaScript
//                Secure = true, // Asegura que la cookie se envíe solo a través de HTTPS
//                Expires = DateTime.UtcNow.AddHours(1) // Configura una expiración para la cookie
//            };
//            context.Response.Cookies.Append("AccessToken", accessToken, cookieOptions);
//        }
//        else
//        {
//            context.Response.Redirect("/Error");
//            return;
//        }

//        context.Response.Redirect("/Success");
//    });
//});
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
static string generateState()
{
    var randomBytes = new byte[32];
    using (var rng = RandomNumberGenerator.Create())
    {
        rng.GetBytes(randomBytes);
    }
    return Convert.ToBase64String(randomBytes);
}
