using Blueprint.API.Configuration;
using Blueprint.API.Logic.Helpers;
using Blueprint.API.Logic.UserLogic;
using Blueprint.API.Logic.Validation;
using Blueprint.API.Repository.AuthRepository.Commands;
using Blueprint.API.Repository.AuthRepository.Queries;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File(path: $"Logs\\{DateTime.Now:yyyy-MM-dd}\\log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// Add services
builder.Services.AddControllers();
builder.Services.AddOpenApiWithJwtAuth();
builder.Services.AddHealthChecks().AddCheck("self", () => HealthCheckResult.Healthy(), tags: ["ready"]);
builder.Services.AddDatabaseServices(builder.Configuration);
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddValidationServices();

builder.Services.AddScoped<IAuthQueryRepository, AuthQueryRepository>();
builder.Services.AddScoped<IAuthCommandRepository, AuthCommandRepository>();
builder.Services.AddScoped<IAuthLogic, AuthLogic>();
builder.Services.AddSingleton<IPasswordVerifier, PasswordVerifier>();
builder.Services.AddSingleton<ITokenProvider, TokenProvider>();

var app = builder.Build();

// Configure middleware pipeline
app.UseOpenApiInDevelopment();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();