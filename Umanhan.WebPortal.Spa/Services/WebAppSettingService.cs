//using Newtonsoft.Json;
using System.Net.Http.Json;
using Umanhan.Models.Dtos;
using Umanhan.Models.Models;
using static System.Net.WebRequestMethods;

namespace Umanhan.WebPortal.Spa.Services
{
    public class WebAppSettingService
    {
        private readonly ApiService _apiService;

        public WebAppSettingService(ApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<WebAppSetting> LoadAsync()
        {
            try
            {
                var response = await _apiService.GetAsync<WebAppSetting>("SecretsAPI", "api/secrets/fresh");
                //Console.WriteLine($"WebAppSettingService.LoadAsync: {JsonConvert.SerializeObject(response)}");
                if (response.IsSuccess && response.Data != null)
                {
                    return response.Data;
                }
                else
                {
                    Console.WriteLine($"Load Error: Failed to load web app settings.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Load Error: {ex.Message}");
            }
            return new WebAppSetting();
        }
    }
}
