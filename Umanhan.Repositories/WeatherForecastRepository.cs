using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Npgsql;
using NpgsqlTypes;
using System.Text.Json;
using Umanhan.Models;
using Umanhan.Models.Entities;
using Umanhan.Models.LoggerEntities;
using Umanhan.Repositories.Interfaces;
using Task = System.Threading.Tasks.Task;

namespace Umanhan.Repositories
{
    public class WeatherForecastRepository : UmanhanRepository<WeatherForecast>, IWeatherForecastRepository
    {
        public WeatherForecastRepository(UmanhanDbContext context) : base(context)
        {
        }

        public Task AddRangeAsync(IEnumerable<WeatherForecast> weatherForecasts)
        {
            var options = new BulkConfig
            {
                UpdateByProperties = ["Lat", "Lng", "Date"],
                PreserveInsertOrder = true,
                SetOutputIdentity = false,
                CalculateStats = true
            };
            return _context.BulkInsertAsync(weatherForecasts, bulkConfig: options);
        }

        public Task<List<WeatherForecast>> GetByCoordinatesAndDatesAsync(double lat, double lng, List<DateOnly> dates)
        {
            return _dbSet.AsNoTracking()
                         .Where(x => x.Lat == lat && 
                                     x.Lng == lng && 
                                     dates.Contains(x.Date))
                         .ToListAsync();
        }

        public Task<List<WeatherForecast>> GetByCoordinatesAsync(double lat, double lng)
        {
            return _dbSet.AsNoTracking()
                         .Where(x => x.Lat == lat &&
                                     x.Lng == lng)
                         .ToListAsync();
        }

        public Task<List<WeatherForecast>> GetByCoordinatesAsync(double lat, double lng, DateOnly date)
        {
            return _dbSet.AsNoTracking()
                         .Where(x => x.Lat == lat &&
                                     x.Lng == lng &&
                                     x.Date == date)
                         .ToListAsync();
        }

        public Task<List<WeatherForecast>> GetByDateAsync(DateOnly start, DateOnly end)
        {
            return _dbSet.AsNoTracking()
                         .Where(x => x.Date >= start &&
                                      x.Date <= end)
                         .ToListAsync();
        }

        public Task<List<WeatherForecast>> GetByDateAsync(DateOnly date)
        {
            return _dbSet.AsNoTracking()
                         .Where(x => x.Date == date)
                         .ToListAsync();
        }

        public void UpdateRange(IEnumerable<WeatherForecast> weatherForecasts)
        {
            _dbSet.UpdateRange(weatherForecasts);
        }

        public async Task UpsertAsync(IEnumerable<WeatherForecast> weatherForecasts)
        {
            var json = JsonSerializer.Serialize(weatherForecasts,
                new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
                });
            var param = new NpgsqlParameter("p0", NpgsqlDbType.Jsonb)
            {
                Value = json
            };

            await _context.Database.ExecuteSqlRawAsync(
                "SELECT fn_upsert_weather_forecasts(@p0);",
                param)
                .ConfigureAwait(false);
        }
    }
}
