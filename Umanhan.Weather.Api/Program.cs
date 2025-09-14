using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Diagnostics;
using Umanhan.Models;
using Umanhan.Models.Attributes;
using Umanhan.Models.Models;
using Umanhan.Repositories;
using Umanhan.Repositories.Interfaces;
using Umanhan.Services;
using Umanhan.Services.Interfaces;
using Umanhan.Shared;
using Umanhan.Weather.Api;
using Umanhan.Weather.Api.Endpoints;

var builder = WebApplication.CreateBuilder(args);
builder.Logging.ClearProviders();
builder.Logging.AddConsole(); // for AWS CloudWatch
builder.Services.AddAWSLambdaHosting(LambdaEventSource.HttpApi); // for AWS Lambda

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpContextAccessor();
builder.Services.AddMemoryCache();

builder.Services.AddDbContextPool<UmanhanDbContext>(options =>
    options.UseNpgsql(Environment.GetEnvironmentVariable("CONNECTION_STRING") ?? throw new InvalidOperationException("Missing CONNECTION_STRING"), o =>
    {
        // execution strategy
        o.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
    })
    .LogTo(Console.WriteLine, new[] { DbLoggerCategory.Database.Command.Name }, LogLevel.Information)
    .EnableSensitiveDataLogging()
    .EnableDetailedErrors()
);

// repositories
builder.Services.AddScoped(typeof(IRepository<>), typeof(UmanhanRepository<>));
// Add services to the container.
builder.Services.AddScoped<IUserContextService, UserContextService>();
builder.Services.AddScoped<IPermissionService, PermissionService>();
builder.Services.AddScoped<RolePermissionService>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<WeatherForecastService>();

// endpoints
builder.Services.AddScoped<WeatherEndpoints>();

var cognitoAuthority = Environment.GetEnvironmentVariable("COGNITO_AUTHORITY") ?? throw new InvalidOperationException("Missing environment variable: COGNITO_AUTHORITY");
var cognitoAudience = Environment.GetEnvironmentVariable("COGNITO_AUDIENCE") ?? throw new InvalidOperationException("Missing environment variable: COGNITO_AUDIENCE");

// Configure CORS
string[] allowedDomains = Environment.GetEnvironmentVariable("ALLOWED_ORIGINS")?.Split(",") ?? [];

string apiPathBase = Environment.GetEnvironmentVariable("API_PATH_BASE") ?? throw new InvalidOperationException("Missing environment variable: API_PATH_BASE");

builder.Services.AddOutputCache(); // Enables caching
//builder.Services.AddHttpClient("OpenMeteo");
builder.Services.AddHttpClient("OpenWeather");
builder.Services.AddCors(item =>
{
    item.AddPolicy("CORSPolicy", builder =>
    {
        builder.WithOrigins(allowedDomains)
               .AllowAnyMethod()
               .AllowAnyHeader()
               .AllowCredentials();
    });
});
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = cognitoAuthority;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidIssuer = cognitoAuthority,
            ValidateIssuerSigningKey = true,
            ValidateIssuer = true,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidAudience = cognitoAudience,
            RoleClaimType = "cognito:groups"
        };
    });

builder.Services.AddScoped<IAuthorizationHandler, PermissionHandler>();

//builder.Services.AddAuthorization();
builder.Services.AddAuthorization(options => {
    options.AddPolicy("Permission", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.Requirements.Add(new PermissionRequirement());
    });
});

var app = builder.Build();

// Resolve IHttpContextAccessor from final DI container
var httpContextAccessor = app.Services.GetRequiredService<IHttpContextAccessor>();
var connectionStringLogs = Environment.GetEnvironmentVariable("CONNECTION_STRING_LOGS")
    ?? throw new InvalidOperationException("Missing CONNECTION_STRING_LOGS");

// Create EF Query Logger
var efQueryLogger = new EfCoreQueryLogger(connectionStringLogs, httpContextAccessor);

// Subscribe to the **global EF Core listener**, not a new one
var diagnosticObserver = new EfCoreDiagnosticObserver(efQueryLogger);
DiagnosticListener.AllListeners.Subscribe(diagnosticObserver);

// Configure the HTTP request pipeline.
app.UsePathBase(apiPathBase);
app.UseRouting();

app.UseOutputCache();
app.UseHttpsRedirection();
app.UseCors("CORSPolicy");
app.UseAuthentication();
app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.Use(async (context, next) =>
{
    context.Response.Headers["Cross-Origin-Resource-Policy"] = "cross-origin";
    await next();
});

string provider = Environment.GetEnvironmentVariable("WEATHER_PROVIDER") ?? throw new InvalidOperationException("Missing environment variable: WEATHER_PROVIDER");
string openWeatherApiKey = Environment.GetEnvironmentVariable("WEATHER_API_KEY") ?? throw new InvalidOperationException("Missing environment variable: WEATHER_API_KEY");

var weatherGroup = app.MapGroup("/api/weather");
weatherGroup.MapGet("/{lat}/{lng}", (HttpContext context, WeatherEndpoints endpoint, double lat, double lng,
    IHttpClientFactory httpClientFactory, IOutputCacheStore cache, CancellationToken cancellationToken) 
    => endpoint.GetWeatherDataAsync(context, provider, openWeatherApiKey, lat, lng, httpClientFactory, cache, cancellationToken))
    .WithMetadata(new RequiresPermissionAttribute("Weather.Read"))
    .RequireAuthorization("Permission");

weatherGroup.MapGet("/forecast/{lat}/{lng}", (HttpContext context, WeatherEndpoints endpoint, double lat, double lng,
    IHttpClientFactory httpClientFactory, IOutputCacheStore cache, CancellationToken cancellationToken)
    => endpoint.GetWeatherForecastDataAsync(context, openWeatherApiKey, lat, lng, httpClientFactory, cache, cancellationToken))
    .WithMetadata(new RequiresPermissionAttribute("Weather.Read"))
    .RequireAuthorization("Permission");

app.Run();