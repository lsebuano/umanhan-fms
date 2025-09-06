using Umanhan.Dtos.HelperModels;

namespace Umanhan.WebPortal.Spa.Services
{
    public class NlpService
    {
        private readonly ApiService _apiService;
        private readonly SystemSettingService _systemSettingService;
        private readonly FarmService _farmService;

        public NlpService(ApiService apiService, SystemSettingService systemSettingService, FarmService farmService)
        {
            _apiService = apiService;
            _systemSettingService = systemSettingService;
            _farmService = farmService;
        }

        private async Task<string[]> GetFarmCropsAsync()
        {
            Guid farmId = _systemSettingService.GetSetting<Guid>(SettingName.DEFAULT_FARM.ToString());
            if (farmId != Guid.Empty)
                return [];

            var result = await _farmService.GetFarmByIdAsync(farmId).ConfigureAwait(false);
            if (result.IsSuccess)
            {
                var farm = result.Data;
                if (farm == null)
                    return [];
                return farm.FarmCrops.Select(x => x.CropName).ToArray();
            }

            return [];
        }

        public async Task<string> AnalyzeWeatherDataAsync(string weatherDataJson)
        {
            try
            {
                var cropsList = await GetFarmCropsAsync().ConfigureAwait(false);
                string crops = string.Join(", ", cropsList);
                var data = new UserChatRequest
                {
                    Input = weatherDataJson,
                    Crops = crops
                };
                var response = await _apiService.PostAsync<UserChatRequest, string>("NlpAPI", $"api/chat/weather-analysis", data).ConfigureAwait(false);
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
            return null;
        }

        public async Task<string> GenerateWeatherHeadlineAsync(DateTime date, string city, float tempAverage, float tempMin,
                                                               float tempMax, double humidity, double windSpeed,
                                                               double windDirection, double windGust)
        {
            try
            {
                var data = new ForecastDailyWeather
                {
                    Date = date,
                    City = city,
                    TempAverage = tempAverage,
                    TempMin = tempMin,
                    TempMax = tempMax,
                    Humidity = humidity,
                    WindSpeed = windSpeed,
                    WindDirection = windDirection,
                    WindGust = windGust,
                };
                var response = await _apiService.PostAsync<ForecastDailyWeather, string>("NlpAPI", $"api/chat/weather-headline", data).ConfigureAwait(false);
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
                return response.Data?.Replace("\"", string.Empty);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<string> GenerateDataInsightAsync(string prompt, string jsonData)
        {
            try
            {
                if (string.IsNullOrEmpty(jsonData))
                    throw new ArgumentException("JSON data cannot be null or empty.", nameof(jsonData));


                var response = await _apiService.PostAsync<AnalyzeDataRequest, string>("NlpAPI", $"api/chat/analyze-data/custom",
                    new AnalyzeDataRequest
                    {
                        ConvoId = Math.Abs(jsonData.GetHashCode()).ToString(),
                        Data = jsonData,
                        Prompt = prompt
                    }).ConfigureAwait(false);

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
                return response.Data?.Replace("\"", string.Empty);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
