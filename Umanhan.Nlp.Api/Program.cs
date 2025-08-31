using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.IdentityModel.Tokens;
using OpenAI;
using OpenAI.Chat;
using Umanhan.Models.Models;
using Umanhan.Nlp.Api.Endpoints;
using Umanhan.Services.Interfaces;
using Umanhan.Services;
using Umanhan.Models.Attributes;
using Umanhan.Repositories.Interfaces;
using Umanhan.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Umanhan.Nlp.Api;
using Umanhan.Models;

var builder = WebApplication.CreateBuilder(args);
builder.Logging.ClearProviders();
builder.Logging.AddConsole(); // for AWS CloudWatch
builder.Services.AddAWSLambdaHosting(LambdaEventSource.HttpApi); // for AWS Lambda

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMemoryCache();

// Add services to the container.
builder.Services.AddSingleton<HttpClient>();
builder.Services.AddSingleton(provider =>
{
    var apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY") ?? throw new InvalidOperationException("Missing environment variable: OPENAI_API_KEY");
    return new OpenAIClient(apiKey);
});

builder.Services.AddDbContextPool<UmanhanDbContext>(options =>
    options.UseNpgsql(Environment.GetEnvironmentVariable("CONNECTION_STRING"), o =>
    {
        // execution strategy
        o.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
    }));

// repositories
builder.Services.AddScoped(typeof(IRepository<>), typeof(UmanhanRepository<>));
builder.Services.AddScoped<RolePermissionService>();
builder.Services.AddScoped<IUserContextService, UserContextService>();
builder.Services.AddScoped<IPermissionService, PermissionService>();

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// endpoints
builder.Services.AddScoped<NlpEndpoints>();

var cognitoAuthority = Environment.GetEnvironmentVariable("COGNITO_AUTHORITY") ?? throw new InvalidOperationException("Missing environment variable: COGNITO_AUTHORITY");
var cognitoAudience = Environment.GetEnvironmentVariable("COGNITO_AUDIENCE") ?? throw new InvalidOperationException("Missing environment variable: COGNITO_AUDIENCE");

// Configure CORS
string[] allowedDomains = Environment.GetEnvironmentVariable("ALLOWED_ORIGINS")?.Split(",") ?? [];

string apiPathBase = Environment.GetEnvironmentVariable("API_PATH_BASE") ?? throw new InvalidOperationException("Missing environment variable: API_PATH_BASE");

builder.Services.AddOutputCache(); // Enables caching
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

builder.Services.AddHttpContextAccessor();
//builder.Services.AddAuthorization();
builder.Services.AddAuthorization(options => {
    options.AddPolicy("Permission", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.Requirements.Add(new PermissionRequirement());
    });
});

var app = builder.Build();

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

var chatGroup = app.MapGroup("/api/chat");
chatGroup.MapPost("/weather-analysis", (NlpEndpoints endpoint, OpenAIClient client,
    IOutputCacheStore cache, CancellationToken cancellationToken, [FromBody] UserChatRequest request)
    => endpoint.AnalyzeWeatherDataAsync(client, cache, request, cancellationToken))
    .WithMetadata(new RequiresPermissionAttribute("Farm.Full"))
    .RequireAuthorization("Permission");

chatGroup.MapPost("/weather-headline", (NlpEndpoints endpoint, OpenAIClient client,
    IOutputCacheStore cache, CancellationToken cancellationToken, [FromBody] ForecastDailyWeather request)
    => endpoint.GenerateWeatherHeadlineAsync(client, cache, request, cancellationToken))
    .WithMetadata(new RequiresPermissionAttribute("Farm.Full"))
    .RequireAuthorization("Permission");

chatGroup.MapPost("/generate-sql", (NlpEndpoints endpoint, OpenAIClient client,
    IOutputCacheStore cache, CancellationToken cancellationToken, [FromBody] GenerateSqlRequest request)
    => endpoint.GeneratePostgreSqlQueryAsync(client, cache, request, cancellationToken))
    .WithMetadata(new RequiresPermissionAttribute("Farm.Full"))
    .RequireAuthorization("Permission");

chatGroup.MapPost("/analyze-data", (NlpEndpoints endpoint, OpenAIClient client,
    IOutputCacheStore cache, CancellationToken cancellationToken, [FromBody] AnalyzeDataRequest request)
    => endpoint.AnalyzeDataAsync(client, cache, request, cancellationToken))
    .WithMetadata(new RequiresPermissionAttribute("Farm.Full"))
    .RequireAuthorization("Permission");

chatGroup.MapPost("/analyze-data/custom", (NlpEndpoints endpoint, OpenAIClient client,
    [FromBody] AnalyzeDataRequest request)
    => endpoint.AnalyzeDataAsync(client, request))
    .WithMetadata(new RequiresPermissionAttribute("Farm.Full"))
    .RequireAuthorization("Permission");

app.Run();