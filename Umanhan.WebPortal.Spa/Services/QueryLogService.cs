using Umanhan.Dtos;
using Umanhan.Dtos.HelperModels;
using Umanhan.Shared.Model;

namespace Umanhan.WebPortal.Spa.Services
{
    public class QueryLogService
    {
        private readonly ApiService _apiService;

        public QueryLogService(ApiService apiService)
        {
            _apiService = apiService;
        }

        public Task<ApiResponse<PagedResult<QueryLogDto>>> GetQueryLogsAsync(DateOnly date, int pageNumber, int pageSize)
        {
            try
            {
                return _apiService.GetAsync<PagedResult<QueryLogDto>>("LogsAPI", $"api/query-logs/date/{date:yyyy-MM-dd}/{pageNumber}/{pageSize}");
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Task<ApiResponse<QueryLogDto>> GetQueryLogByIdAsync(Guid id)
        {
            try
            {
                return _apiService.GetAsync<QueryLogDto>("LogsAPI", $"api/query-logs/{id}");
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
