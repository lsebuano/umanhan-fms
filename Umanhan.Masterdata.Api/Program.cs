using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;
using Umanhan.Masterdata.Api;
using Umanhan.Masterdata.Api.Endpoints;
using Umanhan.Models;
using Umanhan.Models.Attributes;
using Umanhan.Models.Dtos;
using Umanhan.Models.Models;
using Umanhan.Models.Validators;
using Umanhan.Repositories;
using Umanhan.Repositories.Interfaces;
using Umanhan.Services;
using Umanhan.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddMemoryCache();
builder.Services.AddOutputCache();

// Register FluentValidation
builder.Services.AddValidatorsFromAssemblyContaining<CategoryValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<CropValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<CustomerTypeValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<CustomerValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<ExpenseTypeValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<FarmValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<InventoryValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<LaborerValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<LivestockValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<PaymentTypeValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<ProductTypeValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<SoilTypeValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<StaffValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<TaskValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<TransactionTypeValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<UnitValidator>();
builder.Services.AddFluentValidationAutoValidation();

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
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<ICropRepository, CropRepository>();
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<ICustomerTypeRepository, CustomerTypeRepository>();
builder.Services.AddScoped<IExpenseTypeRepository, ExpenseTypeRepository>();
builder.Services.AddScoped<IFarmRepository, FarmRepository>();
builder.Services.AddScoped<IInventoryRepository, InventoryRepository>();
builder.Services.AddScoped<ILaborerRepository, LaborerRepository>();
builder.Services.AddScoped<IModuleRepository, ModuleRepository>();
builder.Services.AddScoped<ILivestockRepository, LivestockRepository>();
builder.Services.AddScoped<IPaymentTypeRepository, PaymentTypeRepository>();
builder.Services.AddScoped<IPermissionRepository, PermissionRepository>();
builder.Services.AddScoped<IPricingConditionTypeRepository, PricingConditionTypeRepository>();
builder.Services.AddScoped<IProductTypeRepository, ProductTypeRepository>();
builder.Services.AddScoped<ISoilTypeRepository, SoilTypeRepository>();
builder.Services.AddScoped<IStaffRepository, StaffRepository>();
builder.Services.AddScoped<ITaskRepository, TaskRepository>();
builder.Services.AddScoped<ITransactionTypeRepository, TransactionTypeRepository>();
builder.Services.AddScoped<IUnitRepository, UnitRepository>();

// validators
builder.Services.AddScoped<IValidator<CategoryDto>, CategoryValidator>();
builder.Services.AddScoped<IValidator<CropDto>, CropValidator>();
builder.Services.AddScoped<IValidator<CustomerDto>, CustomerValidator>();
builder.Services.AddScoped<IValidator<CustomerTypeDto>, CustomerTypeValidator>();
builder.Services.AddScoped<IValidator<ExpenseTypeDto>, ExpenseTypeValidator>();
builder.Services.AddScoped<IValidator<FarmDto>, FarmValidator>();
builder.Services.AddScoped<IValidator<InventoryDto>, InventoryValidator>();
builder.Services.AddScoped<IValidator<LaborerDto>, LaborerValidator>();
builder.Services.AddScoped<IValidator<LivestockDto>, LivestockValidator>();
builder.Services.AddScoped<IValidator<ModuleDto>, ModuleValidator>();
builder.Services.AddScoped<IValidator<PermissionDto>, PermissionValidator>();
builder.Services.AddScoped<IValidator<PaymentTypeDto>, PaymentTypeValidator>();
builder.Services.AddScoped<IValidator<ProductTypeDto>, ProductTypeValidator>();
builder.Services.AddScoped<IValidator<SoilTypeDto>, SoilTypeValidator>();
builder.Services.AddScoped<IValidator<StaffDto>, StaffValidator>();
builder.Services.AddScoped<IValidator<TaskDto>, TaskValidator>();
builder.Services.AddScoped<IValidator<TransactionTypeDto>, TransactionTypeValidator>();
builder.Services.AddScoped<IValidator<UnitDto>, UnitValidator>();

// services
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IUserContextService, UserContextService>();
builder.Services.AddScoped<IPermissionService, PermissionService>();
builder.Services.AddScoped<RolePermissionService>();

//builder.Services.AddScoped<CategoryService>();
builder.Services.AddScoped<CropService>();
builder.Services.AddScoped<CustomerService>();
builder.Services.AddScoped<CustomerTypeService>();
builder.Services.AddScoped<ExpenseTypeService>();
builder.Services.AddScoped<FarmService>();
builder.Services.AddScoped<InventoryService>();
builder.Services.AddScoped<LaborerService>();
builder.Services.AddScoped<LivestockService>();
builder.Services.AddScoped<ModuleService>();
builder.Services.AddScoped<PaymentTypeService>();
builder.Services.AddScoped<PricingConditionTypeService>();
builder.Services.AddScoped<ProductService>();
builder.Services.AddScoped<ProductTypeService>();
builder.Services.AddScoped<SoilTypeService>();
builder.Services.AddScoped<StaffService>();
builder.Services.AddScoped<TaskService>();
builder.Services.AddScoped<TransactionTypeService>();
builder.Services.AddScoped<UnitService>();

