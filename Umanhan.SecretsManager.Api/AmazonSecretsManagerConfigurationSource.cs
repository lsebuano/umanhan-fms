namespace Umanhan.SecretsManager.Api
{
    public class AmazonSecretsManagerConfigurationSource : IConfigurationSource
    {
        private readonly string _region;
        private readonly string _secretName;
        private readonly TimeSpan _refreshInterval;

        public AmazonSecretsManagerConfigurationSource(string region, string secretName, TimeSpan refreshInterval)
        {
            _region = region;
            _secretName = secretName;
            _refreshInterval = refreshInterval;
        }

        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            return new AmazonSecretsManagerConfigurationProvider(_region, _secretName, _refreshInterval);
        }
    }
}
