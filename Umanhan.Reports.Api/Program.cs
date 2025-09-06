using Amazon.S3;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Umanhan.Dtos;
using Umanhan.Dtos.Validators;
using Umanhan.Models;
using Umanhan.Models.Attributes;
using Umanhan.Models.Models;
using Umanhan.Reports.Api;
using Umanhan.Reports.Api.Endpoints;
using Umanhan.Repositories;
using Umanhan.Repositories.Interfaces;
using Umanhan.Services;
using Umanhan.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);
builder.Logging.ClearProviders();
builder.Logging.AddConsole(); // for AWS CloudWatch
builder.Services.AddAWSLambdaHosting(LambdaEventSource.HttpApi); // for AWS Lambda
builder.Services.AddDefaultAWSOptions(builder.Configuration.GetAWSOptions());
builder.Services.AddAWSService<IAmazonS3>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// In-memory cache
builder.Services.AddMemoryCache();

// Register FluentValidation
builder.Services.AddValidatorsFromAssemblyContaining<ReportValidator>();

builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddHttpContextAccessor();

builder.Services.AddDbContextPool<UmanhanDbContext>(options =>
    options.UseNpgsql(Environment.GetEnvironmentVariable("CONNECTION_STRING"), o =>
    {
        // execution strategy
        o.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
    }));

// repositories
builder.Services.AddScoped(typeof(IRepository<>), typeof(UmanhanRepository<>));
//builder.Services.AddScoped<IFarmActivityRepository, FarmActivityRepository>();

// validators
builder.Services.AddScoped<IValidator<ReportDto>, ReportValidator>();

// services
builder.Services.AddScoped<IUserContextService, UserContextService>();
builder.Services.AddScoped<IPermissionService, PermissionService>();
builder.Services.AddScoped<RolePermissionService>();
builder.Services.AddScoped<ReportService>();
builder.Services.AddScoped<ContractPaymentService>();

builder.Services.AddSingleton<ISchemaProvider, S3SchemaProvider>();

// endpoints
builder.Services.AddScoped<ContractReportEndpoints>();
builder.Services.AddScoped<ReportEndpoints>();
builder.Services.AddScoped<PnlEndpoints>();
builder.Services.AddScoped<SalesReportEndpoints>();
builder.Services.AddScoped<GeneralExpenseReportEndpoints>();
builder.Services.AddScoped<ProductPerformanceEndpoints>();
builder.Services.AddScoped<ReceiptEndpoints>();

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

string nlpApiUrl = Environment.GetEnvironmentVariable("WEB_API_URL_NLP") ?? throw new InvalidOperationException("Missing environment variable: WEB_API_URL_NLP");
builder.Services.AddHttpClient("NlpAPI", client =>
{
    client.BaseAddress = new Uri(nlpApiUrl);
});

var cognitoAuthority = Environment.GetEnvironmentVariable("COGNITO_AUTHORITY") ?? throw new InvalidOperationException("Missing environment variable: COGNITO_AUTHORITY");
var cognitoAudience = Environment.GetEnvironmentVariable("COGNITO_AUDIENCE") ?? throw new InvalidOperationException("Missing environment variable: COGNITO_AUDIENCE");

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

var reportsGroup = app.MapGroup("/api/reports");
//reportsGroup.MapGet("/", (FarmZoneEndpoints endpoint) => endpoint.GetAllFarmZonesAsync())
//    .WithMetadata(new RequiresPermissionAttribute("Report.Read"))
//    .RequireAuthorization("Permission");

reportsGroup.MapGet("/pnl-report/{farmId}/{period}", ([FromServices] ReportEndpoints endpoint, Guid farmId, DateTime period) => endpoint.GeneratePnlReportAsync(farmId, period))
    .WithMetadata(new RequiresPermissionAttribute("Report.Read"))
    .RequireAuthorization("Permission");

reportsGroup.MapPost("/generate-nlp-based-report", ([FromServices] ReportEndpoints endpoint, [FromBody] string prompt) => endpoint.GenerateNlpBasedReportAsync(prompt))
    .WithMetadata(new RequiresPermissionAttribute("Report.Read"))
    .RequireAuthorization("Permission");


var pnlGroup = app.MapGroup("/api/pnl");
pnlGroup.MapGet("/total-revenue/{farmId}/{dateStart}/{dateEnd}", ([FromServices] PnlEndpoints endpoint, Guid farmId, DateTime dateStart, DateTime dateEnd) => endpoint.GetTotalRevenueAsync(farmId, dateStart, dateEnd))
    .WithMetadata(new RequiresPermissionAttribute("Report.Read"))
    .RequireAuthorization("Permission");