// endpoints
builder.Services.AddScoped<CategoryEndpoints>();
builder.Services.AddScoped<CropEndpoints>();
builder.Services.AddScoped<CustomerEndpoints>();
builder.Services.AddScoped<CustomerTypeEndpoints>();
builder.Services.AddScoped<ExpenseTypeEndpoints>();
builder.Services.AddScoped<FarmEndpoints>();
builder.Services.AddScoped<InventoryEndpoints>();
builder.Services.AddScoped<LaborerEndpoints>();
builder.Services.AddScoped<LivestockEndpoints>();
builder.Services.AddScoped<ModuleEndpoints>();
builder.Services.AddScoped<PermissionEndpoints>();
builder.Services.AddScoped<PaymentTypeEndpoints>();
builder.Services.AddScoped<PricingConditionTypeEndpoints>();
builder.Services.AddScoped<ProductEndpoints>();
builder.Services.AddScoped<ProductTypeEndpoints>();
builder.Services.AddScoped<SoilTypeEndpoints>();
builder.Services.AddScoped<StaffEndpoints>();
builder.Services.AddScoped<TaskEndpoints>();
builder.Services.AddScoped<TransactionTypeEndpoints>();
builder.Services.AddScoped<UnitEndpoints>();

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

var cognitoAuthority = Environment.GetEnvironmentVariable("COGNITO_AUTHORITY") ?? throw new InvalidOperationException("Missing environment variable: COGNITO_AUTHORITY");
var cognitoAudience = Environment.GetEnvironmentVariable("COGNITO_AUDIENCE") ?? throw new InvalidOperationException("Missing environment variable: COGNITO_AUDIENCE");

// Configure CORS
string[] allowedDomains = Environment.GetEnvironmentVariable("ALLOWED_ORIGINS")?.Split(",") ?? [];

//string apiPathBase = Environment.GetEnvironmentVariable("API_PATH_BASE") ?? throw new InvalidOperationException("Missing environment variable: API_PATH_BASE");

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

// Configure the HTTP request pipelinec.

//app.UsePathBase(apiPathBase);
app.UseRouting();

app.UseHttpsRedirection();
app.UseCors("CORSPolicy");
app.UseAuthentication();
app.UseAuthorization();
//app.UseMiddleware<NoCacheHeaderMiddleware>();
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

var categoryGroup = app.MapGroup("/api/categories");
categoryGroup.MapGet("/", (CategoryEndpoints endpoint) => endpoint.GetAllCategoriesAsync())
    .WithMetadata(new RequiresPermissionAttribute("Masterdata.Read"))
    .RequireAuthorization("Permission");

categoryGroup.MapGet("/{id}", (CategoryEndpoints endpoint, Guid id) => endpoint.GetCategoryByIdAsync(id))
    .WithMetadata(new RequiresPermissionAttribute("Masterdata.Read"))
    .RequireAuthorization("Permission");

categoryGroup.MapPost("/", (CategoryEndpoints endpoint, CategoryDto category) => endpoint.CreateCategoryAsync(category))
    .WithMetadata(new RequiresPermissionAttribute("Masterdata.Write"))
    .RequireAuthorization("Permission");

categoryGroup.MapPut("/{id}", (CategoryEndpoints endpoint, Guid id, CategoryDto category) => endpoint.UpdateCategoryAsync(id, category))
    .WithMetadata(new RequiresPermissionAttribute("Masterdata.Write"))
    .RequireAuthorization("Permission");

categoryGroup.MapDelete("/{id}", (CategoryEndpoints endpoint, Guid id) => endpoint.DeleteCategoryAsync(id))
    .WithMetadata(new RequiresPermissionAttribute("Masterdata.Write"))
    .RequireAuthorization("Permission");

var cropGroup = app.MapGroup("/api/crops");
cropGroup.MapGet("/", (CropEndpoints endpoint) => endpoint.GetAllCropsAsync())
    .WithMetadata(new RequiresPermissionAttribute("Masterdata.Read"))
    .RequireAuthorization("Permission");

cropGroup.MapGet("/{id}", (CropEndpoints endpoint, Guid id) => endpoint.GetCropByIdAsync(id))
    .WithMetadata(new RequiresPermissionAttribute("Masterdata.Read"))
    .RequireAuthorization("Permission");

cropGroup.MapPost("/", (CropEndpoints endpoint, CropDto crop) => endpoint.CreateCropAsync(crop))
    .WithMetadata(new RequiresPermissionAttribute("Masterdata.Write"))
    .RequireAuthorization("Permission");

