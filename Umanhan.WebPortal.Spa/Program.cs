using Blazored.LocalStorage;
using FluentValidation;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Radzen;
using Umanhan.Dtos;
using Umanhan.Dtos.HelperModels;
using Umanhan.Dtos.Validators;
using Umanhan.WebPortal.Spa.Services;
using Umanhan.WebPortal.Spa.Services.Interfaces;

namespace Umanhan.WebPortal.Spa;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebAssemblyHostBuilder.CreateDefault(args);
        builder.RootComponents.Add<App>("#app");
        builder.RootComponents.Add<HeadOutlet>("head::after");

        builder.Configuration.AddJsonFile($"appsettings.{builder.HostEnvironment.Environment}.json", optional: true);
        builder.Logging.AddConfiguration(builder.Configuration.GetSection("Logging"));

        Console.WriteLine($"Environment: {builder.HostEnvironment.Environment}");

        // hide radzen logging in prod
        if (builder.HostEnvironment.IsProduction())
        {
            builder.Logging.AddFilter("Microsoft.AspNetCore.Components.RenderTree.Renderer", LogLevel.None);
        }

        builder.Services.AddScoped(sp => new HttpClient
        {
            BaseAddress = new Uri(builder.HostEnvironment.BaseAddress)
        });
        //builder.Services.AddHttpClient<RolePermissionService>(client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress))
        //                .AddHttpMessageHandler<BaseAddressAuthorizationMessageHandler>();

        builder.Services.AddHttpClient("cfg", client =>
        {
            client.BaseAddress = new Uri(builder.Configuration["Amazon:AwsSecretsBaseUrl"]);
        });

        builder.Services.AddBlazoredLocalStorage();

        WebAppSetting webAppSettings = new();

        webAppSettings.FarmId = Guid.Parse(builder.Configuration["Settings:FarmId"]);
        webAppSettings.CognitoDomain = builder.Configuration["Settings:CognitoDomain"];
        webAppSettings.CognitoClientId = builder.Configuration["Settings:CognitoClientId"];
        webAppSettings.CognitoAuthority = builder.Configuration["Settings:CognitoAuthority"];
        webAppSettings.CognitoMetadataUrl = builder.Configuration["Settings:CognitoMetadataUrl"];
        webAppSettings.CognitoResponseType = builder.Configuration["Settings:CognitoResponseType"];
        webAppSettings.CognitoRedirectUri = builder.Configuration["Settings:CognitoRedirectUri"];
        webAppSettings.CognitoReturnUrl = builder.Configuration["Settings:CognitoReturnUrl"];

        webAppSettings.WebApiUrlOperations = builder.Configuration["Settings:WebApiUrlOperations"];
        webAppSettings.WebApiUrlMasterdata = builder.Configuration["Settings:WebApiUrlMasterdata"];
        webAppSettings.WebApiUrlWeather = builder.Configuration["Settings:WebApiUrlWeather"];
        webAppSettings.WebApiUrlNlp = builder.Configuration["Settings:WebApiUrlNlp"];
        webAppSettings.WebApiUrlUsers = builder.Configuration["Settings:WebApiUrlUsers"];
        webAppSettings.WebApiUrlReport = builder.Configuration["Settings:WebApiUrlReport"];
        webAppSettings.WebApiUrlLogs = builder.Configuration["Settings:WebApiUrlLogs"];
        webAppSettings.WebApiUrlSecrets = builder.Configuration["Settings:WebApiUrlSecrets"];
        webAppSettings.WebApiUrlSettings = builder.Configuration["Settings:WebApiUrlSettings"];

        webAppSettings.GoogleMapsApiKey = builder.Configuration["Settings:GoogleMapsApiKey"];

        builder.Services.AddSingleton(webAppSettings);

        builder.Services.AddScoped<ApiService>();
        builder.Services.AddScoped<SystemSettingService>();
        builder.Services.AddScoped<UserStateService>();

        // business:masterdata services
        builder.Services.AddScoped<CategoryService>();
        builder.Services.AddScoped<ChangeLogService>();
        builder.Services.AddScoped<CropService>();
        builder.Services.AddScoped<CustomerService>();
        builder.Services.AddScoped<CustomerTypeService>();
        builder.Services.AddScoped<ExpenseTypeService>();
        builder.Services.AddScoped<FarmActivityService>();
        builder.Services.AddScoped<FarmCropService>();
        builder.Services.AddScoped<FarmExpenseService>();
        builder.Services.AddScoped<FarmActivityLaborerService>();
        builder.Services.AddScoped<FarmActivityUsageService>();
        builder.Services.AddScoped<FarmContractDetailService>();
        builder.Services.AddScoped<FarmContractService>();
        builder.Services.AddScoped<FarmInventoryService>();
        builder.Services.AddScoped<FarmTransactionService>();
        builder.Services.AddScoped<FarmService>();
        builder.Services.AddScoped<FarmZoneService>();
        builder.Services.AddScoped<InventoryService>();
        builder.Services.AddScoped<LaborerService>();
        builder.Services.AddScoped<LoggerService>();
        //builder.Services.AddScoped<LivestockService>();
        builder.Services.AddScoped<ModuleService>();
        builder.Services.AddScoped<PermService>();
        builder.Services.AddScoped<NlpService>();
        builder.Services.AddScoped<PaymentTypeService>();
        builder.Services.AddScoped<PricingProfileService>();
        builder.Services.AddScoped<PricingService>();
        builder.Services.AddScoped<PricingConditionTypeService>();
        builder.Services.AddScoped<ProductService>();
        builder.Services.AddScoped<ProductTypeService>();
        builder.Services.AddScoped<SoilTypeService>();
        builder.Services.AddScoped<StaffService>();
        builder.Services.AddScoped<TaskService>();
        builder.Services.AddScoped<TransactionTypeService>();
        builder.Services.AddScoped<UnitService>();
        builder.Services.AddScoped<UserService>();
        builder.Services.AddScoped<ReportService>();
        builder.Services.AddScoped<RoleService>();
        builder.Services.AddScoped<RolePermissionService>();
        builder.Services.AddScoped<UserActivityService>();
        builder.Services.AddScoped<WeatherService>();
        builder.Services.AddScoped<QuotationService>();
        builder.Services.AddScoped<SecretService>();
        builder.Services.AddScoped<FarmActivityPhotoService>();
        builder.Services.AddScoped<FarmGeneralExpenseService>();
        builder.Services.AddScoped<ContractPaymentService>();
        builder.Services.AddScoped<QueryLogService>();
        builder.Services.AddScoped<FarmGeneralExpenseReceiptService>();

        // validators
        builder.Services.AddScoped<IValidator<CategoryDto>, CategoryValidator>();
        builder.Services.AddScoped<IValidator<CropDto>, CropValidator>();
        builder.Services.AddScoped<IValidator<CustomerDto>, CustomerValidator>();
        builder.Services.AddScoped<IValidator<CustomerTypeDto>, CustomerTypeValidator>();
        builder.Services.AddScoped<IValidator<ExpenseTypeDto>, ExpenseTypeValidator>();
        builder.Services.AddScoped<IValidator<FarmActivityDto>, FarmActivityValidator>();
        builder.Services.AddScoped<IValidator<FarmActivityLaborerDto>, FarmActivityLaborerValidator>();
        builder.Services.AddScoped<IValidator<FarmActivityUsageDto>, FarmActivityUsageValidator>();
        builder.Services.AddScoped<IValidator<FarmCropDto>, FarmCropValidator>();
        builder.Services.AddScoped<IValidator<FarmInventoryDto>, FarmInventoryValidator>();
        builder.Services.AddScoped<IValidator<FarmDto>, FarmValidator>();
        builder.Services.AddScoped<IValidator<FarmSetupDto>, FarmSetupValidator>();
        builder.Services.AddScoped<IValidator<FarmContractDetailDto>, FarmContractDetailValidator>();
        builder.Services.AddScoped<IValidator<FarmContractDto>, FarmContractValidator>();
        builder.Services.AddScoped<IValidator<FarmTransactionDto>, FarmTransactionValidator>();
        builder.Services.AddScoped<IValidator<FarmZoneDto>, FarmZoneValidator>();
        builder.Services.AddScoped<IValidator<InventoryDto>, InventoryValidator>();
        builder.Services.AddScoped<IValidator<LaborerDto>, LaborerValidator>();
        builder.Services.AddScoped<IValidator<LogDto>, LogValidator>();
        //builder.Services.AddScoped<IValidator<LivestockDto>, LivestockValidator>();
        builder.Services.AddScoped<IValidator<PaymentTypeDto>, PaymentTypeValidator>();
        builder.Services.AddScoped<IValidator<PricingDto>, PricingConditionValidator>();
        builder.Services.AddScoped<IValidator<PricingProfileDto>, PricingProfileValidator>();
        builder.Services.AddScoped<IValidator<ProductTypeDto>, ProductTypeValidator>();
        builder.Services.AddScoped<IValidator<SoilTypeDto>, SoilTypeValidator>();
        builder.Services.AddScoped<IValidator<StaffDto>, StaffValidator>();
        builder.Services.AddScoped<IValidator<SystemSettingDto>, SystemSettingValidator>();
        builder.Services.AddScoped<IValidator<TaskDto>, TaskValidator>();
        builder.Services.AddScoped<IValidator<TransactionTypeDto>, TransactionTypeValidator>();
        builder.Services.AddScoped<IValidator<UnitDto>, UnitValidator>();
        builder.Services.AddScoped<IValidator<UserDto>, UserValidator>();
        builder.Services.AddScoped<IValidator<RoleDto>, RoleValidator>();
        builder.Services.AddScoped<IValidator<ModuleDto>, ModuleValidator>();
        builder.Services.AddScoped<IValidator<PermissionDto>, PermissionValidator>();
        builder.Services.AddScoped<IValidator<RolePermissionDto>, RolePermissionValidator>();
        builder.Services.AddScoped<IValidator<UserActivityDto>, UserActivityValidator>();
        builder.Services.AddScoped<IValidator<QuotationDto>, QuotationValidator>();
        builder.Services.AddScoped<IValidator<QuotationDetailDto>, QuotationDetailValidator>();
        builder.Services.AddScoped<IValidator<QuotationProductDto>, QuotationProductValidator>();
        builder.Services.AddScoped<IValidator<FarmActivityPhotoDto>, FarmActivityPhotoValidator>();
        builder.Services.AddScoped<IValidator<FarmGeneralExpenseDto>, FarmGeneralExpenseValidator>();
        builder.Services.AddScoped<IValidator<PaymentDetailsDto>, PaymentDetailsValidator>();
        builder.Services.AddScoped<IValidator<FarmGeneralExpenseReceiptDto>, FarmGeneralExpenseReceiptValidator>();

        // business:operations services

        // radzen services
        builder.Services.AddScoped<DialogService>();
        builder.Services.AddScoped<NotificationService>();
        builder.Services.AddScoped<AppNotificationService>();

        //builder.Services.AddBlazoredLocalStorage();
        //builder.Services.AddBlazoredSessionStorage();
        builder.Services.AddRadzenComponents();

        //string cognitoUserPoolId = webAppSettings.CognitoUserPoolId;

        builder.Services.AddHttpClient("OperationsAPI", client =>
        {
            string operationsApiUrl = webAppSettings.WebApiUrlOperations;
            client.BaseAddress = new Uri(operationsApiUrl);
        });
        builder.Services.AddHttpClient("MasterdataAPI", client =>
        {
            string masterdataApiUrl = webAppSettings.WebApiUrlMasterdata;
            client.BaseAddress = new Uri(masterdataApiUrl);
        });
        builder.Services.AddHttpClient("WeatherAPI", client =>
        {
            string weatherApiUrl = webAppSettings.WebApiUrlWeather;
            client.BaseAddress = new Uri(weatherApiUrl);
        });
        builder.Services.AddHttpClient("NlpAPI", client =>
        {
            string nlpApiUrl = webAppSettings.WebApiUrlNlp;
            client.BaseAddress = new Uri(nlpApiUrl);
            client.Timeout = TimeSpan.FromMinutes(5);
        });
        builder.Services.AddHttpClient("UsersAPI", client =>
        {
            string usersApiUrl = webAppSettings.WebApiUrlUsers;
            client.BaseAddress = new Uri(usersApiUrl);
        });
        //builder.Services.AddHttpClient("LoggerAPI", client =>
        //{
        //    string loggerApiUrl = webAppSettings.WebApiUrlLogger"];
        //    client.BaseAddress = new Uri(loggerApiUrl);
        //});
        builder.Services.AddHttpClient("ReportAPI", client =>
        {
            string reportApiUrl = webAppSettings.WebApiUrlReport;
            client.BaseAddress = new Uri(reportApiUrl);
            client.Timeout = TimeSpan.FromMinutes(5);
        });
        builder.Services.AddHttpClient("LogsAPI", client =>
        {
            string logApiUrl = webAppSettings.WebApiUrlLogs;
            client.BaseAddress = new Uri(logApiUrl);
            client.Timeout = TimeSpan.FromMinutes(5);
        });
        builder.Services.AddHttpClient("SecretsAPI", client =>
        {
            string secretsApiUrl = webAppSettings.WebApiUrlSecrets;
            client.BaseAddress = new Uri(secretsApiUrl);
            client.Timeout = TimeSpan.FromMinutes(5);
        });
        builder.Services.AddHttpClient("SettingsAPI", client =>
        {
            string settingsApiUrl = webAppSettings.WebApiUrlSettings;
            client.BaseAddress = new Uri(settingsApiUrl);
            client.Timeout = TimeSpan.FromMinutes(5);
        });

        //var http2 = builder.Services.BuildServiceProvider().GetRequiredService<IHttpClientFactory>().CreateClient("MasterdataAPI");
        //var list = await http2.GetFromJsonAsync<IEnumerable<SystemSettingDto>>("/api/system-settings").ConfigureAwait(false);
        //var systemSettings = list?.ToDictionary(x => x.SettingName, x => x.SettingValue) ?? new Dictionary<string, string>();
        //var systemSettings = new List<SystemSettingDto>();
        //#if DEBUG
        //        systemSettings.Add(new SystemSettingDto
        //        {
        //            SettingId = Guid.NewGuid(),
        //            SettingName = "TestSetting",
        //            SettingValue = "Loaded from on app start"
        //        });
        //#endif
        //builder.Services.AddSingleton(systemSettings);

        builder.Services.AddOidcAuthentication(options =>
        {
            string cognitoAppClientId = webAppSettings.CognitoClientId;
            string authority = webAppSettings.CognitoAuthority;
            string metadataUrl = webAppSettings.CognitoMetadataUrl;
            string responseType = webAppSettings.CognitoResponseType;
            string redirectUri = webAppSettings.CognitoRedirectUri;

            // Basic configuration
            options.ProviderOptions.Authority = authority;
            options.ProviderOptions.MetadataUrl = metadataUrl;
            options.ProviderOptions.ClientId = cognitoAppClientId;
            options.ProviderOptions.ResponseType = responseType;

            // Configure scopes
            options.ProviderOptions.DefaultScopes.Clear();
            options.ProviderOptions.DefaultScopes.Add("openid");
            options.ProviderOptions.DefaultScopes.Add("email");
            //options.ProviderOptions.DefaultScopes.Add("profile");
            // Configure callbacks
            options.ProviderOptions.RedirectUri = redirectUri;

            // Map Cognito claims to standard claims
            options.UserOptions.NameClaim = "email";  // Use email as the Name claim
            options.UserOptions.RoleClaim = "cognito:groups";
        });

        builder.Services.AddScoped<AccountClaimsPrincipalFactory<RemoteUserAccount>, CustomClaimsFactory>();
        builder.Services.AddScoped<IPermissionService, PermissionService>();

        await builder.Build().RunAsync();
    }
}