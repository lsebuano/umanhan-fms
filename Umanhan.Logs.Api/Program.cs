using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Diagnostics;
using System.Security.Claims;
using Umanhan.Logs.Api;
using Umanhan.Logs.Api.Endpoints;
using Umanhan.Models;
using Umanhan.Models.Attributes;
using Umanhan.Models.LoggerEntities;
using Umanhan.Models.Models;
using Umanhan.Repositories;
using Umanhan.Repositories.Interfaces;
using Umanhan.Repositories.LoggerContext;
using Umanhan.Repositories.LoggerContext.Interfaces;
using Umanhan.Services;
using Umanhan.Services.Interfaces;
using Umanhan.Shared;

var builder = WebApplication.CreateBuilder(args);
builder.Logging.ClearProviders();
builder.Logging.AddConsole(); // for AWS CloudWatch
builder.Services.AddAWSLambdaHosting(LambdaEventSource.HttpApi); // for AWS Lambda

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMemoryCache();
builder.Services.AddHttpContextAccessor();

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

builder.Services.AddDbContextPool<UmanhanLoggerDbContext>(options =>
    options.UseNpgsql(Environment.GetEnvironmentVariable("CONNECTION_STRING_LOGS") ?? throw new InvalidOperationException("Missing CONNECTION_STRING_LOGS"), o =>
    {
        // execution strategy
        o.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
    })
    .LogTo(Console.WriteLine, new[] { DbLoggerCategory.Database.Command.Name }, LogLevel.Information)
);

// repositories
builder.Services.AddScoped(typeof(IRepository<>), typeof(UmanhanRepository<>));
builder.Services.AddScoped(typeof(IRepository<>), typeof(UmanhanLoggerRepository<>));
builder.Services.AddScoped<IChangeLogRepository, ChangeLogRepository>();
builder.Services.AddScoped<IQueryLogRepository, QueryLogRepository>();

// services
builder.Services.AddScoped<RolePermissionService>();
builder.Services.AddScoped<IUserContextService, UserContextService>();
builder.Services.AddScoped<IPermissionService, PermissionService>();
builder.Services.AddScoped<IChangeLogService, ChangeLogService>();
builder.Services.AddScoped<IQueryLogService, QueryLogService>();

// endpoints
builder.Services.AddScoped<ChangeLogEndpoints>();
builder.Services.AddScoped<QueryLogEndpoints>();

// unit of work
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<ILoggerUnitOfWork, LoggerUnitOfWork>();

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
               RoleClaimType = "cognito:groups",
               NameClaimType = ClaimTypes.Email
           };
           options.MapInboundClaims = true;
       });


builder.Services.AddScoped<IAuthorizationHandler, PermissionHandler>();

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

// Configure the HTTP request pipelinec.
app.UseRouting();

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

var changeLogGroup = app.MapGroup("/api/change-logs");
changeLogGroup.MapGet("/date/{date}", (ChangeLogEndpoints endpoint, DateTime date) => endpoint.GetChangeLogsAsync(date))
    .WithMetadata(new RequiresPermissionAttribute("Log.Read"))
    .RequireAuthorization("Permission");

changeLogGroup.MapGet("/{id}", (ChangeLogEndpoints endpoint, Guid id) => endpoint.GetChangeLogByIdAsync(id))
    .WithMetadata(new RequiresPermissionAttribute("Log.Read"))
    .RequireAuthorization("Permission");


var queryLogGroup = app.MapGroup("/api/query-logs");
queryLogGroup.MapGet("/date/{date}", (QueryLogEndpoints endpoint, DateTime date) => endpoint.GetQueryLogsAsync(date))
    .WithMetadata(new RequiresPermissionAttribute("Log.Read"))
    .RequireAuthorization("Permission");

queryLogGroup.MapGet("/{id}", (QueryLogEndpoints endpoint, Guid id) => endpoint.GetQueryLogByIdAsync(id))
    .WithMetadata(new RequiresPermissionAttribute("Log.Read"))
    .RequireAuthorization("Permission");

app.Run();