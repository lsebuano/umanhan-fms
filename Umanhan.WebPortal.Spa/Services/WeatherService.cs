using Umanhan.Models.Dtos;
using Umanhan.Models.Models;

namespace Umanhan.WebPortal.Spa.Services
{
    public class WeatherService
    {
        private readonly ApiService _apiService;

        public WeatherService(ApiService apiService)
        {
            _apiService = apiService;
        }

        public Task<ApiResponse<WeatherData>> GetWeatherDataAsync(double lat, double lng)
        {
            try
            {
                return _apiService.GetAsync<WeatherData>("WeatherAPI", $"api/weather/{lat}/{lng}");
            }
            catch (Exception ex)
            {

            }
            return null;
        }

        public Task<ApiResponse<List<ForecastDailyWeather>>> GetWeatherForecastDataAsync(double lat, double lng)
        {
            try
            {
                return _apiService.GetAsync<List<ForecastDailyWeather>>("WeatherAPI", $"api/weather/forecast/{lat}/{lng}");
            }
            catch (Exception ex)
            {

            }
            return null;
        }
    }
}