pnlGroup.MapGet("/cost-of-goods-sold/{farmId}/{dateStart}/{dateEnd}", ([FromServices] PnlEndpoints endpoint, Guid farmId, DateTime dateStart, DateTime dateEnd) => endpoint.GetCostOfGoodsSoldAsync(farmId, dateStart, dateEnd))
    .WithMetadata(new RequiresPermissionAttribute("Report.Read"))
    .RequireAuthorization("Permission");

pnlGroup.MapGet("/gross-margin-percent/{farmId}/{dateStart}/{dateEnd}", ([FromServices] PnlEndpoints endpoint, Guid farmId, DateTime dateStart, DateTime dateEnd) => endpoint.GetGrossMarginPercentAsync(farmId, dateStart, dateEnd))
    .WithMetadata(new RequiresPermissionAttribute("Report.Read"))
    .RequireAuthorization("Permission");

pnlGroup.MapGet("/operating-expense-ratio/{farmId}/{dateStart}/{dateEnd}", ([FromServices] PnlEndpoints endpoint, Guid farmId, DateTime dateStart, DateTime dateEnd) => endpoint.GetOperatingExpenseRatioAsync(farmId, dateStart, dateEnd))
    .WithMetadata(new RequiresPermissionAttribute("Report.Read"))
    .RequireAuthorization("Permission");

pnlGroup.MapGet("/operating-expenses/{farmId}/{dateStart}/{dateEnd}", ([FromServices] PnlEndpoints endpoint, Guid farmId, DateTime dateStart, DateTime dateEnd) => endpoint.GetOperatingExpensesAsync(farmId, dateStart, dateEnd))
    .WithMetadata(new RequiresPermissionAttribute("Report.Read"))
    .RequireAuthorization("Permission");

pnlGroup.MapGet("/net-profit-margin/{farmId}/{dateStart}/{dateEnd}", ([FromServices] PnlEndpoints endpoint, Guid farmId, DateTime dateStart, DateTime dateEnd) => endpoint.GetNetProfitMarginAsync(farmId, dateStart, dateEnd))
    .WithMetadata(new RequiresPermissionAttribute("Report.Read"))
    .RequireAuthorization("Permission");

pnlGroup.MapGet("/gross-profit/{farmId}/{dateStart}/{dateEnd}", ([FromServices] PnlEndpoints endpoint, Guid farmId, DateTime dateStart, DateTime dateEnd) => endpoint.GetGrossProfitAsync(farmId, dateStart, dateEnd))
    .WithMetadata(new RequiresPermissionAttribute("Report.Read"))
    .RequireAuthorization("Permission");

pnlGroup.MapGet("/net-profit/{farmId}/{dateStart}/{dateEnd}", ([FromServices] PnlEndpoints endpoint, Guid farmId, DateTime dateStart, DateTime dateEnd) => endpoint.GetNetProfitAsync(farmId, dateStart, dateEnd))
    .WithMetadata(new RequiresPermissionAttribute("Report.Read"))
    .RequireAuthorization("Permission");

pnlGroup.MapGet("/yield-per-hectare/{farmId}/{dateStart}/{dateEnd}", ([FromServices] PnlEndpoints endpoint, Guid farmId, DateTime dateStart, DateTime dateEnd) => endpoint.GetYieldPerHectareAsync(farmId, dateStart, dateEnd))
    .WithMetadata(new RequiresPermissionAttribute("Report.Read"))
    .RequireAuthorization("Permission");

pnlGroup.MapGet("/total-donated/{farmId}/{dateStart}/{dateEnd}", ([FromServices] PnlEndpoints endpoint, Guid farmId, DateTime dateStart, DateTime dateEnd) => endpoint.GetTotalDonatedAsync(farmId, dateStart, dateEnd))
    .WithMetadata(new RequiresPermissionAttribute("Report.Read"))
    .RequireAuthorization("Permission");

pnlGroup.MapGet("/total-spoilage/{farmId}/{dateStart}/{dateEnd}", ([FromServices] PnlEndpoints endpoint, Guid farmId, DateTime dateStart, DateTime dateEnd) => endpoint.GetTotalSpoilageAsync(farmId, dateStart, dateEnd))
    .WithMetadata(new RequiresPermissionAttribute("Report.Read"))
    .RequireAuthorization("Permission");



pnlGroup.MapGet("/total-revenue/list/{farmId}/{dateStart}/{dateEnd}", ([FromServices] PnlEndpoints endpoint, Guid farmId, DateTime dateStart, DateTime dateEnd) => endpoint.GetTotalRevenueListAsync(farmId, dateStart, dateEnd))
    .WithMetadata(new RequiresPermissionAttribute("Report.Read"))
    .RequireAuthorization("Permission");

