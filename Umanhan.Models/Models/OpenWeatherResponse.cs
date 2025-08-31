using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Umanhan.Models.Models
{
    public class OpenWeatherResponse
    {
        public Coord Coord { get; set; }
        public List<Weather> Weather { get; set; }
        public Main Main { get; set; }
        public Wind Wind { get; set; }
        public Clouds Clouds { get; set; }
        public Rain? Rain { get; set; }
        public Sys Sys { get; set; }
        public int Timezone { get; set; }
        public string Name { get; set; }
    }

    public class Coord
    {
        public double Lat { get; set; }
        public double Lon { get; set; }
    }
    public class Weather
    {
        public int Id { get; set; }
        public string Main { get; set; }
        public string Description { get; set; }
        public string Icon { get; set; }
    }
    public class Main
    {
        public double Temp { get; set; }
        public double FeelsLike { get; set; }
        public double TempMin { get; set; }
        public double TempMax { get; set; }
        public int Pressure { get; set; }
        public int Humidity { get; set; }
    }
    public class Wind
    {
        public double Speed { get; set; }
        public int Deg { get; set; }
        public double? Gust { get; set; }
    }
    public class Clouds
    {
        public int All { get; set; }
    }
    public class Rain
    {
        public double? RainLastHour { get; set; }
        public double? RainLast3Hours { get; set; }
    }
    public class Sys
    {
        public long Sunrise { get; set; }
        public long Sunset { get; set; }
    }
}
