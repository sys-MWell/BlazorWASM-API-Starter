using Blazor.Web.Domain.Auth;
using Blazor.Web.Domain.Validation;
using Blazor.Web.Configuration;
using Blazor.Web.Logic.Services.Validation;
using Blazor.Web.Logic.User;
using Blazor.Web.Logic.Auth;
using Blazor.Web.Auth;
using Blazor.Web.Repository.User;
using Microsoft.AspNetCore.Components.Authorization;
using Serilog;

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File(path: $"Logs/{DateTime.Now:yyyy-MM-dd}/log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog();

// Authentication/Authorization
builder.Services.AddJwtAuthentication(builder.Configuration);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveWebAssemblyComponents()
    .AddInteractiveServerComponents();

// Provide cascading auth state for components that use <CascadingAuthenticationState>
builder.Services.AddCascadingAuthenticationState();

// API services
builder.Services.AddApiServices(builder.Configuration);

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

