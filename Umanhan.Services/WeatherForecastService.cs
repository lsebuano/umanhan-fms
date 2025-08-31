using Microsoft.Extensions.Logging;
using Umanhan.Models.Dtos;
using Umanhan.Models.Entities;
using Umanhan.Models.Models;
using Umanhan.Repositories.Interfaces;
using Umanhan.Services.Interfaces;
using Task = System.Threading.Tasks.Task;

namespace Umanhan.Services
{
    public class WeatherForecastService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserContextService _userContext;
        private readonly ILogger<WeatherForecastService> _logger;

        public WeatherForecastService(IUnitOfWork unitOfWork,
            IUserContextService userContext,
            ILogger<WeatherForecastService> logger)
        {
            this._unitOfWork = unitOfWork;
            this._userContext = userContext;
            this._logger = logger;
        }

        private static List<WeatherForecastDto> ToWeatherForecastDto(IEnumerable<WeatherForecast> userActivities)
        {
            return userActivities.Select(x => new WeatherForecastDto
            {
                Cloudiness = x.Cloudiness,
                Humidity = x.Humidity,
                Date = x.Date,
                Lat = x.Lat,
                Lng = x.Lng,
                TemperatureMax = x.TemperatureMax,
                TemperatureMin = x.TemperatureMin,
                TemperatureAverage = x.TemperatureAverage,
                WindAngleDegree = x.WindAngleDegree,
                WindGust = x.WindGust,
                WindSpeed = x.WindSpeed
            })
            .OrderBy(x => x.Date)
            .ToList();
        }

        private static WeatherForecastDto ToWeatherForecastDto(WeatherForecast weatherForecast)
        {
            return new WeatherForecastDto
            {
                Cloudiness = weatherForecast.Cloudiness,
                Humidity = weatherForecast.Humidity,
                Date = weatherForecast.Date,
                Lat = weatherForecast.Lat,
                Lng = weatherForecast.Lng,
                TemperatureMax = weatherForecast.TemperatureMax,
                TemperatureMin = weatherForecast.TemperatureMin,
                TemperatureAverage = weatherForecast.TemperatureAverage,
                WindAngleDegree = weatherForecast.WindAngleDegree,
                WindGust = weatherForecast.WindGust,
            };
        }

        public async Task<WeatherForecastDto> GetWeatherForecastByIdAsync(Guid id, params string[] includeEntities)
        {
            var obj = await _unitOfWork.WeatherForecasts.GetByIdAsync(id, includeEntities).ConfigureAwait(false);
            if (obj == null)
                return null;
            return ToWeatherForecastDto(obj);
        }

        public async Task CreateWeatherForecastsAsync(IEnumerable<ForecastDailyWeather> weatherForecasts, double lat, double lng)
        {
            var newWeatherForecasts = weatherForecasts.Select(weatherForecast => new WeatherForecast
            {
                Cloudiness = weatherForecast.Cloudiness,
                Humidity = weatherForecast.Humidity,
                Date = DateOnly.FromDateTime(weatherForecast.Date),
                Lat = lat,
                Lng = lng,
                TemperatureMax = weatherForecast.TempMax,
                TemperatureMin = weatherForecast.TempMin,
                TemperatureAverage = weatherForecast.TempAverage,
                WindAngleDegree = weatherForecast.WindDirection,
                WindGust = weatherForecast.WindGust,
                WindSpeed = weatherForecast.WindSpeed,
            });

            try
            {
                await _unitOfWork.WeatherForecasts.AddRangeAsync(newWeatherForecasts).ConfigureAwait(false);
                await _unitOfWork.CommitAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating weather forecasts.");
                throw;
            }
        }

        public async Task UpsertWeatherForecastsAsync(IEnumerable<ForecastDailyWeather> weatherForecasts, double lat, double lng)
        {
            var newWeatherForecasts = weatherForecasts.Select(weatherForecast => new WeatherForecast
            {
                Cloudiness = weatherForecast.Cloudiness,
                Humidity = weatherForecast.Humidity,
                Date = DateOnly.FromDateTime(weatherForecast.Date),
                Lat = lat,
                Lng = lng,
                TemperatureMax = weatherForecast.TempMax,
                TemperatureMin = weatherForecast.TempMin,
                TemperatureAverage = weatherForecast.TempAverage,
                WindAngleDegree = weatherForecast.WindDirection,
                WindGust = weatherForecast.WindGust,
                WindSpeed = weatherForecast.WindSpeed,
            });

            try
            {
                await _unitOfWork.WeatherForecasts.UpsertAsync(newWeatherForecasts).ConfigureAwait(false);                
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating weather forecasts.");
                throw;
            }
        }

        public async Task CreateUpdateWeatherForecastsAsync(IEnumerable<ForecastDailyWeather> weatherForecasts, double lat, double lng)
        {
            try
            {
                // Pre-process: Map raw forecasts into entity objects
                var newWeatherForecasts = weatherForecasts.Select(weatherForecast => new WeatherForecast
                {
                    Cloudiness = weatherForecast.Cloudiness,
                    Humidity = weatherForecast.Humidity,
                    Date = DateOnly.FromDateTime(weatherForecast.Date),
                    Lat = lat,
                    Lng = lng,
                    TemperatureMax = weatherForecast.TempMax,
                    TemperatureMin = weatherForecast.TempMin,
                    TemperatureAverage = weatherForecast.TempAverage,
                    WindAngleDegree = weatherForecast.WindDirection,
                    WindGust = weatherForecast.WindGust,
                    WindSpeed = weatherForecast.WindSpeed,
                }).ToList();

                // Fetch existing forecasts for (lat, lng) and date range in a single call
                var dates = newWeatherForecasts.Select(f => f.Date).Distinct().ToList();
                var existingForecasts = await _unitOfWork.WeatherForecasts
                    .GetByCoordinatesAndDatesAsync(lat, lng, dates)
                    .ConfigureAwait(false);

                // lookup for quick matching
                var existingLookup = existingForecasts
                    .ToDictionary(f => f.Date, f => f);

                var toAdd = new List<WeatherForecast>();
                var toUpdate = new List<WeatherForecast>();

                foreach (var forecast in newWeatherForecasts)
                {
                    if (existingLookup.TryGetValue(forecast.Date, out var existing))
                    {
                        // Update existing
                        existing.WindAngleDegree = forecast.WindAngleDegree;
                        existing.Cloudiness = forecast.Cloudiness;
                        existing.TemperatureMin = forecast.TemperatureMin;
                        existing.TemperatureAverage = forecast.TemperatureAverage;
                        existing.TemperatureMax = forecast.TemperatureMax;
                        existing.WindSpeed = forecast.WindSpeed;
                        existing.WindGust = forecast.WindGust;
                        existing.Humidity = forecast.Humidity;

                        toUpdate.Add(existing);
                    }
                    else
                    {
                        toAdd.Add(forecast);
                    }
                }

                // Batch insert/update
                if (toAdd.Any())
                    await _unitOfWork.WeatherForecasts.AddRangeAsync(toAdd).ConfigureAwait(false);

                if (toUpdate.Any())
                    _unitOfWork.WeatherForecasts.UpdateRange(toUpdate);

                await _unitOfWork.CommitAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating weather forecast.");
                throw;
            }
        }
    }
}