cropGroup.MapPut("/{id}", (CropEndpoints endpoint, Guid id, CropDto crop) => endpoint.UpdateCropAsync(id, crop))
    .WithMetadata(new RequiresPermissionAttribute("Masterdata.Write"))
    .RequireAuthorization("Permission");

cropGroup.MapDelete("/{id}", (CropEndpoints endpoint, Guid id) => endpoint.DeleteCropAsync(id))
    .WithMetadata(new RequiresPermissionAttribute("Masterdata.Write"))
    .RequireAuthorization("Permission");

var customerGroup = app.MapGroup("/api/customers");
customerGroup.MapGet("/", (CustomerEndpoints endpoint) => endpoint.GetAllCustomersAsync())
    .WithMetadata(new RequiresPermissionAttribute("Masterdata.Read"))
    .RequireAuthorization("Permission");

customerGroup.MapGet("/{id}", (CustomerEndpoints endpoint, Guid id) => endpoint.GetCustomerByIdAsync(id))
    .WithMetadata(new RequiresPermissionAttribute("Masterdata.Read"))
    .RequireAuthorization("Permission");

customerGroup.MapGet("/contract-eligible", (CustomerEndpoints endpoint) => endpoint.GetCustomersContractEligibleAsync())
    .WithMetadata(new RequiresPermissionAttribute("Masterdata.Read"))
    .RequireAuthorization("Permission");

customerGroup.MapGet("/type/{customerTypeId}", (CustomerEndpoints endpoint, Guid customerTypeId) => endpoint.GetCustomersByTypeAsync(customerTypeId))
    .WithMetadata(new RequiresPermissionAttribute("Masterdata.Read"))
    .RequireAuthorization("Permission");

customerGroup.MapPost("/", (CustomerEndpoints endpoint, CustomerDto customer) => endpoint.CreateCustomerAsync(customer))
    .WithMetadata(new RequiresPermissionAttribute("Masterdata.Write"))
    .RequireAuthorization("Permission");

customerGroup.MapPut("/{id}", (CustomerEndpoints endpoint, Guid id, CustomerDto customer) => endpoint.UpdateCustomerAsync(id, customer))
    .WithMetadata(new RequiresPermissionAttribute("Masterdata.Write"))
    .RequireAuthorization("Permission");

customerGroup.MapDelete("/{id}", (CustomerEndpoints endpoint, Guid id) => endpoint.DeleteCustomerAsync(id))
    .WithMetadata(new RequiresPermissionAttribute("Masterdata.Write"))
    .RequireAuthorization("Permission");

var customerTypeGroup = app.MapGroup("/api/customer-types");
customerTypeGroup.MapGet("/", (CustomerTypeEndpoints endpoint) => endpoint.GetAllCustomerTypesAsync())
    .WithMetadata(new RequiresPermissionAttribute("Masterdata.Read"))
    .RequireAuthorization("Permission");

customerTypeGroup.MapGet("/{id}", (CustomerTypeEndpoints endpoint, Guid id) => endpoint.GetCustomerTypeByIdAsync(id))
    .WithMetadata(new RequiresPermissionAttribute("Masterdata.Read"))
    .RequireAuthorization("Permission");

customerTypeGroup.MapPost("/", (CustomerTypeEndpoints endpoint, CustomerTypeDto customerType) => endpoint.CreateCustomerTypeAsync(customerType))
    .WithMetadata(new RequiresPermissionAttribute("Masterdata.Write"))
    .RequireAuthorization("Permission");

customerTypeGroup.MapPut("/{id}", (CustomerTypeEndpoints endpoint, Guid id, CustomerTypeDto customerType) => endpoint.UpdateCustomerTypeAsync(id, customerType))
    .WithMetadata(new RequiresPermissionAttribute("Masterdata.Write"))
    .RequireAuthorization("Permission");

customerTypeGroup.MapDelete("/{id}", (CustomerTypeEndpoints endpoint, Guid id) => endpoint.DeleteCustomerTypeAsync(id))
    .WithMetadata(new RequiresPermissionAttribute("Masterdata.Write"))
    .RequireAuthorization("Permission");

var expenseTypeGroup = app.MapGroup("/api/expense-types");
expenseTypeGroup.MapGet("/", (ExpenseTypeEndpoints endpoint) => endpoint.GetAllExpenseTypesAsync())
    .WithMetadata(new RequiresPermissionAttribute("Masterdata.Read"))
    .RequireAuthorization("Permission");

expenseTypeGroup.MapGet("/{id}", (ExpenseTypeEndpoints endpoint, Guid id) => endpoint.GetExpenseTypeByIdAsync(id))
    .WithMetadata(new RequiresPermissionAttribute("Masterdata.Read"))
    .RequireAuthorization("Permission");