pnlGroup.MapGet("/cost-of-goods-sold/list/{farmId}/{dateStart}/{dateEnd}", ([FromServices] PnlEndpoints endpoint, Guid farmId, DateTime dateStart, DateTime dateEnd) => endpoint.GetCostOfGoodsSoldListAsync(farmId, dateStart, dateEnd))
    .WithMetadata(new RequiresPermissionAttribute("Report.Read"))
    .RequireAuthorization("Permission");

pnlGroup.MapGet("/operating-expense-ratio/list/{farmId}/{dateStart}/{dateEnd}", ([FromServices] PnlEndpoints endpoint, Guid farmId, DateTime dateStart, DateTime dateEnd) => endpoint.GetOperatingExpenseRatioListAsync(farmId, dateStart, dateEnd))
    .WithMetadata(new RequiresPermissionAttribute("Report.Read"))
    .RequireAuthorization("Permission");

pnlGroup.MapGet("/operating-expenses/list/{farmId}/{dateStart}/{dateEnd}", ([FromServices] PnlEndpoints endpoint, Guid farmId, DateTime dateStart, DateTime dateEnd) => endpoint.GetOperatingExpenseListAsync(farmId, dateStart, dateEnd))
    .WithMetadata(new RequiresPermissionAttribute("Report.Read"))
    .RequireAuthorization("Permission");

pnlGroup.MapGet("/gross-profit/list/{farmId}/{dateStart}/{dateEnd}", ([FromServices] PnlEndpoints endpoint, Guid farmId, DateTime dateStart, DateTime dateEnd) => endpoint.GetGrossProfitListAsync(farmId, dateStart, dateEnd))
    .WithMetadata(new RequiresPermissionAttribute("Report.Read"))
    .RequireAuthorization("Permission");

pnlGroup.MapGet("/net-profit/list/{farmId}/{dateStart}/{dateEnd}", ([FromServices] PnlEndpoints endpoint, Guid farmId, DateTime dateStart, DateTime dateEnd) => endpoint.GetNetProfitListAsync(farmId, dateStart, dateEnd))
    .WithMetadata(new RequiresPermissionAttribute("Report.Read"))
    .RequireAuthorization("Permission");

pnlGroup.MapGet("/net-profit-margin/list/{farmId}/{dateStart}/{dateEnd}", ([FromServices] PnlEndpoints endpoint, Guid farmId, DateTime dateStart, DateTime dateEnd) => endpoint.GetNetProfitMarginListAsync(farmId, dateStart, dateEnd))
    .WithMetadata(new RequiresPermissionAttribute("Report.Read"))
    .RequireAuthorization("Permission");

pnlGroup.MapGet("/total-donated/list/{farmId}/{dateStart}/{dateEnd}", ([FromServices] PnlEndpoints endpoint, Guid farmId, DateTime dateStart, DateTime dateEnd) => endpoint.GetTotalDonatedListAsync(farmId, dateStart, dateEnd))
    .WithMetadata(new RequiresPermissionAttribute("Report.Read"))
    .RequireAuthorization("Permission");

pnlGroup.MapGet("/total-spoilage/list/{farmId}/{dateStart}/{dateEnd}", ([FromServices] PnlEndpoints endpoint, Guid farmId, DateTime dateStart, DateTime dateEnd) => endpoint.GetTotalSpoilageListAsync(farmId, dateStart, dateEnd))
    .WithMetadata(new RequiresPermissionAttribute("Report.Read"))
    .RequireAuthorization("Permission");

//pnlGroup.MapGet("/gross-margin-percent/list/{farmId}/{dateStart}/{dateEnd}", ([FromServices] PnlEndpoints endpoint, Guid farmId, DateTime dateStart, DateTime dateEnd) => endpoint.GetGrossMarginPercentListAsync(farmId, dateStart, dateEnd))
//    .WithMetadata(new RequiresPermissionAttribute("Report.Read"))
//    .RequireAuthorization("Permission");

//pnlGroup.MapGet("/yield-per-hectare/list/{farmId}/{dateStart}/{dateEnd}", ([FromServices] PnlEndpoints endpoint, Guid farmId, DateTime dateStart, DateTime dateEnd) => endpoint.GetYieldPerHectareListAsync(farmId, dateStart, dateEnd))
//    .WithMetadata(new RequiresPermissionAttribute("Report.Read"))
//    .RequireAuthorization("Permission");


