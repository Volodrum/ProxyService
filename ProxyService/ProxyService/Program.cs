using ProxyService.Interfaces;
using ProxyService.Services;
using Serilog;
using System.IO;
using IdentityServer4;
using IdentityServer4.Models;
using ProxyService.Models;
using ProxyService.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using IdentityServer4.Validation;

var builder = WebApplication.CreateBuilder(args);

var host = WebApplication.CreateBuilder(args).Build();
using(var scope = host.Services.CreateScope())
{
    var serviceProvider = scope.ServiceProvider;
    try
    {
        var context = serviceProvider.GetRequiredService<AuthDbContext>();
        DbInitializer.Initialize(context);
    }
    catch (Exception exception)
    {
        var logger = serviceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogError(exception, "An error occured while app initialization");
    }
}

// Add services to the container.
builder.Services.AddRazorPages();

//Add Identity Server
var connectionString = builder.Configuration.GetValue<string>("DbConnection");

builder.Services.AddDbContext<AuthDbContext>(options =>
{
    options.UseSqlite(connectionString);
});
builder.Services.AddIdentity<AppUser, IdentityRole>(config =>
{
    config.Password.RequiredLength = 4;
    config.Password.RequireDigit = false;
    config.Password.RequireNonAlphanumeric = false;
    config.Password.RequireUppercase = false;
})
    .AddEntityFrameworkStores<AuthDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddIdentityServer()
    .AddAspNetIdentity<AppUser>()
    .AddInMemoryApiResources(Configuration.ApiResources)
    .AddInMemoryIdentityResources(Configuration.IdentityResources)
    .AddInMemoryApiScopes(Configuration.ApiScopes)
    .AddInMemoryClients(Configuration.Clients)
    .AddDeveloperSigningCredential();

builder.Services.ConfigureApplicationCookie(config =>
{
    config.Cookie.Name = "ProxyService.Cookie";
    config.LoginPath = "/Auth/Login";
    config.LogoutPath = "/Auth/Logout";
});

//Add support to logging with SERILOG
builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration));

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAuthentication(config =>
{
    config.DefaultAuthenticateScheme =
        JwtBearerDefaults.AuthenticationScheme;
    config.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer("Bearer", options =>
    {
        options.Authority = "https://localhost:44355/";
        options.Audience = "ProxyWebApi";
        options.RequireHttpsMetadata = false;
    });


// Реєструємо HttpClient та сервіси
builder.Services.AddHttpClient();
builder.Services.AddScoped<IUserService, UserService>(); // Реєструємо наш сервіс

// Додаємо HttpClient в DI-контейнер
builder.Services.AddHttpClient();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseStaticFiles();

//Adding identity server
app.UseIdentityServer();

var env = app.Environment;
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(env.ContentRootPath, "Styles")),
    RequestPath = "/styles"
});

//Add support to logging request with SERILOG
app.UseSerilogRequestLogging();

app.UseRouting();

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.MapRazorPages();

app.Run();
