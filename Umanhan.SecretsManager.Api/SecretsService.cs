using Amazon;
using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;
using System.Text.Json;

namespace Umanhan.SecretsManager.Api
{
    public class SecretsService : ISecretsService, IDisposable
    {
        private readonly string _region;
        private readonly string _secretName;
        private readonly TimeSpan _refreshInterval;
        private Dictionary<string, string> _cachedValues = new();
        private readonly Timer _timer;

        public SecretsService(string region, string secretName, TimeSpan refreshInterval)
        {
            _region = region;
            _secretName = secretName;
            _refreshInterval = refreshInterval;

            // Trigger background loading
            _ = RefreshSecretAsync();

            // Refresh periodically
            _timer = new Timer(async _ => await RefreshSecretAsync(), null, _refreshInterval, _refreshInterval);
        }

        public async Task<Dictionary<string, string>> GetKeyValuesAsync()
        {
            if (_cachedValues.Count == 0)
            {
                await RefreshSecretAsync();
            }

            return _cachedValues;
        }

        private async Task RefreshSecretAsync()
        {
            try
            {
                using var client = new AmazonSecretsManagerClient(RegionEndpoint.GetBySystemName(_region));

                var response = await client.GetSecretValueAsync(new GetSecretValueRequest
                {
                    SecretId = _secretName,
                    VersionStage = "AWSCURRENT"
                });

                string? raw = response.SecretString ?? DecodeBinary(response.SecretBinary);

                if (raw != null)
                {
                    _cachedValues = JsonSerializer.Deserialize<Dictionary<string, string>>(raw)
                                    ?? new Dictionary<string, string>();
                }
                else
                {
                    _cachedValues = new Dictionary<string, string>();
                }

                Console.WriteLine($"[SecretsService] Refreshed at {DateTime.UtcNow}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[SecretsService] Error: {ex.Message}");
            }
        }

        private string DecodeBinary(MemoryStream binaryStream)
        {
            using var reader = new StreamReader(binaryStream);
            var base64 = reader.ReadToEnd();
            return System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(base64));
        }

        public void Dispose() => _timer?.Dispose();
    }
}