var contractKpisGroup = app.MapGroup("/api/contracts");
contractKpisGroup.MapGet("/expected-revenue/{farmId}/{dateStart}/{dateEnd}", ([FromServices] ContractReportEndpoints endpoint, Guid farmId, DateTime dateStart, DateTime dateEnd) => endpoint.GetContractsExpectedRevenueAsync(farmId, dateStart, dateEnd))
    .WithMetadata(new RequiresPermissionAttribute("Report.Read"))
    .RequireAuthorization("Permission");

contractKpisGroup.MapGet("/total-value/{farmId}/{dateStart}/{dateEnd}", ([FromServices] ContractReportEndpoints endpoint, Guid farmId, DateTime dateStart, DateTime dateEnd) => endpoint.GetContractsTotalValueAsync(farmId, dateStart, dateEnd))
    .WithMetadata(new RequiresPermissionAttribute("Report.Read"))
    .RequireAuthorization("Permission");

contractKpisGroup.MapGet("/total-value-lost/{farmId}/{dateStart}/{dateEnd}", ([FromServices] ContractReportEndpoints endpoint, Guid farmId, DateTime dateStart, DateTime dateEnd) => endpoint.GetContractsTotalLostValueAsync(farmId, dateStart, dateEnd))
    .WithMetadata(new RequiresPermissionAttribute("Report.Read"))
    .RequireAuthorization("Permission");

contractKpisGroup.MapGet("/approaching-harvest/{farmId}/{dateStart}/{dateEnd}", ([FromServices] ContractReportEndpoints endpoint, Guid farmId, DateTime dateStart, DateTime dateEnd) => endpoint.GetContractsApproachingHarvestAsync(farmId, dateStart, dateEnd))
    .WithMetadata(new RequiresPermissionAttribute("Report.Read"))
    .RequireAuthorization("Permission");

contractKpisGroup.MapGet("/new/{farmId}/{dateStart}/{dateEnd}", ([FromServices] ContractReportEndpoints endpoint, Guid farmId, DateTime dateStart, DateTime dateEnd) => endpoint.GetContractsNewAsync(farmId, dateStart, dateEnd))
    .WithMetadata(new RequiresPermissionAttribute("Report.Read"))
    .RequireAuthorization("Permission");

contractKpisGroup.MapGet("/harvested/{farmId}/{dateStart}/{dateEnd}", ([FromServices] ContractReportEndpoints endpoint, Guid farmId, DateTime dateStart, DateTime dateEnd) => endpoint.GetContractsHarvestedAsync(farmId, dateStart, dateEnd))
    .WithMetadata(new RequiresPermissionAttribute("Report.Read"))
    .RequireAuthorization("Permission");

contractKpisGroup.MapGet("/pickedup/{farmId}/{dateStart}/{dateEnd}", ([FromServices] ContractReportEndpoints endpoint, Guid farmId, DateTime dateStart, DateTime dateEnd) => endpoint.GetContractsConfirmedPickeUpsAsync(farmId, dateStart, dateEnd))
    .WithMetadata(new RequiresPermissionAttribute("Report.Read"))
    .RequireAuthorization("Permission");

contractKpisGroup.MapGet("/paid/{farmId}/{dateStart}/{dateEnd}", ([FromServices] ContractReportEndpoints endpoint, Guid farmId, DateTime dateStart, DateTime dateEnd) => endpoint.GetContractsPaidAsync(farmId, dateStart, dateEnd))
    .WithMetadata(new RequiresPermissionAttribute("Report.Read"))
    .RequireAuthorization("Permission");

var salesKpisGroup = app.MapGroup("/api/sales");
salesKpisGroup.MapGet("/{farmId}/{dateStart}/{dateEnd}", ([FromServices] SalesReportEndpoints endpoint, Guid farmId, DateTime dateStart, DateTime dateEnd) => endpoint.GetFarmSalesAsync(farmId, dateStart, dateEnd))
    .WithMetadata(new RequiresPermissionAttribute("Report.Read"))
    .RequireAuthorization("Permission");

salesKpisGroup.MapGet("/monthly-sales/{farmId}/{year}", ([FromServices] SalesReportEndpoints endpoint, Guid farmId, int year) => endpoint.GetMonthlySalesAsync(farmId, year))
    .WithMetadata(new RequiresPermissionAttribute("Report.Read"))
    .RequireAuthorization("Permission");

salesKpisGroup.MapGet("/monthly-sales-by-customer/{farmId}/{year}", ([FromServices] SalesReportEndpoints endpoint, Guid farmId, int year) => endpoint.GetMonthlySalesByCustomerAsync(farmId, year))
    .WithMetadata(new RequiresPermissionAttribute("Report.Read"))
    .RequireAuthorization("Permission");


