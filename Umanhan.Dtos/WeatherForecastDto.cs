namespace Umanhan.Dtos
{
    public class WeatherForecastDto
    {
        public Guid ForecastId { get; set; }
        public double? Lat { get; set; }
        public double? Lng { get; set; }
        public DateOnly Date { get; set; }
        public float? TemperatureMin { get; set; }
        public float? TemperatureMax { get; set; }
        public float? TemperatureAverage { get; set; }
        public double? Humidity { get; set; }
        public double? WindSpeed { get; set; }
        public double? WindAngleDegree { get; set; }
        public double? WindGust { get; set; }
        public double? Cloudiness { get; set; }
    }
}
