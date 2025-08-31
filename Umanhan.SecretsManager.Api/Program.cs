using Amazon.SecretsManager;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;
using Umanhan.Models;
using Umanhan.Models.Attributes;
using Umanhan.Models.Models;
using Umanhan.Repositories;
using Umanhan.Repositories.Interfaces;
using Umanhan.SecretsManager.Api;
using Umanhan.SecretsManager.Api.Endpoints;
using Umanhan.Services;
using Umanhan.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

//Console.WriteLine($"Umanhan.SecretsManager.Api:1");

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMemoryCache();
builder.Services.AddOutputCache();

builder.Services.AddDbContextPool<UmanhanDbContext>(options =>
    options.UseNpgsql(Environment.GetEnvironmentVariable("CONNECTION_STRING"), o =>
    {
        // execution strategy
        o.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
        o.CommandTimeout(10);
    }));

//Console.WriteLine($"Umanhan.SecretsManager.Api:2");
//// Redis (Valkey) connection
//builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
//{
//    try
//    {
//        var config = new ConfigurationOptions
//        {
//            EndPoints = { Environment.GetEnvironmentVariable("AWS_VALKEY_ENDPOINT") },
//            Ssl = Convert.ToBoolean(Environment.GetEnvironmentVariable("AWS_VALKEY_SSL")), // Enable this only if your ElastiCache Valkey has in-transit encryption
//                                                                                           //Password = Environment.GetEnvironmentVariable("AWS_VALKEY_ENDPOINT_PASSWORD"), // optional
//            AbortOnConnectFail = false,
//            ConnectRetry = 3,
//            ConnectTimeout = 5000,
//        };
//        return ConnectionMultiplexer.Connect(config);
//    }
//    catch (Exception ex)
//    {
//        Console.WriteLine($"Umanhan.SecretsManager.Api:Redis init failed: {ex.Message}");
//        throw;
//    }
//});
//Console.WriteLine($"Umanhan.SecretsManager.Api:3");
//builder.Services.AddSingleton<ICacheService, RedisCacheService>();

string awsRegion = Environment.GetEnvironmentVariable("REGION") ?? throw new InvalidOperationException("Missing environment variable: REGION");
string awsSecretsName = Environment.GetEnvironmentVariable("AWS_SECRETS_NAME") ?? throw new InvalidOperationException("Missing environment variable: AWS_SECRETS_NAME");

//Console.WriteLine($"Umanhan.SecretsManager.Api:4");
//builder.Configuration.AddAmazonSecretsManager(awsRegion, awsSecretsName, TimeSpan.FromSeconds(15));
//builder.Services.Configure<WebAppSetting>(builder.Configuration)
//    .AddSingleton(resolver => resolver.GetRequiredService<IOptionsMonitor<WebAppSetting>>().CurrentValue);

//builder.Services.AddAWSService<IAmazonSecretsManager>(); // optional
builder.Services.AddAmazonSecretsManagerLazy(region: awsRegion, secretName: awsSecretsName);

//Console.WriteLine($"Umanhan.SecretsManager.Api:5");
// repositories
builder.Services.AddScoped(typeof(IRepository<>), typeof(UmanhanRepository<>));

// services
builder.Services.AddScoped<IUserContextService, UserContextService>();
builder.Services.AddScoped<IPermissionService, PermissionService>();
builder.Services.AddScoped<RolePermissionService>();

// endpoints
builder.Services.AddScoped<SecretsManagerEndpoints>();

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

//Console.WriteLine($"Umanhan.SecretsManager.Api:6");
var cognitoAuthority = Environment.GetEnvironmentVariable("COGNITO_AUTHORITY");
var cognitoAudience = Environment.GetEnvironmentVariable("COGNITO_AUDIENCE");

// Configure CORS
string[] allowedDomains = Environment.GetEnvironmentVariable("ALLOWED_ORIGINS")?.Split(",") ?? [];
string apiPathBase = Environment.GetEnvironmentVariable("API_PATH_BASE") ?? throw new InvalidOperationException("Missing environment variable: API_PATH_BASE");

//Console.WriteLine($"Umanhan.SecretsManager.Api:7");
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

//Console.WriteLine($"Umanhan.SecretsManager.Api:8");
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

//Console.WriteLine($"Umanhan.SecretsManager.Api:9");
builder.Services.AddScoped<IAuthorizationHandler, PermissionHandler>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Permission", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.Requirements.Add(new PermissionRequirement());
    });
});

//Console.WriteLine($"Umanhan.SecretsManager.Api:10");
builder.Logging.ClearProviders();
builder.Logging.AddConsole(); // for AWS CloudWatch
builder.Services.AddAWSLambdaHosting(LambdaEventSource.HttpApi); // for AWS Lambda

var app = builder.Build();
app.UseRouting();

app.UseHttpsRedirection();
app.UseCors("CORSPolicy");
app.UseAuthentication();
app.UseAuthorization();
app.UseOutputCache();

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

//Console.WriteLine($"Umanhan.SecretsManager.Api:11");
var secretsGroup = app.MapGroup("/api/secrets");
//secretsGroup.MapGet("/ping", () => Results.Ok("pong"));

//secretsGroup.MapGet("/", (WebAppSetting settings) => Results.Json(settings));
//secretsGroup.MapGet("/", (HttpContext context, SecretsManagerEndpoints endpoint) => endpoint.GetUmanhanAppSecretsAsync(context));
//secretsGroup.MapGet("/google-maps-loader.js", (HttpContext context, SecretsManagerEndpoints endpoint) => endpoint.GetGoogleMapsLoader(context));
//secretsGroup.MapGet("/google-maps-api-key", (HttpContext context, SecretsManagerEndpoints endpoint) => endpoint.GetGoogleMapsApiKey(context));

secretsGroup.MapGet("/", async ([FromServices] ISecretsService secretsService) =>
{
    try
    {
        var settings = await secretsService.GetKeyValuesAsync();
        return Results.Json(settings);
    }
    catch (Exception ex)
    {
        //Console.WriteLine($"Umanhan.SecretsManager.Api:11:ERROR:::{ex.Message}");
        return Results.Json(new Dictionary<string, string>());
    }
});

secretsGroup.MapGet("/fresh", ([FromServices] SecretsManagerEndpoints endpoint) => endpoint.GetUmanhanAppSecretsAsync()).RequireAuthorization();

secretsGroup.MapPut("/", ([FromServices] SecretsManagerEndpoints endpoint, KeyValuePair<string, string> value) => endpoint.UpdateSecretValueAsync(value))
    .WithMetadata(new RequiresPermissionAttribute("Farm.Write"))
    .RequireAuthorization("Permission");

//Console.WriteLine($"Umanhan.SecretsManager.Api:12");
app.Run();