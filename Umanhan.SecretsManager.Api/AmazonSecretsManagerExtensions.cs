namespace Umanhan.SecretsManager.Api
{
    public static class AmazonSecretsManagerExtensions
    {
        public static void AddAmazonSecretsManager(this IConfigurationBuilder configurationBuilder, string region, string secretName, TimeSpan? reloadInterval = null)
        {
            var configurationSource = new AmazonSecretsManagerConfigurationSource(region, secretName, reloadInterval ?? TimeSpan.FromMinutes(5));
            configurationBuilder.Add(configurationSource);
        }

        public static void AddAmazonSecretsManagerLazy(this IServiceCollection services, string region, string secretName, TimeSpan? reloadInterval = null)
        {
            services.AddSingleton<ISecretsService>(sp =>
                new SecretsService(region, secretName, reloadInterval ?? TimeSpan.FromMinutes(5)));
        }
    }
}
