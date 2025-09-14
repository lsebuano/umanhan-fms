using Umanhan.Dtos;
using Umanhan.Dtos.HelperModels;

namespace Umanhan.WebPortal.Spa.Services
{
    public class QueryLogService
    {
        private readonly ApiService _apiService;

        public QueryLogService(ApiService apiService)
        {
            _apiService = apiService;
        }

        public Task<ApiResponse<IEnumerable<QueryLogDto>>> GetQueryLogsAsync(DateOnly date)
        {
            try
            {
                return _apiService.GetAsync<IEnumerable<QueryLogDto>>("LogsAPI", $"api/query-logs/date/{date:yyyy-MM-dd}");
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
