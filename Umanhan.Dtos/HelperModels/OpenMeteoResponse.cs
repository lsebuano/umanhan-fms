namespace Umanhan.Dtos.HelperModels
{
    public class OpenMeteoResponse
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Timezone { get; set; }
        public CurrentWeather CurrentWeather { get; set; }
        public DailyWeather DailyWeather { get; set; }
    }

    public class CurrentWeather
    {
        public double Temperature { get; set; }
        public double WindSpeed { get; set; }
        public int WindDirection { get; set; }
        public double Precipitation { get; set; }
        public int CloudCover { get; set; }
        public int RelativeHumidity { get; set; }
        public double PressureMsl { get; set; }
        public int IsDay { get; set; }
    }

    public class DailyWeather
    {
        public List<long> Sunrise { get; set; }
        public List<long> Sunset { get; set; }
    }
}
