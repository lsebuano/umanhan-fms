using Umanhan.Dtos;
using Umanhan.Dtos.HelperModels;

namespace Umanhan.WebPortal.Spa.Services
{
    public class LoggerService
    {
        private readonly ApiService _apiService;

        public LoggerService(ApiService apiService)
        {
            _apiService = apiService;
        }

        public Task<ApiResponse<IEnumerable<LogDto>>> GetLogsAsync(Guid farmId, DateTime date)
        {
            try
            {
                return _apiService.GetAsync<IEnumerable<LogDto>>("LoggerAPI", $"api/logs/{farmId}/{date:O}");
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Task<ApiResponse<LogDto>> GetLogByIdAsync(Guid id)
        {
            try
            {
                return _apiService.GetAsync<LogDto>("LoggerAPI", $"api/logs/{id}");
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<LogDto> CreateLogAsync(LogDto log)
        {
            try
            {
                var response = await _apiService.PostAsync<LogDto, LogDto>("LoggerAPI", "api/logs", log).ConfigureAwait(false);
                if (!response.IsSuccess)
                {
                    if (response.Errors != null && response.Errors.Any())
                    {
                        var errors = new Dictionary<string, List<string>>();
                        foreach (var error in response.Errors)
                        {
                            errors.Add(error.Key, error.Value);
                        }
                        response.ErrorMessage = string.Join(", ", errors.SelectMany(x => x.Value));
                    }
                }
                return response.Data;
            }
            catch (Exception ex)
            {

            }
            return null;
        }
    }
}
