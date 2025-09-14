using Amazon.S3;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SendGrid;
using System.Diagnostics;
using Umanhan.Dtos;
using Umanhan.Dtos.HelperModels;
using Umanhan.Dtos.Validators;
using Umanhan.Models;
using Umanhan.Models.Attributes;
using Umanhan.Models.Models;
using Umanhan.Operations.Api;
using Umanhan.Operations.Api.Endpoints;
using Umanhan.Repositories;
using Umanhan.Repositories.Interfaces;
using Umanhan.Services;
using Umanhan.Services.Interfaces;
using Umanhan.Shared;

var builder = WebApplication.CreateBuilder(args);
builder.Logging.ClearProviders();
builder.Logging.AddConsole(); // for AWS CloudWatch
builder.Services.AddAWSLambdaHosting(LambdaEventSource.HttpApi); // for AWS Lambda
builder.Services.AddDefaultAWSOptions(builder.Configuration.GetAWSOptions());
builder.Services.AddAWSService<IAmazonS3>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMemoryCache();
builder.Services.AddHttpContextAccessor();

//builder.Services.AddAWSService<IAmazonSimpleEmailService>();


// Register FluentValidation
builder.Services.AddValidatorsFromAssemblyContaining<FarmActivityValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<FarmActivityExpenseValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<FarmActivityLaborerValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<FarmActivityUsageValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<FarmContractDetailValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<FarmContractValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<FarmCropValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<FarmInventoryValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<FarmProduceInventoryValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<FarmZoneValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<PricingConditionValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<PricingProfileValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<QuotationValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<QuotationDetailValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<FarmActivityPhotoValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<QuotationProductValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<FarmGeneralExpenseValidator>();

builder.Services.AddFluentValidationAutoValidation();

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

//builder.Services.AddSingleton<IAmazonSimpleEmailService, AmazonSimpleEmailServiceClient>();
builder.Services.AddSingleton<ISendGridClient>(sp =>
    new SendGridClient(Environment.GetEnvironmentVariable("SENDGRID_API_KEY")));

// Choose one as the default (you can improve this later)
//builder.Services.AddScoped<IEmailService, SesEmailService>();
builder.Services.AddScoped<IEmailService, SendGridEmailService>();

// repositories
builder.Services.AddScoped(typeof(IRepository<>), typeof(UmanhanRepository<>));
builder.Services.AddScoped<IFarmActivityRepository, FarmActivityRepository>();
builder.Services.AddScoped<IFarmActivityLaborerRepository, FarmActivityLaborerRepository>();
builder.Services.AddScoped<IFarmActivityUsageRepository, FarmActivityUsageRepository>();
builder.Services.AddScoped<IFarmContractDetailRepository, FarmContractDetailRepository>();
builder.Services.AddScoped<IFarmContractSaleRepository, FarmContractSaleRepository>();
builder.Services.AddScoped<IFarmContractRepository, FarmContractRepository>();
builder.Services.AddScoped<IFarmCropRepository, FarmCropRepository>();
builder.Services.AddScoped<IFarmInventoryRepository, FarmInventoryRepository>();
builder.Services.AddScoped<IFarmProduceInventoryRepository, FarmProduceInventoryRepository>();
builder.Services.AddScoped<IFarmTransactionRepository, FarmTransactionRepository>();
builder.Services.AddScoped<IFarmZoneRepository, FarmZoneRepository>();
builder.Services.AddScoped<IPricingConditionRepository, PricingConditionRepository>();
builder.Services.AddScoped<IPricingProfileRepository, PricingProfileRepository>();
builder.Services.AddScoped<IQuotationRepository, QuotationRepository>();
builder.Services.AddScoped<IQuotationDetailRepository, QuotationDetailRepository>();
builder.Services.AddScoped<IFarmActivityPhotoRepository, FarmActivityPhotoRepository>();
builder.Services.AddScoped<IFarmGeneralExpenseRepository, FarmGeneralExpenseRepository>();

// validators
builder.Services.AddScoped<IValidator<FarmActivityDto>, FarmActivityValidator>();
builder.Services.AddScoped<IValidator<FarmActivityLaborerDto>, FarmActivityLaborerValidator>();
builder.Services.AddScoped<IValidator<FarmActivityExpenseDto>, FarmActivityExpenseValidator>();
builder.Services.AddScoped<IValidator<FarmActivityUsageDto>, FarmActivityUsageValidator>();
builder.Services.AddScoped<IValidator<FarmContractDetailDto>, FarmContractDetailValidator>();
builder.Services.AddScoped<IValidator<FarmContractDto>, FarmContractValidator>();
builder.Services.AddScoped<IValidator<FarmContractSaleDto>, FarmContractSaleValidator>();
builder.Services.AddScoped<IValidator<FarmCropDto>, FarmCropValidator>();
builder.Services.AddScoped<IValidator<FarmInventoryDto>, FarmInventoryValidator>();
builder.Services.AddScoped<IValidator<FarmProduceInventoryDto>, FarmProduceInventoryValidator>();
builder.Services.AddScoped<IValidator<FarmTransactionDto>, FarmTransactionValidator>();
builder.Services.AddScoped<IValidator<FarmZoneDto>, FarmZoneValidator>();
builder.Services.AddScoped<IValidator<PricingDto>, PricingConditionValidator>();
builder.Services.AddScoped<IValidator<PricingProfileDto>, PricingProfileValidator>();
builder.Services.AddScoped<IValidator<QuotationDto>, QuotationValidator>();
builder.Services.AddScoped<IValidator<QuotationDetailDto>, QuotationDetailValidator>();
builder.Services.AddScoped<IValidator<FarmActivityPhotoDto>, FarmActivityPhotoValidator>();
builder.Services.AddScoped<IValidator<QuotationProductDto>, QuotationProductValidator>();
builder.Services.AddScoped<IValidator<FarmGeneralExpenseDto>, FarmGeneralExpenseValidator>();

// services
builder.Services.AddScoped<IUserContextService, UserContextService>();
builder.Services.AddScoped<IPermissionService, PermissionService>();
builder.Services.AddScoped<RolePermissionService>();

