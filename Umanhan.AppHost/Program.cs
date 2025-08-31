var builder = DistributedApplication.CreateBuilder(args);

//var sampleApi = builder.AddProject<Projects.Umanhan_Api>("sample-api");
var usersApi = builder.AddProject<Projects.Umanhan_UserManager_Api>("users-api");
var opsApi = builder.AddProject<Projects.Umanhan_Operations_Api>("ops-api");
var masterDataApi = builder.AddProject<Projects.Umanhan_Masterdata_Api>("masterdata-api");
var weatherApi = builder.AddProject<Projects.Umanhan_Weather_Api>("weather-api");
var reportsApi = builder.AddProject<Projects.Umanhan_Reports_Api>("reports-api");
var nlpApi = builder.AddProject<Projects.Umanhan_Nlp_Api>("nlp-api");
var secretsApi = builder.AddProject<Projects.Umanhan_SecretsManager_Api>("secrets-api");
//var loggerApi = builder.AddProject<Projects.Umanhan_Logger_Api>("logger-api");
var logsApi = builder.AddProject<Projects.Umanhan_Logs_Api>("logs-api");
var settingsApi = builder.AddProject<Projects.Umanhan_Settings_Api>("settings-api");

builder.AddProject<Projects.Umanhan_WebPortal_Spa>("web-portal")
       //.WithReference(sampleApi)
       .WithReference(usersApi)
       .WithReference(opsApi)
       .WithReference(masterDataApi)
       .WithReference(weatherApi)
       .WithReference(reportsApi)
       .WithReference(secretsApi)
       .WithReference(nlpApi)
       //.WithReference(loggerApi);
       .WithReference(logsApi)
       .WithReference(settingsApi);

builder.Build().Run();