expenseTypeGroup.MapPost("/", (ExpenseTypeEndpoints endpoint, ExpenseTypeDto expenseType) => endpoint.CreateExpenseTypeAsync(expenseType))
    .WithMetadata(new RequiresPermissionAttribute("Masterdata.Write"))
    .RequireAuthorization("Permission");

expenseTypeGroup.MapPut("/{id}", (ExpenseTypeEndpoints endpoint, Guid id, ExpenseTypeDto expenseType) => endpoint.UpdateExpenseTypeAsync(id, expenseType))
    .WithMetadata(new RequiresPermissionAttribute("Masterdata.Write"))
    .RequireAuthorization("Permission");

expenseTypeGroup.MapDelete("/{id}", (ExpenseTypeEndpoints endpoint, Guid id) => endpoint.DeleteExpenseTypeAsync(id))
    .WithMetadata(new RequiresPermissionAttribute("Masterdata.Write"))
    .RequireAuthorization("Permission");

var farmGroup = app.MapGroup("/api/farms");
farmGroup.MapGet("/", (FarmEndpoints endpoint) => endpoint.GetAllFarmsAsync())
    .WithMetadata(new RequiresPermissionAttribute("Masterdata.Read"))
    .RequireAuthorization("Permission");

farmGroup.MapGet("/{id}", (FarmEndpoints endpoint, Guid id) => endpoint.GetFarmByIdAsync(id))
    .WithMetadata(new RequiresPermissionAttribute("Masterdata.Read"))
    .RequireAuthorization("Permission");

farmGroup.MapGet("/{id}/setup-complete", ([FromServices] FarmEndpoints endpoint, Guid id) => endpoint.IsFarmSetupCompleteAsync(id))
    .WithMetadata(new RequiresPermissionAttribute("Masterdata.Read"))
    .RequireAuthorization("Permission");

farmGroup.MapPost("/", (FarmEndpoints endpoint, FarmDto farm) => endpoint.CreateFarmAsync(farm))
    .WithMetadata(new RequiresPermissionAttribute("Masterdata.Write"))
    .RequireAuthorization("Permission");

farmGroup.MapPost("/complete-setup", (FarmEndpoints endpoint, FarmSetupDto farm) => endpoint.CompleteFarmSetupAsync(farm))
    .WithMetadata(new RequiresPermissionAttribute("Masterdata.Write"))
    .RequireAuthorization("Permission");

farmGroup.MapPut("/{id}", (FarmEndpoints endpoint, Guid id, FarmDto farm) => endpoint.UpdateFarmAsync(id, farm))
    .WithMetadata(new RequiresPermissionAttribute("Masterdata.Write"))
    .RequireAuthorization("Permission");

farmGroup.MapDelete("/{id}", (FarmEndpoints endpoint, Guid id) => endpoint.DeleteFarmAsync(id))
    .WithMetadata(new RequiresPermissionAttribute("Masterdata.Write"))
    .RequireAuthorization("Permission");

var inventoryGroup = app.MapGroup("/api/inventories");
inventoryGroup.MapGet("/", (InventoryEndpoints endpoint) => endpoint.GetAllInventoriesAsync())
    .WithMetadata(new RequiresPermissionAttribute("Masterdata.Read"))
    .RequireAuthorization("Permission");

inventoryGroup.MapGet("/{id}", (InventoryEndpoints endpoint, Guid id) => endpoint.GetInventoryByIdAsync(id))
    .WithMetadata(new RequiresPermissionAttribute("Masterdata.Read"))
    .RequireAuthorization("Permission");

inventoryGroup.MapPost("/", (InventoryEndpoints endpoint, InventoryDto inventory) => endpoint.CreateInventoryAsync(inventory))
    .WithMetadata(new RequiresPermissionAttribute("Masterdata.Write"))
    .RequireAuthorization("Permission");

inventoryGroup.MapPut("/{id}", (InventoryEndpoints endpoint, Guid id, InventoryDto inventory) => endpoint.UpdateInventoryAsync(id, inventory))
    .WithMetadata(new RequiresPermissionAttribute("Masterdata.Write"))
    .RequireAuthorization("Permission");

inventoryGroup.MapDelete("/{id}", (InventoryEndpoints endpoint, Guid id) => endpoint.DeleteInventoryAsync(id))
    .WithMetadata(new RequiresPermissionAttribute("Masterdata.Write"))
    .RequireAuthorization("Permission");

var laborerGroup = app.MapGroup("/api/laborers");
laborerGroup.MapGet("/", (LaborerEndpoints endpoint) => endpoint.GetAllLaborersAsync())
    .WithMetadata(new RequiresPermissionAttribute("Masterdata.Read"))
    .RequireAuthorization("Permission");

laborerGroup.MapGet("/{id}", (LaborerEndpoints endpoint, Guid id) => endpoint.GetLaborerByIdAsync(id))
    .WithMetadata(new RequiresPermissionAttribute("Masterdata.Read"))
    .RequireAuthorization("Permission");

