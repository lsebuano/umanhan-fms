using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Umanhan.Models.Models
{
    public class WeatherForecastData
    {
        public List<ForecastItem> List { get; set; } = new();
        public City City { get; set; }
    }

    public class City
    {
        public string Name { get; set; }
        public string Country { get; set; }
        public int Population { get; set; }
        public long Sunrise { get; set; }
        public long Sunset { get; set; }
    }

    public class ForecastRain
    {
        [JsonPropertyName("1h")]
        public double RainLast1H { get; set; }

        [JsonPropertyName("3h")]
        public double RainLast3H { get; set; }
    }

    public class ForecastClouds
    {
        public double All { get; set; }
    }

    public class ForecastItem
    {
        public long Dt { get; set; }
        public MainWeather Main { get; set; } = new();
        public ForecastWind Wind { get; set; } = new();
        public ForecastRain Rain { get; set; } = new();
        public ForecastClouds Clouds { get; set; } = new();
        public List<ForecastWeatherCondition> Weather { get; set; } = new();
    }

    public class MainWeather
    {
        public float Temp { get; set; }
        public float TempMin { get; set; }
        public float TempMax { get; set; }
        public float Humidity { get; set; }
    }

    public class ForecastWind
    {
        public double Speed { get; set; }
        public double Deg { get; set; }
        public double? Gust { get; set; }
    }

    public class ForecastWeatherCondition
    {
        public string Description { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
    }

    public class ForecastDailyWeather
    {
        public string City { get; set; }
        public string Country { get; set; }
        public string Sunrise { get; set; }
        public string Sunset { get; set; }
        public int Population { get; set; }
        public DateTime Date { get; set; }
        public float TempMin { get; set; }
        public float TempMax { get; set; }
        public float TempAverage { get; set; }
        public string Weather { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
        public double WindSpeed { get; set; }   // Wind speed in m/s
        public double WindDirection { get; set; } // Wind direction in degrees
        public string WindCardinalDirection { get; set; }
        public double WindGust { get; set; }    // in m/s
        public double Humidity { get; set; }    // percentage
        public double Cloudiness { get; set; }      // cloudiness, percentage
        public double RainLast1H { get; set; }  // rain volume, in mm
        public double RainLast3H { get; set; }  // rain volume, in mm
    }
}
