//using Blazored.SessionStorage;
//using Blazored.SessionStorage;
namespace Umanhan.WebPortal.Spa.Services
{
    public class SecretService
    {
        private readonly ApiService _apiService;

        public SecretService(ApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<bool> UpdateSecretAsync(KeyValuePair<string, string> secret)
        {
            try
            {
                var response = await _apiService.PutAsync<KeyValuePair<string, string>, bool>("SecretsAPI", $"api/secrets", secret).ConfigureAwait(false);
                if (!response.IsSuccess)
                {
                    if (response.Errors != null && response.Errors.Any())
                    {
                        var errors = new Dictionary<string, List<string>>();
                        foreach (var error in response.Errors)
                        {
                            errors.Add(error.Key, error.Value);
                        }
                        response.ErrorMessage = string.Join(", ", errors.SelectMany(x => x.Value));
                    }
                }
                return response.Data;
            }
            catch (Exception ex)
            {

            }
            return false;
        }
    }
}
