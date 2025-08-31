using Microsoft.AspNetCore.OutputCaching;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using Umanhan.Models.LoggerEntities;
using Umanhan.Models.Models;
using Umanhan.Services;

namespace Umanhan.Weather.Api.Endpoints
{
    public class WeatherEndpoints
    {
        private readonly ILogger<WeatherEndpoints> _logger;
        private readonly WeatherForecastService _weatherForecastService;

        public WeatherEndpoints(ILogger<WeatherEndpoints> logger,
            WeatherForecastService weatherForecastService)
        {
            _logger = logger;
            _weatherForecastService = weatherForecastService;
        }

        public async Task<IResult> GetWeatherDataAsync(HttpContext context, string provider, string apiKey, double lat, double lng, IHttpClientFactory httpClientFactory, IOutputCacheStore cache, CancellationToken cancellationToken)
        {
            if (!AuthorizationHelper.IsAuthorized(context))
                return Results.Forbid();

            string cacheKey = $"weather-{lat}-{lng}";

            // Check cache
            var cachedData = await cache.GetAsync(cacheKey, cancellationToken);
            if (cachedData != null)
            {
                var cachedJson = System.Text.Encoding.UTF8.GetString(cachedData);
                var cachedResponse = JsonSerializer.Deserialize<WeatherData>(cachedJson);
                return Results.Json(cachedResponse);
            }

            WeatherData? response = null;
            try
            {
                string openMeteoUrl = $"https://api.open-meteo.com/v1/forecast?latitude={lat}&longitude={lng}&hourly=temperature_2m,precipitation,wind_speed_10m,soil_moisture_0_10cm";
                string openWeatherUrl = $"https://api.openweathermap.org/data/2.5/weather?lat={lat}&lon={lng}&APPID={apiKey}&units=metric";

                if (provider.Equals("open-meteo", StringComparison.OrdinalIgnoreCase))
                {
                    var client = httpClientFactory.CreateClient("OpenMeteo");
                    var openMeteoResponse = await client.GetFromJsonAsync<OpenMeteoResponse>(openMeteoUrl, cancellationToken).ConfigureAwait(false);

                    if (openMeteoResponse is null)
                    {
                        return Results.BadRequest("Failed to fetch Open-Meteo data.");
                    }

                    response = this.MapOpenMeteoData(openMeteoResponse);
                }
                else if (provider.Equals("openweather", StringComparison.OrdinalIgnoreCase))
                {
                    var client = httpClientFactory.CreateClient("OpenWeather");
                    var openWeatherResponse = await client.GetFromJsonAsync<OpenWeatherResponse>(openWeatherUrl, cancellationToken).ConfigureAwait(false);

                    if (openWeatherResponse is null)
                    {
                        return Results.BadRequest("Failed to fetch OpenWeather data.");
                    }

                    response = this.MapOpenWeatherData(openWeatherResponse);
                }
                else
                {
                    return Results.Problem("Invalid provider. Use 'open-meteo' or 'openweather'.", statusCode: 400);
                }

                if (response == null)
                    return Results.Problem("Failed to fetch weather data.", statusCode: 502);

                // Serialize and store in cache
                var responseJson = JsonSerializer.SerializeToUtf8Bytes(response);
                await cache.SetAsync(cacheKey, responseJson, null, TimeSpan.FromMinutes(10), cancellationToken);

                return Results.Json(response);
            }
            catch (HttpRequestException ex) when (ex.InnerException is System.Net.Sockets.SocketException)
            {
                _logger.LogError(ex, "Network issue while fetching weather data from {Provider} for coordinates ({Lat}, {Lng})", provider, lat, lng);
                return Results.Problem("Network issue: Unable to reach Open-Meteo API.", statusCode: 503);
            }
            catch (TaskCanceledException)
            {
                _logger.LogError("Request timeout while fetching weather data from {Provider} for coordinates ({Lat}, {Lng})", provider, lat, lng);
                return Results.Problem("Request timeout: Open-Meteo API took too long to respond.", statusCode: 504);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while fetching weather data from {Provider} for coordinates ({Lat}, {Lng})", provider, lat, lng);
                return Results.Problem($"Unexpected error: {ex.Message}", statusCode: 500);
            }
        }

