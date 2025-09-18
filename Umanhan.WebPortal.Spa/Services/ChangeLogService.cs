using Umanhan.Dtos;
using Umanhan.Dtos.HelperModels;
using Umanhan.Shared.Model;

namespace Umanhan.WebPortal.Spa.Services
{
    public class ChangeLogService
    {
        private readonly ApiService _apiService;

        public ChangeLogService(ApiService apiService)
        {
            _apiService = apiService;
        }

        public Task<ApiResponse<PagedResult<ChangeLogDto>>> GetChangeLogsAsync(DateOnly date, int pageNumber, int pageSize)
        {
            try
            {
                return _apiService.GetAsync<PagedResult<ChangeLogDto>>("LogsAPI", $"api/change-logs/date/{date:yyyy-MM-dd}/{pageNumber}/{pageSize}");
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