laborerGroup.MapPost("/", (LaborerEndpoints endpoint, LaborerDto laborer) => endpoint.CreateLaborerAsync(laborer))
    .WithMetadata(new RequiresPermissionAttribute("Masterdata.Write"))
    .RequireAuthorization("Permission");

laborerGroup.MapPut("/{id}", (LaborerEndpoints endpoint, Guid id, LaborerDto laborer) => endpoint.UpdateLaborerAsync(id, laborer))
    .WithMetadata(new RequiresPermissionAttribute("Masterdata.Write"))
    .RequireAuthorization("Permission");

laborerGroup.MapDelete("/{id}", (LaborerEndpoints endpoint, Guid id) => endpoint.DeleteLaborerAsync(id))
    .WithMetadata(new RequiresPermissionAttribute("Masterdata.Write"))
    .RequireAuthorization("Permission");

var moduleGroup = app.MapGroup("/api/modules");
moduleGroup.MapGet("/", (ModuleEndpoints endpoint) => endpoint.GetAllModulesAsync())
    .WithMetadata(new RequiresPermissionAttribute("Masterdata.Read"))
    .RequireAuthorization("Permission");

moduleGroup.MapGet("/{id}", (ModuleEndpoints endpoint, Guid id) => endpoint.GetModuleByIdAsync(id))
    .WithMetadata(new RequiresPermissionAttribute("Masterdata.Read"))
    .RequireAuthorization("Permission");

moduleGroup.MapPost("/", (ModuleEndpoints endpoint, ModuleDto module) => endpoint.CreateModuleAsync(module))
    .WithMetadata(new RequiresPermissionAttribute("Masterdata.Write"))
    .RequireAuthorization("Permission");

moduleGroup.MapPut("/{id}", (ModuleEndpoints endpoint, Guid id, ModuleDto module) => endpoint.UpdateModuleAsync(id, module))
    .WithMetadata(new RequiresPermissionAttribute("Masterdata.Write"))
    .RequireAuthorization("Permission");

moduleGroup.MapDelete("/{id}", (ModuleEndpoints endpoint, Guid id) => endpoint.DeleteModuleAsync(id))
    .WithMetadata(new RequiresPermissionAttribute("Masterdata.Write"))
    .RequireAuthorization("Permission");

var permissionGroup = app.MapGroup("/api/permissions");
permissionGroup.MapGet("/", (PermissionEndpoints endpoint) => endpoint.GetAllPermissionsAsync())
    .WithMetadata(new RequiresPermissionAttribute("Masterdata.Read"))
    .RequireAuthorization("Permission");

permissionGroup.MapGet("/{id}", (PermissionEndpoints endpoint, Guid id) => endpoint.GetPermissionByIdAsync(id))
    .WithMetadata(new RequiresPermissionAttribute("Masterdata.Read"))
    .RequireAuthorization("Permission");

permissionGroup.MapPost("/", (PermissionEndpoints endpoint, PermissionDto permission) => endpoint.CreatePermissionAsync(permission))
    .WithMetadata(new RequiresPermissionAttribute("Masterdata.Write"))
    .RequireAuthorization("Permission");

permissionGroup.MapPut("/{id}", (PermissionEndpoints endpoint, Guid id, PermissionDto permission) => endpoint.UpdatePermissionAsync(id, permission))
    .WithMetadata(new RequiresPermissionAttribute("Masterdata.Write"))
    .RequireAuthorization("Permission");

permissionGroup.MapDelete("/{id}", (PermissionEndpoints endpoint, Guid id) => endpoint.DeletePermissionAsync(id))
    .WithMetadata(new RequiresPermissionAttribute("Masterdata.Write"))
    .RequireAuthorization("Permission");

//var livestockGroup = app.MapGroup("/api/livestocks"); 
//livestockGroup.MapGet("/", (LivestockEndpoints endpoint) => endpoint.GetAllLivestocksAsync(context)).RequireAuthorization();
//livestockGroup.MapGet("/{id}", (LivestockEndpoints endpoint, Guid id) => endpoint.GetLivestockByIdAsync(context, id)).RequireAuthorization();
//livestockGroup.MapPost("/", (LivestockEndpoints endpoint, LivestockDto livestock) => endpoint.CreateLivestockAsync(context, livestock)).RequireAuthorization();
//livestockGroup.MapPut("/{id}", (LivestockEndpoints endpoint, Guid id, LivestockDto livestock) => endpoint.UpdateLivestockAsync(context, id, livestock)).RequireAuthorization();
//livestockGroup.MapDelete("/{id}", (LivestockEndpoints endpoint, Guid id) => endpoint.DeleteLivestockAsync(context, id)).RequireAuthorization();