builder.Services.AddScoped<FarmActivityService>();
builder.Services.AddScoped<FarmActivityLaborerService>();
builder.Services.AddScoped<FarmActivityExpenseService>();
builder.Services.AddScoped<FarmActivityUsageService>();
builder.Services.AddScoped<FarmExpenseService>();
builder.Services.AddScoped<FarmInventoryService>();
builder.Services.AddScoped<FarmProduceInventoryService>();
builder.Services.AddScoped<FarmContractDetailService>();
builder.Services.AddScoped<FarmContractService>();
builder.Services.AddScoped<FarmContractSaleService>();
builder.Services.AddScoped<FarmCropService>();
builder.Services.AddScoped<FarmTransactionService>();
builder.Services.AddScoped<FarmZoneService>();
builder.Services.AddScoped<PricingService>();
builder.Services.AddScoped<PricingProfileService>();
builder.Services.AddScoped<QuotationService>();
//builder.Services.AddScoped<EmailService>();
builder.Services.AddScoped<FarmActivityPhotoService>();
builder.Services.AddScoped<FarmGeneralExpenseService>();
builder.Services.AddScoped<FarmGeneralExpenseReceiptService>();

// endpoints
builder.Services.AddScoped<FarmLaborEndpoints>();
builder.Services.AddScoped<FarmActivityEndpoints>();
builder.Services.AddScoped<FarmExpenseEndpoints>();
builder.Services.AddScoped<FarmUsageEndpoints>();
builder.Services.AddScoped<FarmContractDetailEndpoints>();
builder.Services.AddScoped<FarmContractEndpoints>();
//builder.Services.AddScoped<FarmContractSaleEndpoints>();
builder.Services.AddScoped<FarmCropEndpoints>();
builder.Services.AddScoped<FarmInventoryEndpoints>();
builder.Services.AddScoped<FarmProduceInventoryEndpoints>();
builder.Services.AddScoped<FarmSalesEndpoints>();
builder.Services.AddScoped<FarmZoneEndpoints>();
builder.Services.AddScoped<PricingEndpoints>();
builder.Services.AddScoped<PricingProfileEndpoints>();
builder.Services.AddScoped<QuotationEndpoints>();
builder.Services.AddScoped<FarmActivityPhotoEndpoints>();
builder.Services.AddScoped<FarmGeneralExpenseEndpoints>();
builder.Services.AddScoped<FarmGeneralExpenseReceiptEndpoints>();

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

var cognitoAuthority = Environment.GetEnvironmentVariable("COGNITO_AUTHORITY") ?? throw new InvalidOperationException("Missing environment variable: COGNITO_AUTHORITY");
var cognitoAudience = Environment.GetEnvironmentVariable("COGNITO_AUDIENCE") ?? throw new InvalidOperationException("Missing environment variable: COGNITO_AUDIENCE");

var s3BucketUrl = Environment.GetEnvironmentVariable("PHOTOS_BUCKET_URL") ?? throw new InvalidOperationException("Missing environment variable: PHOTOS_BUCKET_URL");
var photosBucketName = Environment.GetEnvironmentVariable("PHOTOS_BUCKET_NAME") ?? throw new InvalidOperationException("Missing environment variable: PHOTOS_BUCKET_NAME");

// Configure CORS
string[] allowedDomains = Environment.GetEnvironmentVariable("ALLOWED_ORIGINS")?.Split(",") ?? [];

string apiPathBase = Environment.GetEnvironmentVariable("API_PATH_BASE") ?? throw new InvalidOperationException("Missing environment variable: API_PATH_BASE");

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

builder.Services.AddHttpClient();

