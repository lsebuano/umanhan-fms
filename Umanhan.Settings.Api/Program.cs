using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;
using Umanhan.Models;
using Umanhan.Models.Attributes;
using Umanhan.Models.Dtos;
using Umanhan.Models.Models;
using Umanhan.Models.Validators;
using Umanhan.Repositories;
using Umanhan.Repositories.Interfaces;
using Umanhan.Services;
using Umanhan.Services.Interfaces;
using Umanhan.Settings.Api;
using Umanhan.Settings.Api.Endpoints;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMemoryCache();
builder.Services.AddOutputCache();

// Add services to the container.
builder.Services.AddValidatorsFromAssemblyContaining<SystemSettingValidator>();

builder.Services.AddDbContextPool<UmanhanDbContext>(options =>
    options.UseNpgsql(Environment.GetEnvironmentVariable("CONNECTION_STRING"), o =>
    {
        // execution strategy
        o.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
        o.CommandTimeout(10);
    }));

//// Redis (Valkey) connection
//builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
//{
//    var config = new ConfigurationOptions
//    {
//        EndPoints = { Environment.GetEnvironmentVariable("AWS_VALKEY_ENDPOINT") },
//        Ssl = Convert.ToBoolean(Environment.GetEnvironmentVariable("AWS_VALKEY_SSL")), // Enable this only if your ElastiCache Valkey has in-transit encryption
//        //Password = Environment.GetEnvironmentVariable("AWS_VALKEY_ENDPOINT_PASSWORD"), // optional
//        AbortOnConnectFail = false,
//        ConnectRetry = 3,
//        ConnectTimeout = 5000,
//    };
//    return ConnectionMultiplexer.Connect(config);
//});
//builder.Services.AddSingleton<ICacheService, RedisCacheService>();

// repositories
builder.Services.AddScoped(typeof(IRepository<>), typeof(UmanhanRepository<>));
builder.Services.AddScoped<ISystemSettingRepository, SystemSettingRepository>();

// validators
builder.Services.AddScoped<IValidator<SystemSettingDto>, SystemSettingValidator>();

// services
builder.Services.AddScoped<IUserContextService, UserContextService>();
builder.Services.AddScoped<IPermissionService, PermissionService>();
builder.Services.AddScoped<RolePermissionService>();
builder.Services.AddScoped<SystemSettingService>();

// endpoints
builder.Services.AddScoped<SystemSettingEndpoints>();

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

var cognitoAuthority = Environment.GetEnvironmentVariable("COGNITO_AUTHORITY") ?? throw new InvalidOperationException("Missing environment variable: COGNITO_AUTHORITY");
var cognitoAudience = Environment.GetEnvironmentVariable("COGNITO_AUDIENCE") ?? throw new InvalidOperationException("Missing environment variable: COGNITO_AUDIENCE");

// Configure CORS
string[] allowedDomains = Environment.GetEnvironmentVariable("ALLOWED_ORIGINS")?.Split(",") ?? [];

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
builder.Services.AddAuthorization(options => {
    options.AddPolicy("Permission", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.Requirements.Add(new PermissionRequirement());
    });
});

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

var settingGroup = app.MapGroup("/api/system-settings");
settingGroup.MapGet("/", (SystemSettingEndpoints endpoint) => endpoint.GetAllSystemSettingsAsync()).RequireAuthorization();
settingGroup.MapGet("/{id}", (SystemSettingEndpoints endpoint, Guid id) => endpoint.GetSystemSettingByIdAsync(id)).RequireAuthorization();
settingGroup.MapGet("/name/{name}", (SystemSettingEndpoints endpoint, string name) => endpoint.GetSystemSettingByNameAsync(name)).RequireAuthorization();

settingGroup.MapPost("/", (SystemSettingEndpoints endpoint, SystemSettingDto setting) => endpoint.CreateSystemSettingAsync(setting))
    .WithMetadata(new RequiresPermissionAttribute("Masterdata.Write"))
    .RequireAuthorization("Permission");

settingGroup.MapPut("/{id}", (SystemSettingEndpoints endpoint, Guid id, SystemSettingDto setting) => endpoint.UpdateSystemSettingAsync(id, setting))
    .WithMetadata(new RequiresPermissionAttribute("Masterdata.Write"))
    .RequireAuthorization("Permission");

settingGroup.MapDelete("/{id}", (SystemSettingEndpoints endpoint, Guid id) => endpoint.DeleteSystemSettingAsync(id))
    .WithMetadata(new RequiresPermissionAttribute("Masterdata.Write"))
    .RequireAuthorization("Permission");

app.Run();
