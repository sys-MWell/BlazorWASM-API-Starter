using Blazor.Web.Domain.Auth;
using Blazor.Web.Domain.Validation;
using Blazor.Web.Domain.Shared;
using Blazor.Web.Logic.Services.Validation;
using Blazor.Web.Logic.User;
using Blazor.Web.Logic.Auth;
using Blazor.Web.Auth;
using Blazor.Web.Repository.User;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File(
        path: $"Logs\\{DateTime.Now:yyyy-MM-dd}\\log-.txt",
        rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// Authentication/Authorization (JWT Bearer)
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    var cfg = builder.Configuration.GetSection("Jwt");
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = cfg["Issuer"],
        ValidateAudience = true,
        ValidAudience = cfg["Audience"],
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(cfg["Key"] ?? string.Empty)),
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveWebAssemblyComponents()
    .AddInteractiveServerComponents();

// Provide cascading auth state for components that use <CascadingAuthenticationState>
builder.Services.AddCascadingAuthenticationState();

// Server-side HttpClient for calling external/internal APIs
builder.Services.AddHttpClient("ApiClient", client =>
{
    var apiUrl = builder.Configuration.GetValue<string>("AppApi:BaseUrl") ?? string.Empty;
    if (!string.IsNullOrWhiteSpace(apiUrl))
    {
        client.BaseAddress = new Uri(apiUrl);
    }
});

// Bind and register ApiSettings for repositories/services that require it
builder.Services.AddSingleton(sp => new ApiSettings
{
    AppApiBaseUrl = builder.Configuration.GetValue<string>("AppApi:BaseUrl") ?? string.Empty
});

// Register application services (validators, repositories, logic)
builder.Services.AddScoped<ITokenStore, InMemoryTokenStore>();
builder.Services.AddScoped<ITokenPersistence, ProtectedSessionTokenPersistence>();
builder.Services.AddScoped<IUserSession, UserSession>();
builder.Services.AddScoped<ILogicValidator, LogicValidator>();
builder.Services.AddScoped<ICredentialValidator, CredentialValidator>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserLogic, UserLogic>();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<Blazor.Web.Components.App>()
    .AddInteractiveWebAssemblyRenderMode()
    .AddInteractiveServerRenderMode()
    .AddAdditionalAssemblies(typeof(Blazor.Web.Client._Imports).Assembly);

app.Run();