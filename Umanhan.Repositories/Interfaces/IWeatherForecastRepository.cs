using Umanhan.Models.Entities;
using Task = System.Threading.Tasks.Task;

namespace Umanhan.Repositories.Interfaces
{
    public interface IWeatherForecastRepository : IRepository<WeatherForecast>
    {
        // add new methods specific to this repository
        Task<List<WeatherForecast>> GetByDateAsync(DateOnly date);
        Task<List<WeatherForecast>> GetByDateAsync(DateOnly start, DateOnly end);
        Task<List<WeatherForecast>> GetByCoordinatesAsync(double lat, double lng);
        Task<List<WeatherForecast>> GetByCoordinatesAsync(double lat, double lng, DateOnly date);
        Task<List<WeatherForecast>> GetByCoordinatesAndDatesAsync(double lat, double lng, List<DateOnly> dates);
        Task AddRangeAsync(IEnumerable<WeatherForecast> weatherForecasts);
        void UpdateRange(IEnumerable<WeatherForecast> weatherForecasts);
        Task UpsertAsync(IEnumerable<WeatherForecast> weatherForecasts);
    }
}
