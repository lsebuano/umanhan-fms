
using Amazon;
using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;
using Amazon.Runtime;
using Amazon.SimpleEmail;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SendGrid;
using StackExchange.Redis;
using System.Security.Claims;
using Umanhan.Models;
using Umanhan.Models.Attributes;
using Umanhan.Models.Dtos;
using Umanhan.Models.Models;
using Umanhan.Models.Validators;
using Umanhan.Repositories;
using Umanhan.Repositories.Interfaces;
using Umanhan.Services;
using Umanhan.Services.Interfaces;
using Umanhan.Shared;
using Umanhan.UserManager.Api;
using Umanhan.UserManager.Api.Endpoints;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMemoryCache();

//builder.Services.AddAWSService<IAmazonSimpleEmailService>();

// Add services to the container.
// Register FluentValidation
builder.Services.AddValidatorsFromAssemblyContaining<RoleValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<ModuleValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<PermissionValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<RolePermissionValidator>();
builder.Services.AddFluentValidationAutoValidation();

builder.Services.AddDbContextPool<UmanhanDbContext>(options =>
    options.UseNpgsql(Environment.GetEnvironmentVariable("CONNECTION_STRING"), o =>
    {
        // execution strategy
        o.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
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

//builder.Services.AddSingleton<IAuthorizationHandler, RolesOrPermissionsHandler>();
builder.Services.AddSingleton<IAmazonCognitoIdentityProvider>(sp =>
{
    //string accessKey = Environment.GetEnvironmentVariable("AWS_ACCESS_KEY_ID");
    //string secretKey = Environment.GetEnvironmentVariable("AWS_SECRET_ACCESS_KEY");

    //return new AmazonCognitoIdentityProviderClient(new BasicAWSCredentials(accessKey, secretKey), RegionEndpoint.USEast1);

    return new AmazonCognitoIdentityProviderClient(RegionEndpoint.USEast1);
});

builder.Services.AddScoped<UserStateService>(sp =>
{
    var httpContextAccessor = sp.GetRequiredService<IHttpContextAccessor>();
    return new UserStateService(httpContextAccessor.HttpContext?.User ?? new ClaimsPrincipal());
});

//builder.Services.AddSingleton<IAmazonSimpleEmailService, AmazonSimpleEmailServiceClient>();
builder.Services.AddSingleton<ISendGridClient>(sp =>
    new SendGridClient(Environment.GetEnvironmentVariable("SENDGRID_API_KEY")));

// Choose one as the default (you can improve this later)
//builder.Services.AddScoped<IEmailService, SesEmailService>();
builder.Services.AddScoped<IEmailService, SendGridEmailService>();

// repositories
builder.Services.AddScoped(typeof(IRepository<>), typeof(UmanhanRepository<>));
//builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<IModuleRepository, ModuleRepository>();
builder.Services.AddScoped<IPermissionRepository, PermissionRepository>();
builder.Services.AddScoped<IRolePermissionRepository, RolePermissionRepository>();
//builder.Services.AddScoped<IClaimRepository, ClaimRepository>();

// validators
builder.Services.AddScoped<IValidator<ModuleDto>, ModuleValidator>();
builder.Services.AddScoped<IValidator<PermissionDto>, PermissionValidator>();
builder.Services.AddScoped<IValidator<RoleDto>, RoleValidator>();
builder.Services.AddScoped<IValidator<RolePermissionDto>, RolePermissionValidator>();

// services
builder.Services.AddScoped<IUserContextService, UserContextService>();
builder.Services.AddScoped<IPermissionService, PermissionService>();
builder.Services.AddScoped<RolePermissionService>();

builder.Services.AddScoped<RoleService>();
builder.Services.AddScoped<ModuleService>();
//builder.Services.AddScoped<EmailService>();
//builder.Services.AddScoped<UserService>();
//builder.Services.AddScoped<ClaimService>();

// endpoints
builder.Services.AddScoped<ClaimEndpoints>();
builder.Services.AddScoped<ModuleEndpoints>();
builder.Services.AddScoped<PermissionEndpoints>();
builder.Services.AddScoped<RoleEndpoints>();
builder.Services.AddScoped<RolePermissionEndpoints>();
builder.Services.AddScoped<UserEndpoints>();

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

var cognitoAuthority = Environment.GetEnvironmentVariable("COGNITO_AUTHORITY") ?? throw new InvalidOperationException("Missing environment variable: COGNITO_AUTHORITY");
var cognitoAudience = Environment.GetEnvironmentVariable("COGNITO_AUDIENCE") ?? throw new InvalidOperationException("Missing environment variable: COGNITO_AUDIENCE");

// Configure CORS
string[] allowedDomains = Environment.GetEnvironmentVariable("ALLOWED_ORIGINS")?.Split(",") ?? [];

string apiPathBase = Environment.GetEnvironmentVariable("API_PATH_BASE") ?? throw new InvalidOperationException("Missing environment variable: API_PATH_BASE");
string userPoolId = Environment.GetEnvironmentVariable("COGNITO_USERPOOLID") ?? throw new InvalidOperationException("Missing environment variable: COGNITO_USERPOOLID");

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
builder.Services.AddAuthorization(options =>
{
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

// Configure the HTTP request pipeline.
//app.UsePathBase(apiPathBase);
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

var roleGroup = app.MapGroup("/api/user-mgr/roles");
roleGroup.MapGet("/", (RoleEndpoints endpoint) => endpoint.GetAllRolesAsync())
    .WithMetadata(new RequiresPermissionAttribute("User.Read"))
    .RequireAuthorization("Permission");

roleGroup.MapGet("/active", (RoleEndpoints endpoint) => endpoint.GetAllRolesAsync(true))
    .WithMetadata(new RequiresPermissionAttribute("User.Read"))
    .RequireAuthorization("Permission");

roleGroup.MapGet("/except", (UserStateService userState, RoleEndpoints endpoint) => endpoint.GetRolesExceptAsync(userState))
.WithMetadata(new RequiresPermissionAttribute("User.Read"))
.RequireAuthorization("Permission");

roleGroup.MapGet("/cognito", (RoleEndpoints endpoint, IAmazonCognitoIdentityProvider cognitoClient) => endpoint.GetAllGroupsAsync(cognitoClient, userPoolId))
    .WithMetadata(new RequiresPermissionAttribute("User.Read"))
    .RequireAuthorization("Permission");

roleGroup.MapGet("/{id}", (RoleEndpoints endpoint, Guid id) => endpoint.GetRoleByIdAsync(id))
    .WithMetadata(new RequiresPermissionAttribute("User.Read"))
    .RequireAuthorization("Permission");

roleGroup.MapPost("/", (RoleEndpoints endpoint, [FromBody] RoleDto role, IAmazonCognitoIdentityProvider cognitoClient) => endpoint.CreateRoleAsync(cognitoClient, role, userPoolId))
    .WithMetadata(new RequiresPermissionAttribute("User.Write"))
    .RequireAuthorization("Permission");

roleGroup.MapPut("/{id}", (RoleEndpoints endpoint, [FromRoute] Guid id, [FromBody] RoleDto role, IAmazonCognitoIdentityProvider cognitoClient) => endpoint.UpdateRoleAsync(cognitoClient, id, role, userPoolId))
    .WithMetadata(new RequiresPermissionAttribute("User.Write"))
    .RequireAuthorization("Permission");

roleGroup.MapDelete("/{id}", (RoleEndpoints endpoint, [FromQuery] Guid id, [FromQuery] string groupName, IAmazonCognitoIdentityProvider cognitoClient) => endpoint.DeleteRoleAsync(cognitoClient, id, groupName, userPoolId))
    .WithMetadata(new RequiresPermissionAttribute("User.Write"))
    .RequireAuthorization("Permission");

var moduleGroup = app.MapGroup("/api/user-mgr/modules");
moduleGroup.MapGet("/", (ModuleEndpoints endpoint) => endpoint.GetAllModulesAsync())
    .WithMetadata(new RequiresPermissionAttribute("User.Read"))
    .RequireAuthorization("Permission");

moduleGroup.MapGet("/{id}", (ModuleEndpoints endpoint, Guid id) => endpoint.GetModuleByIdAsync(id))
    .WithMetadata(new RequiresPermissionAttribute("User.Read"))
    .RequireAuthorization("Permission");

moduleGroup.MapPost("/", (ModuleEndpoints endpoint, ModuleDto module) => endpoint.CreateModuleAsync(module))
    .WithMetadata(new RequiresPermissionAttribute("User.Write"))
    .RequireAuthorization("Permission");

moduleGroup.MapPut("/{id}", (ModuleEndpoints endpoint, Guid id, ModuleDto module) => endpoint.UpdateModuleAsync(id, module))
    .WithMetadata(new RequiresPermissionAttribute("User.Write"))
    .RequireAuthorization("Permission");

moduleGroup.MapDelete("/{id}", (ModuleEndpoints endpoint, Guid id) => endpoint.DeleteModuleAsync(id))
    .WithMetadata(new RequiresPermissionAttribute("User.Write"))
    .RequireAuthorization("Permission");

var permissionGroup = app.MapGroup("/api/user-mgr/permissions");
permissionGroup.MapGet("/", (PermissionEndpoints endpoint) => endpoint.GetAllPermissionsAsync())
    .WithMetadata(new RequiresPermissionAttribute("User.Read"))
    .RequireAuthorization("Permission");

permissionGroup.MapPost("/", (PermissionEndpoints endpoint, PermissionDto permission) => endpoint.CreatePermissionAsync(permission))
    .WithMetadata(new RequiresPermissionAttribute("User.Write"))
    .RequireAuthorization("Permission");

permissionGroup.MapPut("/{id}", (PermissionEndpoints endpoint, Guid id, PermissionDto permission) => endpoint.UpdatePermissionAsync(id, permission))
    .WithMetadata(new RequiresPermissionAttribute("User.Write"))
    .RequireAuthorization("Permission");

permissionGroup.MapDelete("/{id}", (PermissionEndpoints endpoint, Guid id) => endpoint.DeletePermissionAsync(id))
    .WithMetadata(new RequiresPermissionAttribute("User.Write"))
    .RequireAuthorization("Permission");

var rolePermissionGroup = app.MapGroup("/api/user-mgr/role-permissions");
rolePermissionGroup.MapGet("/", (RolePermissionEndpoints endpoint) => endpoint.GetAllRolePermissionsAsync())
    .WithMetadata(new RequiresPermissionAttribute("User.Read"))
    .RequireAuthorization("Permission");

rolePermissionGroup.MapGet("/role/{id}", (RolePermissionEndpoints endpoint, Guid id) => endpoint.GetPermissionsForRoleAsync(id))
    .WithMetadata(new RequiresPermissionAttribute("User.Read"))
    .RequireAuthorization("Permission");

rolePermissionGroup.MapGet("/{id}", (RolePermissionEndpoints endpoint, Guid id) => endpoint.GetRolePermissionByIdAsync(id))
    .WithMetadata(new RequiresPermissionAttribute("User.Read"))
    .RequireAuthorization("Permission");

rolePermissionGroup.MapPost("/", (RolePermissionEndpoints endpoint, RolePermissionDto role) => endpoint.CreateRolePermissionAsync(role))
    .WithMetadata(new RequiresPermissionAttribute("User.Write"))
    .RequireAuthorization("Permission");

rolePermissionGroup.MapPut("/{id}", (RolePermissionEndpoints endpoint, Guid id, RolePermissionDto role) => endpoint.UpdateRolePermissionAsync(id, role))
    .WithMetadata(new RequiresPermissionAttribute("User.Write"))
    .RequireAuthorization("Permission");

rolePermissionGroup.MapDelete("/{id}", (RolePermissionEndpoints endpoint, Guid id) => endpoint.DeleteRolePermissionAsync(id))
    .WithMetadata(new RequiresPermissionAttribute("User.Write"))
    .RequireAuthorization("Permission");

var userGroup = app.MapGroup("/api/user-mgr/users");
userGroup.MapGet("/", (UserEndpoints endpoint, IAmazonCognitoIdentityProvider cognitoClient) => endpoint.GetAllUsersAsync(cognitoClient, userPoolId))
    .WithMetadata(new RequiresPermissionAttribute("User.Read"))
    .RequireAuthorization("Permission");

userGroup.MapGet("/{email}", (UserEndpoints endpoint, IAmazonCognitoIdentityProvider cognitoClient, IConfiguration config, string email) => endpoint.GetUserByEmailAsync(cognitoClient, email, userPoolId))
    .WithMetadata(new RequiresPermissionAttribute("User.Read"))
    .RequireAuthorization("Permission");

userGroup.MapPost("/", (UserEndpoints endpoint, IAmazonCognitoIdentityProvider cognitoClient, IConfiguration config, UserDto user) => endpoint.CreateCognitoUserAsync(cognitoClient, user, userPoolId))
    .WithMetadata(new RequiresPermissionAttribute("User.Write"))
    .RequireAuthorization("Permission");

userGroup.MapPost("/{username}/groups", (UserEndpoints endpoint, string username, IAmazonCognitoIdentityProvider cognitoClient, IConfiguration config, [FromBody] IEnumerable<string> groups) => endpoint.AddCognitoUserToGroupsAsync(username, cognitoClient, groups, userPoolId))
    .WithMetadata(new RequiresPermissionAttribute("User.Write"))
    .RequireAuthorization("Permission");

userGroup.MapPost("/{username}/force-logout", (UserEndpoints endpoint, string username, IAmazonCognitoIdentityProvider cognitoClient, IConfiguration config) => endpoint.LogoutCognitoUserAsync(username, cognitoClient, userPoolId))
    .WithMetadata(new RequiresPermissionAttribute("User.Full"))
    .RequireAuthorization("Permission");

userGroup.MapPost("/{username}/remove-from/{groupName}", (UserEndpoints endpoint, string username, string groupName, IAmazonCognitoIdentityProvider cognitoClient, IConfiguration config) => endpoint.RemoveCognitoUserFromGroupAsync(username, groupName, cognitoClient, userPoolId))
    .WithMetadata(new RequiresPermissionAttribute("User.Write"))
    .RequireAuthorization("Permission");

userGroup.MapPost("/{username}/disable", (UserEndpoints endpoint, string username, IAmazonCognitoIdentityProvider cognitoClient, IConfiguration config) => endpoint.DisableCognitoUserAsync(username, cognitoClient, userPoolId))
    .WithMetadata(new RequiresPermissionAttribute("User.Write"))
    .RequireAuthorization("Permission");

userGroup.MapPatch("/{username}", (UserEndpoints endpoint, string username, IAmazonCognitoIdentityProvider cognitoClient, IConfiguration config, UserDto user) => endpoint.UpdateCognitoUserDetailsAsync(username, cognitoClient, user, userPoolId))
    .WithMetadata(new RequiresPermissionAttribute("User.Write"))
    .RequireAuthorization("Permission");

var claimsGroup = app.MapGroup("/api/user-mgr/claims");
//claimsGroup.MapGet("/", (IOutputCacheStore cache, CancellationToken cancellationToken, ClaimEndpoints endpoint) => endpoint.GetUserClaimsAsync(cache, cancellationToken)).RequireAuthorization();
claimsGroup.MapGet("/", (ClaimEndpoints endpoint) => endpoint.GetUserClaimsAsync()).RequireAuthorization();

// logout all users under a group. to be called when there's a permission change
userGroup.MapPost("/{groupName}/logout", (UserEndpoints endpoint, string groupName, IAmazonCognitoIdentityProvider cognitoClient, IConfiguration config) => endpoint.LogoutCognitoUsersAsync(groupName, cognitoClient, userPoolId));

app.Run();