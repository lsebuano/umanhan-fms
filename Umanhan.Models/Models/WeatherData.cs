using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Umanhan.Models.Models
{
    public class WeatherData
    {
        public Coordinates Coord { get; set; }
        public List<WeatherCondition> Weather { get; set; }
        public MainWeatherInfo Main { get; set; }
        public WindInfo Wind { get; set; }
        public CloudInfo Clouds { get; set; }
        public RainInfo Rain { get; set; }
        public SunInfo Sun { get; set; }
        public SoilInfo Soil { get; set; }
        public AgriculturalInfo Agriculture { get; set; }
        public string Timezone { get; set; }
        public string Name { get; set; }
    }

    public class Coordinates
    {
        public double Lat { get; set; }
        public double Lon { get; set; }
    }

    public class WeatherCondition
    {
        public int Id { get; set; }
        public string Main { get; set; }
        public string Description { get; set; }
        public string Icon { get; set; }
    }

    public class MainWeatherInfo
    {
        public double Temperature { get; set; }
        public double FeelsLike { get; set; }
        public double TempMin { get; set; }
        public double TempMax { get; set; }
        public double Pressure { get; set; }
        public int Humidity { get; set; }
        public double DewPoint { get; set; }
    }

    public class WindInfo
    {
        public double Speed { get; set; }
        public int Deg { get; set; }
        public double Gust { get; set; }
    }

    public class CloudInfo
    {
        public int Cloudiness { get; set; }
    }

    public class RainInfo
    {
        public double? RainLastHour { get; set; }
        public double? RainLast3Hours { get; set; }
    }

    public class SunInfo
    {
        public long Sunrise { get; set; }
        public long Sunset { get; set; }
        public double UvIndex { get; set; }
        public double SolarRadiation { get; set; }
    }

    public class SoilInfo
    {
        public double SoilTemperature { get; set; }
        public double SoilMoisture { get; set; }
    }

    public class AgriculturalInfo
    {
        public double FrostProbability { get; set; }
        public double DroughtIndex { get; set; }
        public double Evapotranspiration { get; set; }
        public double GrowingDegreeDays { get; set; }
    }
}
