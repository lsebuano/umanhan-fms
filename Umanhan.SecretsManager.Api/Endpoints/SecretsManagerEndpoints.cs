using Amazon;
using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;
using Microsoft.AspNetCore.DataProtection;
using System.ComponentModel.DataAnnotations;
using System.Security;
using System.Text;
using System.Text.Json;
using Umanhan.Models.Models;
using Umanhan.Services.Interfaces;
using static System.Net.WebRequestMethods;

namespace Umanhan.SecretsManager.Api.Endpoints
{
    public class SecretsManagerEndpoints
    {
        private readonly ILogger<SecretsManagerEndpoints> _logger;
        //private readonly ICacheService _cacheService;

        private const string MODULE_CACHE_KEY = "secrets";

        private static async Task<GetSecretValueResponse> GetSecretAsync(IAmazonSecretsManager client, string secretName)
        {
            GetSecretValueRequest request = new GetSecretValueRequest()
            {
                SecretId = secretName,
                VersionStage = "AWSCURRENT", // VersionStage defaults to AWSCURRENT if unspecified.
            };

            GetSecretValueResponse response = null;

            // For the sake of simplicity, this example handles only the most
            // general SecretsManager exception.
            try
            {
                response = await client.GetSecretValueAsync(request);
            }
            catch (AmazonSecretsManagerException e)
            {
                Console.WriteLine($"Error: {e.Message}");
            }
            return response;
        }

        private static string DecodeString(GetSecretValueResponse response)
        {
            // Decrypts secret using the associated AWS Key Management Service
            // Customer Master Key (CMK.) Depending on whether the secret is a
            // string or binary value, one of these fields will be populated.
            if (response.SecretString is not null)
            {
                var secret = response.SecretString;
                return secret;
            }
            else if (response.SecretBinary is not null)
            {
                var memoryStream = response.SecretBinary;
                StreamReader reader = new StreamReader(memoryStream);
                string decodedBinarySecret = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(reader.ReadToEnd()));
                return decodedBinarySecret;
            }
            else
            {
                return string.Empty;
            }
        }

        public SecretsManagerEndpoints(ILogger<SecretsManagerEndpoints> logger)
        {
            _logger = logger;
            //_cacheService = cacheService;
        }

        public async Task<IResult> GetUmanhanAppSecretsAsync()
        {
            //var key = $"{MODULE_CACHE_KEY}:list";
            //var result = await _cacheService.GetOrSetAsync(key, async () =>
            //{
            string secretName = Environment.GetEnvironmentVariable("AWS_SECRETS_NAME") ?? throw new InvalidOperationException("Missing environment variable: AWS_SECRET_NAME");
            IAmazonSecretsManager client = new AmazonSecretsManagerClient();
            var result = await GetSecretAsync(client, secretName);
            //}, TimeSpan.FromSeconds(60));

            if (result is not null)
            {
                var secret = DecodeString(result);
                if (!string.IsNullOrEmpty(secret))
                {
                    var config = new ConfigurationBuilder().AddJsonStream(new MemoryStream(Encoding.UTF8.GetBytes(secret))).Build();
                    var settings = new WebAppSetting();
                    config.Bind(settings);

                    return Results.Ok(settings);
                }
                else
                {
                    Console.WriteLine("No secret value was returned.");
                }
            }
            return Results.Problem("Failed to retrieve secret value.");
        }

        public IResult GetGoogleMapsApiKey(HttpContext context)
        {
            var apiKey = Environment.GetEnvironmentVariable("GOOGLE_MAPS_API_KEY") ?? throw new InvalidOperationException("Missing environment variable: GOOGLE_MAPS_API_KEY");
            context.Response.Headers["Cache-Control"] = "no-store";
            return Results.Json(apiKey);
        }

        public IResult GetGoogleMapsLoader(HttpContext context)
        {
            //if (!AuthorizationHelper.IsAuthorized(context))
            //    return Results.Forbid();

            var mapsUrl = Environment.GetEnvironmentVariable("GOOGLE_MAPS_API_URL") ?? throw new InvalidOperationException("Missing environment variable: GOOGLE_MAPS_API_URL");
            var script = $@"
                (function(){{
                    var script = document.createElement('script');
                    script.src = '{mapsUrl}';
                    script.async = true;
                    document.body.appendChild(script);
                }})();
            ";

            context.Response.Headers["Cache-Control"] = "no-store";
            return Results.Text(script, "application/javascript");
        }

        public async Task<IResult> UpdateSecretValueAsync(KeyValuePair<string, string> value)
        {
            try
            {
                string secretName = Environment.GetEnvironmentVariable("AWS_SECRETS_NAME");

                var client = new AmazonSecretsManagerClient();
                var currentSecret = await client.GetSecretValueAsync(new GetSecretValueRequest
                {
                    SecretId = secretName,
                    VersionStage = "AWSCURRENT"
                }).ConfigureAwait(false);

                var dict = JsonSerializer.Deserialize<Dictionary<string, string>>(currentSecret.SecretString);
                if (dict == null)
                {
                    _logger.LogError("Failed to deserialize current secret value for {SecretName}", secretName);
                    return Results.Problem("Failed to deserialize current secret value.");
                }

                dict[value.Key] = value.Value;
                var updatedSecretString = JsonSerializer.Serialize(dict);
                var putRequest = new PutSecretValueRequest
                {
                    SecretId = secretName,
                    SecretString = updatedSecretString,
                    VersionStages = new List<string> { "AWSCURRENT" }
                };

                var response = await client.PutSecretValueAsync(putRequest).ConfigureAwait(false);
                if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
                {
                    return Results.Ok(true);
                }
                else
                {
                    _logger.LogError("Failed to update secret value for {SecretName}. Status code: {StatusCode}", secretName, response.HttpStatusCode);
                    return Results.Problem("Failed to update secret value.");
                }
            }
            catch (Exception ex)
            {
                string msg = "Unable to update the secret value.";
                _logger.LogError(ex, msg);
                return Results.Problem(msg);
            }
        }
    }
}
