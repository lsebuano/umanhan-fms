using Umanhan.Models.Dtos;
using Umanhan.Models.Entities;
using Umanhan.Models.Models;

namespace Umanhan.WebPortal.Spa.Services
{
    public class ChangeLogService
    {
        private readonly ApiService _apiService;

        public ChangeLogService(ApiService apiService)
        {
            _apiService = apiService;
        }

        public Task<ApiResponse<IEnumerable<ChangeLogDto>>> GetChangeLogsAsync(DateOnly date)
        {
            try
            {
                return _apiService.GetAsync<IEnumerable<ChangeLogDto>>("LogsAPI", $"api/change-logs/date/{date:yyyy-MM-dd}");
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Task<ApiResponse<ChangeLogDto>> GetChangeLogByIdAsync(Guid id)
        {
            try
            {
                return _apiService.GetAsync<ChangeLogDto>("LogsAPI", $"api/change-logs/{id}");
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