var genExpensesKpisGroup = app.MapGroup("/api/gen-expenses");
genExpensesKpisGroup.MapGet("/current-year/{farmId}", ([FromServices] GeneralExpenseReportEndpoints endpoint, Guid farmId) => endpoint.GetFarmGeneralExpensesCurrentYearAsync(farmId))
    .WithMetadata(new RequiresPermissionAttribute("Report.Read"))
    .RequireAuthorization("Permission");

genExpensesKpisGroup.MapGet("/previous-year/{farmId}", ([FromServices] GeneralExpenseReportEndpoints endpoint, Guid farmId) => endpoint.GetFarmGeneralExpensesPreviousYearAsync(farmId))
    .WithMetadata(new RequiresPermissionAttribute("Report.Read"))
    .RequireAuthorization("Permission");

genExpensesKpisGroup.MapGet("/current-month/{farmId}", ([FromServices] GeneralExpenseReportEndpoints endpoint, Guid farmId) => endpoint.GetFarmGeneralExpensesCurrentMonthAsync(farmId))
    .WithMetadata(new RequiresPermissionAttribute("Report.Read"))
    .RequireAuthorization("Permission");

genExpensesKpisGroup.MapGet("/previous-month/{farmId}", ([FromServices] GeneralExpenseReportEndpoints endpoint, Guid farmId) => endpoint.GetFarmGeneralExpensesPreviousMonthAsync(farmId))
    .WithMetadata(new RequiresPermissionAttribute("Report.Read"))
    .RequireAuthorization("Permission");

genExpensesKpisGroup.MapGet("/current-quarter/{farmId}", ([FromServices] GeneralExpenseReportEndpoints endpoint, Guid farmId) => endpoint.GetFarmGeneralExpensesCurrentQuarterAsync(farmId))
    .WithMetadata(new RequiresPermissionAttribute("Report.Read"))
    .RequireAuthorization("Permission");

genExpensesKpisGroup.MapGet("/previous-quarter/{farmId}", ([FromServices] GeneralExpenseReportEndpoints endpoint, Guid farmId) => endpoint.GetFarmGeneralExpensesPreviousQuarterAsync(farmId))
    .WithMetadata(new RequiresPermissionAttribute("Report.Read"))
    .RequireAuthorization("Permission");

genExpensesKpisGroup.MapGet("/trend-list-12-months/{farmId}", ([FromServices] GeneralExpenseReportEndpoints endpoint, Guid farmId) => endpoint.Get12MonthExpensesSummaryListAsync(farmId))
    .WithMetadata(new RequiresPermissionAttribute("Report.Read"))
    .RequireAuthorization("Permission");

genExpensesKpisGroup.MapGet("/trend-list-12-months/{farmId}/previous", ([FromServices] GeneralExpenseReportEndpoints endpoint, Guid farmId) => endpoint.Get12MonthExpensesSummaryListPreviousAsync(farmId))
    .WithMetadata(new RequiresPermissionAttribute("Report.Read"))
    .RequireAuthorization("Permission");


var productPerfKpisGroup = app.MapGroup("/api/product-performances");
productPerfKpisGroup.MapGet("/{farmId}/{dateStart}/{dateEnd}", ([FromServices] ProductPerformanceEndpoints endpoint, Guid farmId, DateTime dateStart, DateTime dateEnd) => endpoint.GetProductPerformancesAsync(farmId, dateStart, dateEnd))
    .WithMetadata(new RequiresPermissionAttribute("Report.Read"))
    .RequireAuthorization("Permission");


productPerfKpisGroup.MapGet("/year/{farmId}/{year}", ([FromServices] ProductPerformanceEndpoints endpoint, Guid farmId, int year) => endpoint.GetProductPerformancesByYearAsync(farmId, year))
    .WithMetadata(new RequiresPermissionAttribute("Report.Read"))
    .RequireAuthorization("Permission");



var receiptGroup = app.MapGroup("/api/receipts");
receiptGroup.MapGet("/{paymentId}", ([FromServices] ReceiptEndpoints endpoint, string paymentId) => endpoint.GetReceiptByIdAsync(paymentId))
    .WithMetadata(new RequiresPermissionAttribute("Report.Read"))
    .RequireAuthorization("Permission");

receiptGroup.MapPost("/generate", ([FromServices] ReceiptEndpoints endpoint, PaymentDetailsDto dto) => endpoint.GenerateReceiptAsync(dto))
    .WithMetadata(new RequiresPermissionAttribute("Report.Read"))
    .RequireAuthorization("Permission");

app.Run();