//builder.Services.AddAuthorization();
builder.Services.AddAuthorization(options =>
{
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

// create email templates. this should be ran only once
//#if DEBUG
//var t = app.Services.GetRequiredService<IAmazonSimpleEmailService>();
//await EmailTemplates.InitializeTemplates(t);
//#endif

// Configure the HTTP request pipelinec.
app.UsePathBase(apiPathBase);
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

var farmZoneGroup = app.MapGroup("/api/farm-zones");
farmZoneGroup.MapGet("/", (FarmZoneEndpoints endpoint) => endpoint.GetAllFarmZonesAsync())
    .WithMetadata(new RequiresPermissionAttribute("Farm.Read"))
    .RequireAuthorization("Permission");

farmZoneGroup.MapGet("/farm-id/{farmId}", (FarmZoneEndpoints endpoint, Guid farmId) => endpoint.GetFarmZonesByFarmAsync(farmId))
    .WithMetadata(new RequiresPermissionAttribute("Farm.Read"))
    .RequireAuthorization("Permission");

farmZoneGroup.MapGet("/{id}", (FarmZoneEndpoints endpoint, Guid id) => endpoint.GetFarmZoneByIdAsync(id))
    .WithMetadata(new RequiresPermissionAttribute("Farm.Read"))
    .RequireAuthorization("Permission");

farmZoneGroup.MapPost("/", (FarmZoneEndpoints endpoint, FarmZoneDto farmZone) => endpoint.CreateFarmZoneAsync(farmZone))
    .WithMetadata(new RequiresPermissionAttribute("Farm.Write"))
    .RequireAuthorization("Permission");

farmZoneGroup.MapPut("/{id}", (FarmZoneEndpoints endpoint, Guid id, FarmZoneDto farmZone) => endpoint.UpdateFarmZoneAsync(id, farmZone))
    .WithMetadata(new RequiresPermissionAttribute("Farm.Write"))
    .RequireAuthorization("Permission");

farmZoneGroup.MapDelete("/{id}", (FarmZoneEndpoints endpoint, Guid id) => endpoint.DeleteFarmZoneAsync(id))
    .WithMetadata(new RequiresPermissionAttribute("Farm.Write"))
    .RequireAuthorization("Permission");

farmZoneGroup.MapPost("/boundary", (FarmZoneEndpoints endpoint, FarmZoneDto farmZoneBoundary) => endpoint.CreateUpdateFarmZoneBoundaryAsync(farmZoneBoundary))
    .WithMetadata(new RequiresPermissionAttribute("Farm.Write"))
    .RequireAuthorization("Permission");

farmZoneGroup.MapPut("/boundary/{id}", (FarmZoneEndpoints endpoint, Guid id, FarmZoneDto farmZoneBoundary) => endpoint.UpdateFarmBoundaryZoneAsync(id, farmZoneBoundary))
    .WithMetadata(new RequiresPermissionAttribute("Farm.Write"))
    .RequireAuthorization("Permission");

var farmCropGroup = app.MapGroup("/api/farm-crops");
farmCropGroup.MapGet("/", (FarmCropEndpoints endpoint) => endpoint.GetAllFarmCropsAsync())
    .WithMetadata(new RequiresPermissionAttribute("Farm.Read"))
    .RequireAuthorization("Permission");

farmCropGroup.MapGet("/{id}", (FarmCropEndpoints endpoint, Guid id) => endpoint.GetFarmCropByIdAsync(id))
    .WithMetadata(new RequiresPermissionAttribute("Farm.Read"))
    .RequireAuthorization("Permission");

farmCropGroup.MapGet("/crop/{id}", (FarmCropEndpoints endpoint, Guid id) => endpoint.GetFarmCropByCropIdAsync(id))
    .WithMetadata(new RequiresPermissionAttribute("Farm.Write"))
    .RequireAuthorization("Permission");

farmCropGroup.MapPost("/", (FarmCropEndpoints endpoint, FarmCropDto farmCrop) => endpoint.CreateFarmCropAsync(farmCrop))
    .WithMetadata(new RequiresPermissionAttribute("Farm.Write"))
    .RequireAuthorization("Permission");

farmCropGroup.MapPut("/{id}", (FarmCropEndpoints endpoint, Guid id, FarmCropDto farmCrop) => endpoint.UpdateFarmCropAsync(id, farmCrop))
    .WithMetadata(new RequiresPermissionAttribute("Farm.Write"))
    .RequireAuthorization("Permission");

farmCropGroup.MapDelete("/{id}", (FarmCropEndpoints endpoint, Guid id) => endpoint.DeleteFarmCropAsync(id))
    .WithMetadata(new RequiresPermissionAttribute("Farm.Write"))
    .RequireAuthorization("Permission");

farmCropGroup.MapPost("/crop", (FarmCropEndpoints endpoint, FarmCropDto farmCropBoundary) => endpoint.CreateUpdateFarmCropAsync(farmCropBoundary))
    .WithMetadata(new RequiresPermissionAttribute("Farm.Write"))
    .RequireAuthorization("Permission");

var farmActivityGroup = app.MapGroup("/api/farm-activities");
farmActivityGroup.MapGet("/{id}", ([FromServices] FarmActivityEndpoints endpoint, Guid id) => endpoint.GetFarmActivityByIdAsync(id))
    .WithMetadata(new RequiresPermissionAttribute("Farm.Read"))
    .RequireAuthorization("Permission");

farmActivityGroup.MapGet("/farm-id/{farmId}", ([FromServices] FarmActivityEndpoints endpoint, Guid farmId) => endpoint.GetFarmActivitiesAsync(farmId))
    .WithMetadata(new RequiresPermissionAttribute("Farm.Read"))
    .RequireAuthorization("Permission");

farmActivityGroup.MapGet("/farm-id/{farmId}/start-date/{date}", ([FromServices] FarmActivityEndpoints endpoint, Guid farmId, DateTime date) => endpoint.GetFarmActivitiesAsync(farmId, date))
    .WithMetadata(new RequiresPermissionAttribute("Farm.Read"))
    .RequireAuthorization("Permission");

farmActivityGroup.MapGet("/{activityId}/expenses", ([FromServices] FarmActivityEndpoints endpoint, Guid activityId) => endpoint.GetFarmActivityExpensesAsync(activityId))
    .WithMetadata(new RequiresPermissionAttribute("Farm.Read"))
    .RequireAuthorization("Permission");

farmActivityGroup.MapPost("/", ([FromServices] FarmActivityEndpoints endpoint, FarmActivityDto farmActivity) => endpoint.CreateFarmActivityAsync(farmActivity))
    .WithMetadata(new RequiresPermissionAttribute("Farm.Write"))
    .RequireAuthorization("Permission");

farmActivityGroup.MapPut("/{id}", ([FromServices] FarmActivityEndpoints endpoint, Guid id, FarmActivityDto farmActivity) => endpoint.UpdateFarmActivityAsync(id, farmActivity))
    .WithMetadata(new RequiresPermissionAttribute("Farm.Write"))
    .RequireAuthorization("Permission");

farmActivityGroup.MapDelete("/{id}", ([FromServices] FarmActivityEndpoints endpoint, Guid id) => endpoint.DeleteFarmActivityAsync(id))
    .WithMetadata(new RequiresPermissionAttribute("Farm.Write"))
    .RequireAuthorization("Permission");

var pricingGroup = app.MapGroup("/api/pricing");
pricingGroup.MapGet("/", ([FromServices] PricingEndpoints endpoint, Guid id) => endpoint.GetAllPricingsAsync())
    .WithMetadata(new RequiresPermissionAttribute("Farm.Read"))
    .RequireAuthorization("Permission");

pricingGroup.MapGet("/{id}", ([FromServices] PricingEndpoints endpoint, Guid id) => endpoint.GetPricingByIdAsync(id))
    .WithMetadata(new RequiresPermissionAttribute("Farm.Read"))
    .RequireAuthorization("Permission");

pricingGroup.MapGet("/farm-id/{farmId}", ([FromServices] PricingEndpoints endpoint, Guid farmId) => endpoint.GetPricingsByFarmIdAsync(farmId))
    .WithMetadata(new RequiresPermissionAttribute("Farm.Read"))
    .RequireAuthorization("Permission");

pricingGroup.MapGet("/final-price/{profileId}/{basePrice}", ([FromServices] PricingEndpoints endpoint, Guid profileId, decimal basePrice) => endpoint.CalculateFinalPriceAsync(profileId, basePrice))
    .WithMetadata(new RequiresPermissionAttribute("Farm.Read"))
    .RequireAuthorization("Permission");

pricingGroup.MapPost("/", ([FromServices] PricingEndpoints endpoint, PricingDto obj) => endpoint.CreatePricingAsync(obj))
    .WithMetadata(new RequiresPermissionAttribute("Farm.Write"))
    .RequireAuthorization("Permission");

pricingGroup.MapPut("/{id}", ([FromServices] PricingEndpoints endpoint, Guid id, PricingDto obj) => endpoint.UpdatePricingAsync(id, obj))
    .WithMetadata(new RequiresPermissionAttribute("Farm.Write"))
    .RequireAuthorization("Permission");

pricingGroup.MapDelete("/{id}", ([FromServices] PricingEndpoints endpoint, Guid id) => endpoint.DeletePricingAsync(id))
    .WithMetadata(new RequiresPermissionAttribute("Farm.Write"))
    .RequireAuthorization("Permission");

var expenseGroup = app.MapGroup("/api/farm-expenses");
expenseGroup.MapGet("/farm-id/{farmId}", ([FromServices] FarmExpenseEndpoints endpoint, Guid farmId) => endpoint.GetFarmActivityExpensesAsync(farmId))
    .WithMetadata(new RequiresPermissionAttribute("Farm.Read"))
    .RequireAuthorization("Permission");

expenseGroup.MapGet("/{id}", ([FromServices] FarmExpenseEndpoints endpoint, Guid id) => endpoint.GetFarmExpenseByIdAsync(id))
    .WithMetadata(new RequiresPermissionAttribute("Farm.Read"))
    .RequireAuthorization("Permission");

expenseGroup.MapGet("/activity/{activityId}", ([FromServices] FarmExpenseEndpoints endpoint, Guid activityId) => endpoint.GetFarmExpenseByActivityAsync(activityId))
    .WithMetadata(new RequiresPermissionAttribute("Farm.Read"))
    .RequireAuthorization("Permission");

expenseGroup.MapPost("/", ([FromServices] FarmExpenseEndpoints endpoint, FarmActivityExpenseDto farmExpense) => endpoint.CreateFarmExpenseAsync(farmExpense))
    .WithMetadata(new RequiresPermissionAttribute("Farm.Write"))
    .RequireAuthorization("Permission");

expenseGroup.MapPut("/{id}", ([FromServices] FarmExpenseEndpoints endpoint, Guid id, FarmActivityExpenseDto farmExpense) => endpoint.UpdateFarmExpenseAsync(id, farmExpense))
    .WithMetadata(new RequiresPermissionAttribute("Farm.Write"))
    .RequireAuthorization("Permission");

expenseGroup.MapDelete("/{id}", ([FromServices] FarmExpenseEndpoints endpoint, Guid id) => endpoint.DeleteFarmExpenseAsync(id))
    .WithMetadata(new RequiresPermissionAttribute("Farm.Write"))
    .RequireAuthorization("Permission");

var laborGroup = app.MapGroup("/api/farm-labors");
laborGroup.MapGet("/farm-id/{farmId}", ([FromServices] FarmLaborEndpoints endpoint, Guid farmId) => endpoint.GetFarmActivityLaborersAsync(farmId))
    .WithMetadata(new RequiresPermissionAttribute("Farm.Read"))
    .RequireAuthorization("Permission");

laborGroup.MapGet("/{id}", ([FromServices] FarmLaborEndpoints endpoint, Guid id) => endpoint.GetFarmLaborByIdAsync(id))
    .WithMetadata(new RequiresPermissionAttribute("Farm.Read"))
    .RequireAuthorization("Permission");

laborGroup.MapGet("/activity/{activityId}", ([FromServices] FarmLaborEndpoints endpoint, Guid activityId) => endpoint.GetFarmLaborByActivityAsync(activityId))
    .WithMetadata(new RequiresPermissionAttribute("Farm.Read"))
    .RequireAuthorization("Permission");

laborGroup.MapPost("/", ([FromServices] FarmLaborEndpoints endpoint, FarmActivityLaborerDto farmLabor) => endpoint.CreateFarmLaborAsync(farmLabor))
    .WithMetadata(new RequiresPermissionAttribute("Farm.Write"))
    .RequireAuthorization("Permission");

laborGroup.MapPut("/{id}", ([FromServices] FarmLaborEndpoints endpoint, Guid id, FarmActivityLaborerDto farmLabor) => endpoint.UpdateFarmLaborAsync(id, farmLabor))
    .WithMetadata(new RequiresPermissionAttribute("Farm.Write"))
    .RequireAuthorization("Permission");

laborGroup.MapDelete("/{id}", ([FromServices] FarmLaborEndpoints endpoint, Guid id) => endpoint.DeleteFarmLaborAsync(id))
    .WithMetadata(new RequiresPermissionAttribute("Farm.Write"))
    .RequireAuthorization("Permission");

var usageGroup = app.MapGroup("/api/farm-usages");
usageGroup.MapGet("/farm-id/{farmId}", ([FromServices] FarmUsageEndpoints endpoint, Guid farmId) => endpoint.GetFarmActivityUsagesAsync(farmId))
    .WithMetadata(new RequiresPermissionAttribute("Farm.Read"))
    .RequireAuthorization("Permission");

usageGroup.MapGet("/farm-id/{farmId}/{itemId}", ([FromServices] FarmUsageEndpoints endpoint, Guid farmId, Guid itemId) => endpoint.GetFarmActivityUsagesByItemAsync(farmId, itemId))
    .WithMetadata(new RequiresPermissionAttribute("Farm.Read"))
    .RequireAuthorization("Permission");

usageGroup.MapGet("/{id}", ([FromServices] FarmUsageEndpoints endpoint, Guid id) => endpoint.GetFarmUsageByIdAsync(id))
    .WithMetadata(new RequiresPermissionAttribute("Farm.Read"))
    .RequireAuthorization("Permission");

usageGroup.MapGet("/activity/{activityId}", ([FromServices] FarmUsageEndpoints endpoint, Guid activityId) => endpoint.GetFarmUsageByActivityAsync(activityId))
    .WithMetadata(new RequiresPermissionAttribute("Farm.Read"))
    .RequireAuthorization("Permission");

usageGroup.MapPost("/", ([FromServices] FarmUsageEndpoints endpoint, FarmActivityUsageDto farmUsage) => endpoint.CreateFarmUsageAsync(farmUsage))
    .WithMetadata(new RequiresPermissionAttribute("Farm.Write"))
    .RequireAuthorization("Permission");

usageGroup.MapPut("/{id}", ([FromServices] FarmUsageEndpoints endpoint, Guid id, FarmActivityUsageDto farmUsage) => endpoint.UpdateFarmUsageAsync(id, farmUsage))
    .WithMetadata(new RequiresPermissionAttribute("Farm.Write"))
    .RequireAuthorization("Permission");

usageGroup.MapDelete("/{id}", ([FromServices] FarmUsageEndpoints endpoint, Guid id) => endpoint.DeleteFarmUsageAsync(id))
    .WithMetadata(new RequiresPermissionAttribute("Farm.Write"))
    .RequireAuthorization("Permission");

var farmInventoryGroup = app.MapGroup("/api/farm-inventories");
farmInventoryGroup.MapGet("/farm-id/{farmId}", ([FromServices] FarmInventoryEndpoints endpoint, Guid farmId) => endpoint.GetFarmInventoriesAsync(farmId))
    .WithMetadata(new RequiresPermissionAttribute("Farm.Read"))
    .RequireAuthorization("Permission");

farmInventoryGroup.MapGet("/farm-id/{farmId}/behaviors/{behaviors}", ([FromServices] FarmInventoryEndpoints endpoint, Guid farmId, string behaviors) => endpoint.GetFarmInventoriesAsync(farmId, behaviors))
    .WithMetadata(new RequiresPermissionAttribute("Farm.Read"))
    .RequireAuthorization("Permission");

farmInventoryGroup.MapGet("/{id}", ([FromServices] FarmInventoryEndpoints endpoint, Guid id) => endpoint.GetFarmInventoryByIdAsync(id))
    .WithMetadata(new RequiresPermissionAttribute("Farm.Read"))
    .RequireAuthorization("Permission");

farmInventoryGroup.MapGet("/presigned-url/{filename}/{contentType}", ([FromServices] FarmInventoryEndpoints endpoint, string filename, string contentType) => endpoint.GetS3PresignedUrlAsync(photosBucketName, filename, contentType))
    .WithMetadata(new RequiresPermissionAttribute("Farm.Write"))
    .RequireAuthorization("Permission");

farmInventoryGroup.MapPost("/", ([FromServices] FarmInventoryEndpoints endpoint, FarmInventoryDto farmInventory) => endpoint.CreateFarmInventoryAsync(farmInventory))
    .WithMetadata(new RequiresPermissionAttribute("Farm.Write"))
    .RequireAuthorization("Permission");

farmInventoryGroup.MapPut("/{id}", ([FromServices] FarmInventoryEndpoints endpoint, Guid id, FarmInventoryDto farmInventory) => endpoint.UpdateFarmInventoryAsync(id, farmInventory))
    .WithMetadata(new RequiresPermissionAttribute("Farm.Write"))
    .RequireAuthorization("Permission");

farmInventoryGroup.MapPut("/{id}/{s3ObjectKey}/{s3ObjectContentType}", ([FromServices] FarmInventoryEndpoints endpoint, Guid id, string s3ObjectKey, string s3ObjectContentType) => endpoint.UpdateFarmInventoryPhotoAsync(id, s3ObjectKey, s3ObjectContentType, s3BucketUrl))
    .WithMetadata(new RequiresPermissionAttribute("Farm.Write"))
    .RequireAuthorization("Permission");

//farmInventoryGroup.MapPut("/upload-file/{filename}", ([FromServices] FarmInventoryEndpoints endpoint, string filename) => endpoint.UploadInventoryPhotoAsync(photosBucketName, filename))
//    .WithMetadata(new RequiresPermissionAttribute("Farm.Write"))
//    .RequireAuthorization("Permission");

farmInventoryGroup.MapDelete("/{id}", ([FromServices] FarmInventoryEndpoints endpoint, Guid id) => endpoint.DeleteFarmInventoryAsync(id))
    .WithMetadata(new RequiresPermissionAttribute("Farm.Write"))
    .RequireAuthorization("Permission");

var farmContractGroup = app.MapGroup("/api/farm-contracts");
farmContractGroup.MapGet("/farm-id/{farmId}", ([FromServices] FarmContractEndpoints endpoint, Guid farmId) => endpoint.GetFarmContractsAsync(farmId))
    .WithMetadata(new RequiresPermissionAttribute("Farm.Read"))
    .RequireAuthorization("Permission");

farmContractGroup.MapGet("/farm-id/{farmId}/{dateStart}/{dateEnd}", ([FromServices] FarmContractEndpoints endpoint, Guid farmId, DateTime dateStart, DateTime dateEnd) => endpoint.GetFarmContractsAsync(farmId, dateStart, dateEnd))
    .WithMetadata(new RequiresPermissionAttribute("Farm.Read"))
    .RequireAuthorization("Permission");

farmContractGroup.MapGet("/{id}", ([FromServices] FarmContractEndpoints endpoint, Guid id) => endpoint.GetFarmContractByIdAsync(id))
    .WithMetadata(new RequiresPermissionAttribute("Farm.Read"))
    .RequireAuthorization("Permission");

farmContractGroup.MapPost("/", ([FromServices] FarmContractEndpoints endpoint, FarmContractDto farmContract) => endpoint.CreateFarmContractAsync(farmContract))
    .WithMetadata(new RequiresPermissionAttribute("Farm.Write"))
    .RequireAuthorization("Permission");

farmContractGroup.MapPut("/{id}", ([FromServices] FarmContractEndpoints endpoint, Guid id, FarmContractDto farmContract) => endpoint.UpdateFarmContractAsync(id, farmContract))
    .WithMetadata(new RequiresPermissionAttribute("Farm.Write"))
    .RequireAuthorization("Permission");

farmContractGroup.MapDelete("/{id}", ([FromServices] FarmContractEndpoints endpoint, Guid id) => endpoint.DeleteFarmContractAsync(id))
    .WithMetadata(new RequiresPermissionAttribute("Farm.Write"))
    .RequireAuthorization("Permission");

var farmContractDetailGroup = app.MapGroup("/api/farm-contracts/details");
farmContractDetailGroup.MapGet("/farm-id/{farmId}", ([FromServices] FarmContractDetailEndpoints endpoint, Guid farmId) => endpoint.GetFarmContractDetailsAsync(farmId))
    .WithMetadata(new RequiresPermissionAttribute("Farm.Read"))
    .RequireAuthorization("Permission");

farmContractDetailGroup.MapGet("/{id}", ([FromServices] FarmContractDetailEndpoints endpoint, Guid id) => endpoint.GetFarmContractDetailByIdAsync(id))
    .WithMetadata(new RequiresPermissionAttribute("Farm.Read"))
    .RequireAuthorization("Permission");

farmContractDetailGroup.MapPost("/", ([FromServices] FarmContractDetailEndpoints endpoint, FarmContractDetailDto farmContractDetail) => endpoint.CreateFarmContractDetailAsync(farmContractDetail))
    .WithMetadata(new RequiresPermissionAttribute("Farm.Write"))
    .RequireAuthorization("Permission");

farmContractDetailGroup.MapPut("/{id}", ([FromServices] FarmContractDetailEndpoints endpoint, Guid id, FarmContractDetailDto farmContractDetail) => endpoint.UpdateFarmContractDetailAsync(id, farmContractDetail))
    .WithMetadata(new RequiresPermissionAttribute("Farm.Write"))
    .RequireAuthorization("Permission");

farmContractDetailGroup.MapPut("/confirm-pickup/{id}", ([FromServices] FarmContractDetailEndpoints endpoint, Guid id) => endpoint.ConfirmPickupAsync(id))
    .WithMetadata(new RequiresPermissionAttribute("Farm.Write"))
    .RequireAuthorization("Permission");

farmContractDetailGroup.MapPut("/cancel-transaction/{id}", ([FromServices] FarmContractDetailEndpoints endpoint, Guid id) => endpoint.CancelTransactionAsync(id))
    .WithMetadata(new RequiresPermissionAttribute("Farm.Write"))
    .RequireAuthorization("Permission");

farmContractDetailGroup.MapPut("/mark-as-paid/{id}", ([FromServices] FarmContractDetailEndpoints endpoint, Guid id) => endpoint.MarkTransactionAsPaidAsync(id))
    .WithMetadata(new RequiresPermissionAttribute("Farm.Write"))
    .RequireAuthorization("Permission");

farmContractDetailGroup.MapPut("/set-pickup-date/{id}/{date}", ([FromServices] FarmContractDetailEndpoints endpoint, Guid id, DateTime date) => endpoint.SetPickupDateAsync(id, date))
    .WithMetadata(new RequiresPermissionAttribute("Farm.Write"))
    .RequireAuthorization("Permission");

farmContractDetailGroup.MapPut("/set-harvest-date/{id}/{date}", ([FromServices] FarmContractDetailEndpoints endpoint, Guid id, DateTime date) => endpoint.SetHarvestDateAsync(id, date))
    .WithMetadata(new RequiresPermissionAttribute("Farm.Write"))
    .RequireAuthorization("Permission");

farmContractDetailGroup.MapPut("/recover-harvest/{id}", ([FromServices] FarmContractDetailEndpoints endpoint, Guid id) => endpoint.RecoverHarvestAsync(id))
    .WithMetadata(new RequiresPermissionAttribute("Farm.Write"))
    .RequireAuthorization("Permission");

farmContractDetailGroup.MapDelete("/{id}", ([FromServices] FarmContractDetailEndpoints endpoint, Guid id) => endpoint.DeleteFarmContractDetailAsync(id))
    .WithMetadata(new RequiresPermissionAttribute("Farm.Write"))
    .RequireAuthorization("Permission");

var farmContractSaleGroup = app.MapGroup("/api/farm-contracts/sales");
farmContractSaleGroup.MapGet("/farm-id/{farmId}", ([FromServices] FarmContractSaleEndpoints endpoint, Guid farmId) => endpoint.GetFarmContractSalesAsync(farmId))
    .WithMetadata(new RequiresPermissionAttribute("Finance.Read"))
    .RequireAuthorization("Permission");

//farmContractSaleGroup.MapGet("/{id}", ([FromServices] FarmContractSaleEndpoints endpoint, Guid id) => endpoint.GetFarmContractSaleByIdAsync(id))

var farmSalesGroup = app.MapGroup("/api/farm-sales");
farmSalesGroup.MapGet("/farm-id/{farmId}", ([FromServices] FarmSalesEndpoints endpoint, Guid farmId) => endpoint.GetFarmSalesAsync(farmId))
    .WithMetadata(new RequiresPermissionAttribute("Finance.Read"),
                  new RequiresPermissionAttribute("Cashier.Read"))
    .RequireAuthorization("Permission");

farmSalesGroup.MapGet("/{id}", ([FromServices] FarmSalesEndpoints endpoint, Guid id) => endpoint.GetFarmSalesByIdAsync(id))
    .WithMetadata(new RequiresPermissionAttribute("Finance.Read"),
                  new RequiresPermissionAttribute("Cashier.Read"))
    .RequireAuthorization("Permission");

farmSalesGroup.MapGet("/recent/{farmId}", ([FromServices] FarmSalesEndpoints endpoint, Guid farmId) => endpoint.GetRecentFarmTransactionsAsync(farmId))
    .WithMetadata(new RequiresPermissionAttribute("Finance.Read"),
                  new RequiresPermissionAttribute("Cashier.Read"))
    .RequireAuthorization("Permission");

farmSalesGroup.MapPost("/", ([FromServices] FarmSalesEndpoints endpoint, FarmTransactionDto farmSales) => endpoint.CreateFarmSalesAsync(farmSales))
    .WithMetadata(new RequiresPermissionAttribute("Finance.Write"),
                  new RequiresPermissionAttribute("Cashier.Write"))
    .RequireAuthorization("Permission");

farmSalesGroup.MapPut("/{id}", ([FromServices] FarmSalesEndpoints endpoint, Guid id, FarmTransactionDto farmSales) => endpoint.UpdateFarmSalesAsync(id, farmSales))
    .WithMetadata(new RequiresPermissionAttribute("Finance.Write"),
                  new RequiresPermissionAttribute("Cashier.Write"))
    .RequireAuthorization("Permission");

farmSalesGroup.MapPut("/cancel-transaction/{id}", ([FromServices] FarmSalesEndpoints endpoint, Guid id) => endpoint.CancelTransactionAsync(id))
    .WithMetadata(new RequiresPermissionAttribute("Finance.Write"))
    .RequireAuthorization("Permission");

farmSalesGroup.MapDelete("/{id}", ([FromServices] FarmSalesEndpoints endpoint, Guid id) => endpoint.DeleteFarmSalesAsync(id))
    .WithMetadata(new RequiresPermissionAttribute("Finance.Write"))
    .RequireAuthorization("Permission");

var farmProduceInventoryGroup = app.MapGroup("/api/produce");
farmProduceInventoryGroup.MapGet("/{farmId}", ([FromServices] FarmProduceInventoryEndpoints endpoint, Guid farmId) => endpoint.GetFarmProduceInventoriesAsync(farmId))
    .WithMetadata(new RequiresPermissionAttribute("Farm.Read"),
                  new RequiresPermissionAttribute("Cashier.Read"))
    .RequireAuthorization("Permission");

farmProduceInventoryGroup.MapGet("/{farmId}/{typeId}", ([FromServices] FarmProduceInventoryEndpoints endpoint, Guid farmId, Guid typeId) => endpoint.GetFarmProduceInventoriesAsync(farmId, typeId))
    .WithMetadata(new RequiresPermissionAttribute("Farm.Read"),
                  new RequiresPermissionAttribute("Cashier.Read"))
    .RequireAuthorization("Permission");

var pricingProfileGroup = app.MapGroup("/api/pricing-profiles");
pricingProfileGroup.MapGet("/{id}", ([FromServices] PricingProfileEndpoints endpoint, Guid id) => endpoint.GetPricingProfileByIdAsync(id))
    .WithMetadata(new RequiresPermissionAttribute("Farm.Read"))
    .RequireAuthorization("Permission");

pricingProfileGroup.MapGet("/farm-id/{farmId}", ([FromServices] PricingProfileEndpoints endpoint, Guid farmId) => endpoint.GetPricingProfilesByFarmIdAsync(farmId))
    .WithMetadata(new RequiresPermissionAttribute("Farm.Read"))
    .RequireAuthorization("Permission");

pricingProfileGroup.MapPost("/", ([FromServices] PricingProfileEndpoints endpoint, PricingProfileDto obj) => endpoint.CreatePricingProfileAsync(obj))
    .WithMetadata(new RequiresPermissionAttribute("Farm.Write"))
    .RequireAuthorization("Permission");

pricingProfileGroup.MapPut("/{id}", ([FromServices] PricingProfileEndpoints endpoint, Guid id, PricingProfileDto obj) => endpoint.UpdatePricingProfileAsync(id, obj))
    .WithMetadata(new RequiresPermissionAttribute("Farm.Write"))
    .RequireAuthorization("Permission");

pricingProfileGroup.MapDelete("/{id}", ([FromServices] PricingProfileEndpoints endpoint, Guid id) => endpoint.DeletePricingProfileAsync(id))
    .WithMetadata(new RequiresPermissionAttribute("Farm.Write"))
    .RequireAuthorization("Permission");

var quotationGroup = app.MapGroup("/api/quotations");
quotationGroup.MapGet("/{id}", ([FromServices] QuotationEndpoints endpoint, Guid id) => endpoint.GetQuotationByIdAsync(id))
    .WithMetadata(new RequiresPermissionAttribute("Farm.Read"))
    .RequireAuthorization("Permission");

quotationGroup.MapGet("/farm-id/{farmId}", ([FromServices] QuotationEndpoints endpoint, Guid farmId) => endpoint.GetQuotationsByFarmIdAsync(farmId))
    .WithMetadata(new RequiresPermissionAttribute("Farm.Read"))
    .RequireAuthorization("Permission");

quotationGroup.MapGet("/farm-id/{farmId}/{topN}", ([FromServices] QuotationEndpoints endpoint, Guid farmId, int topN) => endpoint.GetQuotationsByFarmIdAsync(farmId, topN))
    .WithMetadata(new RequiresPermissionAttribute("Farm.Read"))
    .RequireAuthorization("Permission");

quotationGroup.MapGet("/farm-id/{farmId}/top3", ([FromServices] QuotationEndpoints endpoint, Guid farmId) => endpoint.GetTop3QuotationsByFarmIdAsync(farmId))
    .WithMetadata(new RequiresPermissionAttribute("Farm.Read"))
    .RequireAuthorization("Permission");

//quotationGroup.MapPost("/", ([FromServices] QuotationEndpoints endpoint, QuotationDto obj) => endpoint.CreateQuotationAsync(obj))
//    .WithMetadata(new RequiresPermissionAttribute("Farm.Write"))
//    .RequireAuthorization("Permission");

quotationGroup.MapPost("/send-quotation", ([FromServices] QuotationEndpoints endpoint, SendQuotationParamsModel paramsModel) => endpoint.SendQuotationEmailAsync(paramsModel))
    .WithMetadata(new RequiresPermissionAttribute("Farm.Read"))
    .RequireAuthorization("Permission");

quotationGroup.MapPost("/create-send-quotation", ([FromServices] QuotationEndpoints endpoint, SendQuotationParamsModel paramsModel) => endpoint.CreateAndSendQuotationEmailAsync(paramsModel))
    .WithMetadata(new RequiresPermissionAttribute("Farm.Read"))
    .RequireAuthorization("Permission");

var activityPhotosGroup = app.MapGroup("/api/farm-activity-photos");
activityPhotosGroup.MapGet("/{id}", ([FromServices] FarmActivityPhotoEndpoints endpoint, Guid id) => endpoint.GetFarmActivityPhotoByIdAsync(id))
    .WithMetadata(new RequiresPermissionAttribute("Farm.Read"))
    .RequireAuthorization("Permission");

activityPhotosGroup.MapGet("/activity/{activityId}", ([FromServices] FarmActivityPhotoEndpoints endpoint, Guid activityId) => endpoint.GetFarmActivityPhotoByActivityAsync(activityId))
    .WithMetadata(new RequiresPermissionAttribute("Farm.Read"))
    .RequireAuthorization("Permission");

activityPhotosGroup.MapPost("/", ([FromServices] FarmActivityPhotoEndpoints endpoint, FarmActivityPhotoDto farmPhoto) => endpoint.CreateFarmActivityPhotoAsync(farmPhoto))
    .WithMetadata(new RequiresPermissionAttribute("Farm.Write"))
    .RequireAuthorization("Permission");

activityPhotosGroup.MapPost("/upload", ([FromServices] FarmActivityPhotoEndpoints endpoint, S3PhotoUploadDto obj) => endpoint.CreateFarmActivityPhotoAsync(obj, s3BucketUrl))
    .WithMetadata(new RequiresPermissionAttribute("Farm.Write"))
    .RequireAuthorization("Permission");

activityPhotosGroup.MapPut("/{id}", ([FromServices] FarmActivityPhotoEndpoints endpoint, Guid id, FarmActivityPhotoDto farmPhoto) => endpoint.UpdateFarmActivityPhotoAsync(id, farmPhoto))
    .WithMetadata(new RequiresPermissionAttribute("Farm.Write"))
    .RequireAuthorization("Permission");

activityPhotosGroup.MapDelete("/{id}", ([FromServices] FarmActivityPhotoEndpoints endpoint, Guid id) => endpoint.DeleteFarmActivityPhotoAsync(id))
    .WithMetadata(new RequiresPermissionAttribute("Farm.Write"))
    .RequireAuthorization("Permission");


var farmGeneralExpensesGroup = app.MapGroup("/api/farm-general-expenses");
farmGeneralExpensesGroup.MapGet("/farm-id/{farmId}", ([FromServices] FarmGeneralExpenseEndpoints endpoint, Guid farmId) => endpoint.GetFarmGeneralExpensesAsync(farmId))
    .WithMetadata(new RequiresPermissionAttribute("Finance.Read"))
    .RequireAuthorization("Permission");

farmGeneralExpensesGroup.MapGet("/farm-id/{farmId}/{startDate}/{endDate}", ([FromServices] FarmGeneralExpenseEndpoints endpoint, Guid farmId, DateTime startDate, DateTime endDate) => endpoint.GetFarmGeneralExpensesAsync(farmId, startDate, endDate))
    .WithMetadata(new RequiresPermissionAttribute("Finance.Read"))
    .RequireAuthorization("Permission");

farmGeneralExpensesGroup.MapGet("/{id}", ([FromServices] FarmGeneralExpenseEndpoints endpoint, Guid id) => endpoint.GetFarmGeneralExpenseByIdAsync(id))
    .WithMetadata(new RequiresPermissionAttribute("Finance.Read"))
    .RequireAuthorization("Permission");

//farmGeneralExpensesGroup.MapGet("/recent/{farmId}", ([FromServices] FarmGeneralExpenseEndpoints endpoint, Guid farmId) => endpoint.GetRecentFarmGeneralExpensesAsync(farmId))
//    .WithMetadata(new RequiresPermissionAttribute("Finance.Read"))
//    .RequireAuthorization("Permission");

farmGeneralExpensesGroup.MapPost("/", ([FromServices] FarmGeneralExpenseEndpoints endpoint, FarmGeneralExpenseDto farmGeneralExpense) => endpoint.CreateFarmGeneralExpenseAsync(farmGeneralExpense))
    .WithMetadata(new RequiresPermissionAttribute("Finance.Write"))
    .RequireAuthorization("Permission");

farmGeneralExpensesGroup.MapPut("/{id}", ([FromServices] FarmGeneralExpenseEndpoints endpoint, Guid id, FarmGeneralExpenseDto farmGeneralExpense) => endpoint.UpdateFarmGeneralExpenseAsync(id, farmGeneralExpense))
    .WithMetadata(new RequiresPermissionAttribute("Finance.Write"))
    .RequireAuthorization("Permission");

farmGeneralExpensesGroup.MapDelete("/{id}", ([FromServices] FarmGeneralExpenseEndpoints endpoint, Guid id) => endpoint.DeleteFarmGeneralExpenseAsync(id))
    .WithMetadata(new RequiresPermissionAttribute("Finance.Write"))
    .RequireAuthorization("Permission");


var generalExpenseReceiptsGroup = app.MapGroup("/api/farm-general-expense-receipts");
generalExpenseReceiptsGroup.MapGet("/{id}", ([FromServices] FarmGeneralExpenseReceiptEndpoints endpoint, Guid id) => endpoint.GetFarmGeneralExpenseReceiptByIdAsync(id))
    .WithMetadata(new RequiresPermissionAttribute("Finance.Read"))
    .RequireAuthorization("Permission");

generalExpenseReceiptsGroup.MapGet("/general-expense/{generalExpenseId}", ([FromServices] FarmGeneralExpenseReceiptEndpoints endpoint, Guid generalExpenseId) => endpoint.GetFarmGeneralExpenseReceiptByGeneralExpenseAsync(generalExpenseId))
    .WithMetadata(new RequiresPermissionAttribute("Finance.Read"))
    .RequireAuthorization("Permission");

generalExpenseReceiptsGroup.MapPost("/", ([FromServices] FarmGeneralExpenseReceiptEndpoints endpoint, FarmGeneralExpenseReceiptDto receipt) => endpoint.CreateFarmGeneralExpenseReceiptAsync(receipt))
    .WithMetadata(new RequiresPermissionAttribute("Finance.Write"))
    .RequireAuthorization("Permission");

generalExpenseReceiptsGroup.MapPost("/upload", ([FromServices] FarmGeneralExpenseReceiptEndpoints endpoint, S3PhotoUploadDto obj) => endpoint.CreateFarmGeneralExpenseReceiptAsync(obj, s3BucketUrl))
    .WithMetadata(new RequiresPermissionAttribute("Finance.Write"))
    .RequireAuthorization("Permission");

generalExpenseReceiptsGroup.MapPut("/{id}", ([FromServices] FarmGeneralExpenseReceiptEndpoints endpoint, Guid id, FarmGeneralExpenseReceiptDto receipt) => endpoint.UpdateFarmGeneralExpenseReceiptAsync(id, receipt))
    .WithMetadata(new RequiresPermissionAttribute("Finance.Write"))
    .RequireAuthorization("Permission");

generalExpenseReceiptsGroup.MapDelete("/{id}", ([FromServices] FarmGeneralExpenseReceiptEndpoints endpoint, Guid id) => endpoint.DeleteFarmGeneralExpenseReceiptAsync(id))
    .WithMetadata(new RequiresPermissionAttribute("Finance.Write"))
    .RequireAuthorization("Permission");

//var endpointDataSource = app.Services.GetRequiredService<EndpointDataSource>();
//foreach (var endpoint in endpointDataSource.Endpoints)
//{
//    Console.WriteLine($"Endpoint: {endpoint.DisplayName}");
//}

app.Run();