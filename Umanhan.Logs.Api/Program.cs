using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using Umanhan.Logs.Api;
using Umanhan.Logs.Api.Endpoints;
using Umanhan.Models;
using Umanhan.Models.Attributes;
using Umanhan.Models.Models;
using Umanhan.Repositories;
using Umanhan.Repositories.Interfaces;
using Umanhan.Services;
using Umanhan.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);
builder.Logging.ClearProviders();
builder.Logging.AddConsole(); // for AWS CloudWatch
builder.Services.AddAWSLambdaHosting(LambdaEventSource.HttpApi); // for AWS Lambda

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMemoryCache();

builder.Services.AddDbContextPool<UmanhanDbContext>(options =>
    options.UseNpgsql(Environment.GetEnvironmentVariable("CONNECTION_STRING"), o =>
    {
        // execution strategy
        o.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
        o.CommandTimeout(10);
    }));

// repositories
builder.Services.AddScoped(typeof(IRepository<>), typeof(UmanhanRepository<>));
builder.Services.AddScoped<IChangeLogRepository, ChangeLogRepository>();

// services
builder.Services.AddScoped<RolePermissionService>();
builder.Services.AddScoped<IUserContextService, UserContextService>();
builder.Services.AddScoped<IPermissionService, PermissionService>();
builder.Services.AddScoped<IChangeLogService, ChangeLogService>();

// endpoints
builder.Services.AddScoped<ChangeLogEndpoints>();

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
               RoleClaimType = "cognito:groups",
               NameClaimType = ClaimTypes.Email
           };
           options.MapInboundClaims = true;
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

app.Run();