        public async Task<IResult> GetWeatherForecastDataAsync(HttpContext context, string apiKey, double lat, double lng, IHttpClientFactory httpClientFactory, IOutputCacheStore cache, CancellationToken cancellationToken)
        {
            if (!AuthorizationHelper.IsAuthorized(context))
                return Results.Forbid();

            if (lat == 0 && lng == 0)
                return Results.BadRequest();

            string cacheKey = $"weather-forecast-{lat}-{lng}";

            // Check cache
            var cachedData = await cache.GetAsync(cacheKey, cancellationToken);
            if (cachedData != null)
            {
                var cachedJson = System.Text.Encoding.UTF8.GetString(cachedData);
                var cachedResponse = JsonSerializer.Deserialize<List<ForecastDailyWeather>>(cachedJson);
                return Results.Json(cachedResponse);
            }

            List<ForecastDailyWeather> response = null;
            try
            {
                string providerUrl = Environment.GetEnvironmentVariable("WEATHER_PROVIDER_URL");
                string openWeatherUrl = $"{providerUrl}?lat={lat}&lon={lng}&APPID={apiKey}&units=metric";

                var client = httpClientFactory.CreateClient("OpenWeather");
                var openWeatherForecastResponse = await client.GetFromJsonAsync<WeatherForecastData>(openWeatherUrl, cancellationToken).ConfigureAwait(false);

                if (openWeatherForecastResponse is null)
                {
                    return Results.BadRequest("Failed to fetch OpenWeather data.");
                }

                response = openWeatherForecastResponse?.List
                    .GroupBy(x => DateTimeOffset.FromUnixTimeSeconds(x.Dt).Date.ToManilaTime())
                    .Select(g => new ForecastDailyWeather
                    {
                        Date = g.Key,
                        City = openWeatherForecastResponse.City?.Name,
                        Country = openWeatherForecastResponse.City?.Country,
                        Sunrise = DateTimeOffset.FromUnixTimeSeconds(openWeatherForecastResponse.City?.Sunrise ?? 0).DateTime.ToManilaTime().ToString("HH:mm"),
                        Sunset = DateTimeOffset.FromUnixTimeSeconds(openWeatherForecastResponse.City?.Sunset ?? 0).DateTime.ToManilaTime().ToString("HH:mm"),
                        Population = openWeatherForecastResponse.City?.Population ?? 0,
                        TempMin = g.Min(x => x.Main.TempMin),       // minimum temperature in degC if metric
                        TempMax = g.Max(x => x.Main.TempMax),       // maximum temperature in degC if metric
                        TempAverage = g.Average(x => x.Main.Temp),  // average temperature per day
                        Humidity = g.Average(x => x.Main.Humidity), // average humidity per day
                        Weather = g.First().Weather.First().Description,
                        Icon = g.First().Weather.First().Icon,
                        WindSpeed = g.Average(x => x.Wind.Speed),   // average wind speed per day
                        WindDirection = g.First().Wind.Deg,         // first wind direction
                        WindCardinalDirection = GetWindCardinalDirection(g.First().Wind.Deg),
                        WindGust = g.Average(x => x.Wind.Gust ?? 0),// average wind gusts for the day
                        Cloudiness = g.Average(x => x.Clouds.All),
                        RainLast1H = g.Average(x => x.Rain.RainLast1H),
                        RainLast3H = g.Average(x => x.Rain.RainLast3H)
                    })
                    .ToList() ?? [];

                if (response == null)
                    return Results.Problem("Failed to fetch weather data.", statusCode: 502);

                // Serialize and store in cache
                var responseJson = JsonSerializer.SerializeToUtf8Bytes(response);
                await cache.SetAsync(cacheKey, responseJson, null, TimeSpan.FromMinutes(10), cancellationToken);

                //_ = _weatherForecastService.CreateWeatherForecastsAsync(response, lat, lng);
                //_ = _weatherForecastService.CreateUpdateWeatherForecastsAsync(response, lat, lng);
                await _weatherForecastService.UpsertWeatherForecastsAsync(response, lat, lng).ConfigureAwait(false);

                return Results.Json(response);
            }
            catch (HttpRequestException ex) when (ex.InnerException is System.Net.Sockets.SocketException)
            {
                _logger.LogError(ex, "Network issue while fetching weather forecast data for coordinates ({Lat}, {Lng})", lat, lng);
                return Results.Problem("Network issue: Unable to reach Open-Meteo API.", statusCode: 503);
            }
            catch (TaskCanceledException)
            {
                _logger.LogError("Request timeout while fetching weather forecast data for coordinates ({Lat}, {Lng})", lat, lng);
                return Results.Problem("Request timeout: Open-Meteo API took too long to respond.", statusCode: 504);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while fetching weather forecast data for coordinates ({Lat}, {Lng})", lat, lng);
                return Results.Problem($"Unexpected error: {ex.Message}", statusCode: 500);
            }
        }

