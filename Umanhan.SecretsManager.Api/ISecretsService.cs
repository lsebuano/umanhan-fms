namespace Umanhan.SecretsManager.Api
{
    public interface ISecretsService
    {
        Task<Dictionary<string, string>> GetKeyValuesAsync();
    }
}