var paymentTypeGroup = app.MapGroup("/api/payment-types");
paymentTypeGroup.MapGet("/", (PaymentTypeEndpoints endpoint) => endpoint.GetAllPaymentTypesAsync())
    .WithMetadata(new RequiresPermissionAttribute("Masterdata.Read"))
    .RequireAuthorization("Permission");

paymentTypeGroup.MapGet("/{id}", (PaymentTypeEndpoints endpoint, Guid id) => endpoint.GetPaymentTypeByIdAsync(id))
    .WithMetadata(new RequiresPermissionAttribute("Masterdata.Read"))
    .RequireAuthorization("Permission");

paymentTypeGroup.MapPost("/", (PaymentTypeEndpoints endpoint, PaymentTypeDto paymentType) => endpoint.CreatePaymentTypeAsync(paymentType))
    .WithMetadata(new RequiresPermissionAttribute("Masterdata.Write"))
    .RequireAuthorization("Permission");

paymentTypeGroup.MapPut("/{id}", (PaymentTypeEndpoints endpoint, Guid id, PaymentTypeDto paymentType) => endpoint.UpdatePaymentTypeAsync(id, paymentType))
    .WithMetadata(new RequiresPermissionAttribute("Masterdata.Write"))
    .RequireAuthorization("Permission");

paymentTypeGroup.MapDelete("/{id}", (PaymentTypeEndpoints endpoint, Guid id) => endpoint.DeletePaymentTypeAsync(id))
    .WithMetadata(new RequiresPermissionAttribute("Masterdata.Write"))
    .RequireAuthorization("Permission");

var productTypeGroup = app.MapGroup("/api/product-types");
productTypeGroup.MapGet("/", (ProductTypeEndpoints endpoint) => endpoint.GetAllProductTypesAsync())
    .WithMetadata(new RequiresPermissionAttribute("Masterdata.Read"))
    .RequireAuthorization("Permission");

productTypeGroup.MapGet("/{id}", (ProductTypeEndpoints endpoint, Guid id) => endpoint.GetProductTypeByIdAsync(id))
    .WithMetadata(new RequiresPermissionAttribute("Masterdata.Read"))
    .RequireAuthorization("Permission");

productTypeGroup.MapPost("/", (ProductTypeEndpoints endpoint, ProductTypeDto productType) => endpoint.CreateProductTypeAsync(productType))
    .WithMetadata(new RequiresPermissionAttribute("Masterdata.Write"))
    .RequireAuthorization("Permission");

productTypeGroup.MapPut("/{id}", (ProductTypeEndpoints endpoint, Guid id, ProductTypeDto productType) => endpoint.UpdateProductTypeAsync(id, productType))
    .WithMetadata(new RequiresPermissionAttribute("Masterdata.Write"))
    .RequireAuthorization("Permission");

productTypeGroup.MapDelete("/{id}", (ProductTypeEndpoints endpoint, Guid id) => endpoint.DeleteProductTypeAsync(id))
    .WithMetadata(new RequiresPermissionAttribute("Masterdata.Write"))
    .RequireAuthorization("Permission");

var productGroup = app.MapGroup("/api/products");
productGroup.MapGet("/farm-id/{farmId}/type-id/{typeId}", (ProductEndpoints endpoint, Guid farmId, Guid typeId) => endpoint.GetProductsByFarmByTypeAsync(farmId, typeId))
    .WithMetadata(new RequiresPermissionAttribute("Masterdata.Read"))
    .RequireAuthorization("Permission");

productGroup.MapGet("/{id}/type-id/{typeId}", (ProductEndpoints endpoint, Guid typeId, Guid id) => endpoint.GetProductByIdAsync(typeId, id))
    .WithMetadata(new RequiresPermissionAttribute("Masterdata.Read"))
    .RequireAuthorization("Permission");

productGroup.MapGet("/farm-id/{farmId}", (ProductEndpoints endpoint, Guid farmId) => endpoint.GetProductsByFarmAsync(farmId))
    .WithMetadata(new RequiresPermissionAttribute("Masterdata.Read"))
    .RequireAuthorization("Permission");

var soilTypeGroup = app.MapGroup("/api/soil-types");
soilTypeGroup.MapGet("/", (SoilTypeEndpoints endpoint) => endpoint.GetAllSoilTypesAsync())
    .WithMetadata(new RequiresPermissionAttribute("Masterdata.Read"))
    .RequireAuthorization("Permission");

soilTypeGroup.MapGet("/{id}", (SoilTypeEndpoints endpoint, Guid id) => endpoint.GetSoilTypeByIdAsync(id))
    .WithMetadata(new RequiresPermissionAttribute("Masterdata.Read"))
    .RequireAuthorization("Permission");

soilTypeGroup.MapPost("/", (SoilTypeEndpoints endpoint, SoilTypeDto soilType) => endpoint.CreateSoilTypeAsync(soilType))
    .WithMetadata(new RequiresPermissionAttribute("Masterdata.Write"))
    .RequireAuthorization("Permission");