        private WeatherData MapOpenMeteoData(OpenMeteoResponse openMeteoResponse)
        {
            return new WeatherData
            {
                Coord = new Coordinates
                {
                    Lat = openMeteoResponse.Latitude,
                    Lon = openMeteoResponse.Longitude
                },
                Weather = new List<WeatherCondition>
                        {
                            new WeatherCondition
                            {
                                Main = openMeteoResponse.CurrentWeather.IsDay == 1 ? "Daytime" : "Nighttime",
                                Description = "Weather condition based on temperature and wind speed",
                                Icon = GetWeatherIcon(openMeteoResponse.CurrentWeather.Temperature, openMeteoResponse.CurrentWeather.WindSpeed)
                            }
                        },
                Main = new MainWeatherInfo
                {
                    Temperature = openMeteoResponse.CurrentWeather.Temperature,
                    FeelsLike = openMeteoResponse.CurrentWeather.Temperature, // Open-Meteo does not provide 'feels_like' directly
                    Pressure = openMeteoResponse.CurrentWeather.PressureMsl,
                    Humidity = openMeteoResponse.CurrentWeather.RelativeHumidity,
                    DewPoint = CalculateDewPoint(openMeteoResponse.CurrentWeather.Temperature, openMeteoResponse.CurrentWeather.RelativeHumidity)
                },
                Wind = new WindInfo
                {
                    Speed = openMeteoResponse.CurrentWeather.WindSpeed,
                    Deg = openMeteoResponse.CurrentWeather.WindDirection
                },
                Clouds = new CloudInfo { Cloudiness = openMeteoResponse.CurrentWeather.CloudCover },
                Rain = new RainInfo
                {
                    RainLastHour = openMeteoResponse.CurrentWeather.Precipitation,
                },
                Sun = new SunInfo
                {
                    Sunrise = openMeteoResponse.DailyWeather.Sunrise.FirstOrDefault(),
                    Sunset = openMeteoResponse.DailyWeather.Sunset.FirstOrDefault()
                },
                Timezone = openMeteoResponse.Timezone,
                Name = "Open-Meteo Location" // Open-Meteo does not provide location names
            };
        }

        private WeatherData MapOpenWeatherData(OpenWeatherResponse openWeatherResponse)
        {
            return new WeatherData
            {
                Coord = new Coordinates
                {
                    Lat = openWeatherResponse.Coord.Lat,
                    Lon = openWeatherResponse.Coord.Lon
                },
                Weather = openWeatherResponse.Weather.Select(w => new WeatherCondition
                {
                    Id = w.Id,
                    Main = w.Main,
                    Description = w.Description,
                    Icon = w.Icon
                }).ToList(),
                Main = new MainWeatherInfo
                {
                    Temperature = openWeatherResponse.Main.Temp,
                    FeelsLike = openWeatherResponse.Main.FeelsLike,
                    TempMin = openWeatherResponse.Main.TempMin,
                    TempMax = openWeatherResponse.Main.TempMax,
                    Pressure = openWeatherResponse.Main.Pressure,
                    Humidity = openWeatherResponse.Main.Humidity,
                    DewPoint = CalculateDewPoint(openWeatherResponse.Main.Temp, openWeatherResponse.Main.Humidity)
                },
                Wind = new WindInfo
                {
                    Speed = openWeatherResponse.Wind.Speed,
                    Deg = openWeatherResponse.Wind.Deg,
                    Gust = openWeatherResponse.Wind.Gust ?? 0
                },
                Clouds = new CloudInfo { Cloudiness = openWeatherResponse.Clouds.All },
                Rain = new RainInfo
                {
                    RainLastHour = openWeatherResponse.Rain?.RainLastHour,
                    RainLast3Hours = openWeatherResponse.Rain?.RainLast3Hours
                },
                Sun = new SunInfo
                {
                    Sunrise = openWeatherResponse.Sys.Sunrise,
                    Sunset = openWeatherResponse.Sys.Sunset
                },
                Timezone = openWeatherResponse.Timezone.ToString(),
                Name = openWeatherResponse.Name
            };
        }

        private static double CalculateDewPoint(double temperature, int humidity)
        {
            double a = 17.27;
            double b = 237.7;
            double alpha = ((a * temperature) / (b + temperature)) + Math.Log(humidity / 100.0);
            return (b * alpha) / (a - alpha);
        }

        private static string GetWeatherIcon(double temperature, double windSpeed)
        {
            if (temperature > 30)
                return "hot.png"; // Custom icon for hot weather
            else if (temperature < 10)
                return "cold.png"; // Custom icon for cold weather
            else if (windSpeed > 15)
                return "windy.png"; // Custom icon for windy weather
            else
                return "default.png"; // Default icon
        }

        private static string GetWindCardinalDirection(double degrees)
        {
            string[] directions = {
                "N", "NNE", "NE", "ENE", "E", "ESE", "SE", "SSE",
                "S", "SSW", "SW", "WSW", "W", "WNW", "NW", "NNW", "N"
            };

            int index = (int)Math.Round(degrees / 22.5) % 16;
            return directions[index];
        }
    }
}
