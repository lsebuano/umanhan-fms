using Amazon.SecretsManager.Model;
using Amazon.SecretsManager;
using System.Text.Json;
using Amazon;
using Microsoft.Extensions.Primitives;

// Source: https://aws.amazon.com/blogs/modernizing-with-aws/how-to-load-net-configuration-from-aws-secrets-manager/
namespace Umanhan.SecretsManager.Api
{
    public class AmazonSecretsManagerConfigurationProvider : ConfigurationProvider, IDisposable
    {
        private readonly string _region;
        private readonly string _secretName;
        private readonly PeriodicWatcher _watcher;

        public AmazonSecretsManagerConfigurationProvider(string region, string secretName, TimeSpan refreshInterval)
        {
            _region = region;
            _secretName = secretName;
            _watcher = new PeriodicWatcher(refreshInterval);
            ChangeToken.OnChange(() => _watcher.Watch(), () =>
            {
                try
                {
                    Load();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("AmazonSecretsManagerConfigurationProvider Error: {ex}", ex);
                }
            });
        }

        public override void Load()
        {
            var secret = GetSecretAsync().GetAwaiter().GetResult();
            if (secret != null)
                Data = JsonSerializer.Deserialize<Dictionary<string, string>>(secret) ?? new Dictionary<string, string>();
            else
                Data = new Dictionary<string, string>();
        }

        private async Task<string> GetSecretAsync()
        {
            var request = new GetSecretValueRequest
            {
                SecretId = _secretName,
                VersionStage = "AWSCURRENT" // VersionStage defaults to AWSCURRENT if unspecified.
            };

            try
            {
                using (var client = new AmazonSecretsManagerClient(RegionEndpoint.GetBySystemName(_region)))
                {
                    var response = await client.GetSecretValueAsync(request).ConfigureAwait(false);

                    string secretString;
                    if (response.SecretString != null)
                    {
                        secretString = response.SecretString;
                    }
                    else
                    {
                        var memoryStream = response.SecretBinary;
                        var reader = new StreamReader(memoryStream);
                        secretString = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(reader.ReadToEnd()));
                    }

                    return secretString;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("AmazonSecretsManagerConfigurationProvider.GetSecretAsync Error: {ex}", ex);
            }
            return null;
        }

        public void Dispose() => _watcher.Dispose();
    }
}