soilTypeGroup.MapPut("/{id}", (SoilTypeEndpoints endpoint, Guid id, SoilTypeDto soilType) => endpoint.UpdateSoilTypeAsync(id, soilType))
    .WithMetadata(new RequiresPermissionAttribute("Masterdata.Write"))
    .RequireAuthorization("Permission");

soilTypeGroup.MapDelete("/{id}", (SoilTypeEndpoints endpoint, Guid id) => endpoint.DeleteSoilTypeAsync(id))
    .WithMetadata(new RequiresPermissionAttribute("Masterdata.Write"))
    .RequireAuthorization("Permission");

var staffGroup = app.MapGroup("/api/staffs");
staffGroup.MapGet("/", (StaffEndpoints endpoint) => endpoint.GetAllStaffsAsync())
    .WithMetadata(new RequiresPermissionAttribute("Masterdata.Read"))
    .RequireAuthorization("Permission");

staffGroup.MapGet("/{id}", (StaffEndpoints endpoint, Guid id) => endpoint.GetStaffByIdAsync(id))
    .WithMetadata(new RequiresPermissionAttribute("Masterdata.Read"))
    .RequireAuthorization("Permission");

staffGroup.MapGet("/farm-id/{farmId}", (StaffEndpoints endpoint, Guid farmId) => endpoint.GetStaffsByFarmAsync(farmId))
    .WithMetadata(new RequiresPermissionAttribute("Masterdata.Read"))
    .RequireAuthorization("Permission");

staffGroup.MapPost("/", (StaffEndpoints endpoint, StaffDto staff) => endpoint.CreateStaffAsync(staff))
    .WithMetadata(new RequiresPermissionAttribute("Masterdata.Write"))
    .RequireAuthorization("Permission");

staffGroup.MapPut("/{id}", (StaffEndpoints endpoint, Guid id, StaffDto staff) => endpoint.UpdateStaffAsync(id, staff))
    .WithMetadata(new RequiresPermissionAttribute("Masterdata.Write"))
    .RequireAuthorization("Permission");

staffGroup.MapDelete("/{id}", (StaffEndpoints endpoint, Guid id) => endpoint.DeleteStaffAsync(id))
    .WithMetadata(new RequiresPermissionAttribute("Masterdata.Write"))
    .RequireAuthorization("Permission");

//var settingGroup = app.MapGroup("/api/system-settings");
//settingGroup.MapGet("/", (SystemSettingEndpoints endpoint) => endpoint.GetAllSystemSettingsAsync()).RequireAuthorization();
//settingGroup.MapGet("/{id}", (SystemSettingEndpoints endpoint, Guid id) => endpoint.GetSystemSettingByIdAsync(id)).RequireAuthorization();
//settingGroup.MapGet("/name/{name}", (SystemSettingEndpoints endpoint, string name) => endpoint.GetSystemSettingByNameAsync(name)).RequireAuthorization();

//settingGroup.MapPost("/", (SystemSettingEndpoints endpoint, SystemSettingDto setting) => endpoint.CreateSystemSettingAsync(setting))
//    .WithMetadata(new RequiresPermissionAttribute("Masterdata.Write"))
//    .RequireAuthorization("Permission");

//settingGroup.MapPut("/{id}", (SystemSettingEndpoints endpoint, Guid id, SystemSettingDto setting) => endpoint.UpdateSystemSettingAsync(id, setting))
//    .WithMetadata(new RequiresPermissionAttribute("Masterdata.Write"))
//    .RequireAuthorization("Permission");

//settingGroup.MapDelete("/{id}", (SystemSettingEndpoints endpoint, Guid id) => endpoint.DeleteSystemSettingAsync(id))
//    .WithMetadata(new RequiresPermissionAttribute("Masterdata.Write"))
//    .RequireAuthorization("Permission");

var taskGroup = app.MapGroup("/api/tasks");
taskGroup.MapGet("/", (TaskEndpoints endpoint) => endpoint.GetAllTasksAsync())
    .WithMetadata(new RequiresPermissionAttribute("Masterdata.Read"))
    .RequireAuthorization("Permission");

taskGroup.MapGet("/{id}", (TaskEndpoints endpoint, Guid id) => endpoint.GetTaskByIdAsync(id))
    .WithMetadata(new RequiresPermissionAttribute("Masterdata.Read"))
    .RequireAuthorization("Permission");

taskGroup.MapGet("/getby/{categoryId}", (TaskEndpoints endpoint, Guid categoryId) => endpoint.GetTasksByCategoryAsync(categoryId))
    .WithMetadata(new RequiresPermissionAttribute("Masterdata.Read"))
    .RequireAuthorization("Permission");

taskGroup.MapGet("/cat-group-activities", (TaskEndpoints endpoint) => endpoint.GetTasksForFarmActivitiesAsync())
    .WithMetadata(new RequiresPermissionAttribute("Masterdata.Read"))
    .RequireAuthorization("Permission");

taskGroup.MapPost("/", (TaskEndpoints endpoint, TaskDto task) => endpoint.CreateTaskAsync(task))
    .WithMetadata(new RequiresPermissionAttribute("Masterdata.Write"))
    .RequireAuthorization("Permission");

taskGroup.MapPut("/{id}", (TaskEndpoints endpoint, Guid id, TaskDto task) => endpoint.UpdateTaskAsync(id, task))
    .WithMetadata(new RequiresPermissionAttribute("Masterdata.Write"))
    .RequireAuthorization("Permission");

taskGroup.MapPut("/unassign", (TaskEndpoints endpoint, TaskDto task) => endpoint.UnassignTaskToCategoryAsync(task))
    .WithMetadata(new RequiresPermissionAttribute("Masterdata.Write"))
    .RequireAuthorization("Permission");

taskGroup.MapDelete("/{id}", (TaskEndpoints endpoint, Guid id) => endpoint.DeleteTaskAsync(id))
    .WithMetadata(new RequiresPermissionAttribute("Masterdata.Write"))
    .RequireAuthorization("Permission");

var transactionTypeGroup = app.MapGroup("/api/transaction-types");
transactionTypeGroup.MapGet("/", (TransactionTypeEndpoints endpoint) => endpoint.GetAllTransactionTypesAsync())
    .WithMetadata(new RequiresPermissionAttribute("Masterdata.Read"))
    .RequireAuthorization("Permission");

transactionTypeGroup.MapGet("/{id}", (TransactionTypeEndpoints endpoint, Guid id) => endpoint.GetTransactionTypeByIdAsync(id))
    .WithMetadata(new RequiresPermissionAttribute("Masterdata.Read"))
    .RequireAuthorization("Permission");

transactionTypeGroup.MapPost("/", (TransactionTypeEndpoints endpoint, TransactionTypeDto transactionType) => endpoint.CreateTransactionTypeAsync(transactionType))
    .WithMetadata(new RequiresPermissionAttribute("Masterdata.Write"))
    .RequireAuthorization("Permission");

transactionTypeGroup.MapPut("/{id}", (TransactionTypeEndpoints endpoint, Guid id, TransactionTypeDto transactionType) => endpoint.UpdateTransactionTypeAsync(id, transactionType))
    .WithMetadata(new RequiresPermissionAttribute("Masterdata.Write"))
    .RequireAuthorization("Permission");

transactionTypeGroup.MapDelete("/{id}", (TransactionTypeEndpoints endpoint, Guid id) => endpoint.DeleteTransactionTypeAsync(id))
    .WithMetadata(new RequiresPermissionAttribute("Masterdata.Write"))
    .RequireAuthorization("Permission");

var unitGroup = app.MapGroup("/api/units");
unitGroup.MapGet("/", (UnitEndpoints endpoint) => endpoint.GetAllUnitsAsync())
    .WithMetadata(new RequiresPermissionAttribute("Masterdata.Read"))
    .RequireAuthorization("Permission");

unitGroup.MapGet("/{id}", (UnitEndpoints endpoint, Guid id) => endpoint.GetUnitByIdAsync(id))
    .WithMetadata(new RequiresPermissionAttribute("Masterdata.Read"))
    .RequireAuthorization("Permission");

unitGroup.MapPost("/", (UnitEndpoints endpoint, UnitDto unit) => endpoint.CreateUnitAsync(unit))
    .WithMetadata(new RequiresPermissionAttribute("Masterdata.Write"))
    .RequireAuthorization("Permission");

unitGroup.MapPut("/{id}", (UnitEndpoints endpoint, Guid id, UnitDto unit) => endpoint.UpdateUnitAsync(id, unit))
    .WithMetadata(new RequiresPermissionAttribute("Masterdata.Write"))
    .RequireAuthorization("Permission");

unitGroup.MapDelete("/{id}", (UnitEndpoints endpoint, Guid id) => endpoint.DeleteUnitAsync(id))
    .WithMetadata(new RequiresPermissionAttribute("Masterdata.Write"))
    .RequireAuthorization("Permission");

var pricingConditionTypeGroup = app.MapGroup("/api/condition-types");
pricingConditionTypeGroup.MapGet("/", (PricingConditionTypeEndpoints endpoint) => endpoint.GetPricingConditionTypesAsync())
    .WithMetadata(new RequiresPermissionAttribute("Masterdata.Read"))
    .RequireAuthorization("Permission");

pricingConditionTypeGroup.MapGet("/{id}", (PricingConditionTypeEndpoints endpoint, Guid id) => endpoint.GetPricingConditionTypeByIdAsync(id))
    .WithMetadata(new RequiresPermissionAttribute("Masterdata.Read"))
    .RequireAuthorization("Permission");

app.Run();