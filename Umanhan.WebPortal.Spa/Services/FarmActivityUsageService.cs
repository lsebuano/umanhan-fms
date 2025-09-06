using Umanhan.Dtos;
using Umanhan.Dtos.HelperModels;

namespace Umanhan.WebPortal.Spa.Services
{
    public class FarmActivityUsageService
    {
        private readonly ApiService _apiService;

        public FarmActivityUsageService(ApiService apiService)
        {
            _apiService = apiService;
        }

        public Task<ApiResponse<IEnumerable<FarmActivityUsageDto>>> GetFarmActivityUsagesAsync(Guid farmId)
        {
            try
            {
                return _apiService.GetAsync<IEnumerable<FarmActivityUsageDto>>("OperationsAPI", $"api/farm-usages/farm-id/{farmId}");
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Task<ApiResponse<IEnumerable<FarmActivityUsageDto>>> GetFarmActivityUsageByItemIdAsync(Guid farmId, Guid itemId)
        {
            try
            {
                return _apiService.GetAsync<IEnumerable<FarmActivityUsageDto>>("OperationsAPI", $"api/farm-usages/farm-id/{farmId}/{itemId}");
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Task<ApiResponse<FarmActivityUsageDto>> GetFarmActivityUsageByIdAsync(Guid id)
        {
            try
            {
                return _apiService.GetAsync<FarmActivityUsageDto>("OperationsAPI", $"api/farm-usages/{id}");
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Task<ApiResponse<IEnumerable<FarmActivityUsageDto>>> GetFarmActivityUsageByActivityAsync(Guid activityId)
        {
            try
            {
                return _apiService.GetAsync<IEnumerable<FarmActivityUsageDto>>("OperationsAPI", $"api/farm-usages/activity/{activityId}");
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<FarmActivityUsageDto> CreateFarmActivityUsageAsync(FarmActivityUsageDto farmUsage)
        {
            try
            {
                var response = await _apiService.PostAsync<FarmActivityUsageDto, FarmActivityUsageDto>("OperationsAPI", $"api/farm-usages", farmUsage).ConfigureAwait(false);
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

        public async Task<FarmActivityUsageDto> UpdateFarmActivityUsageAsync(FarmActivityUsageDto farmUsage)
        {
            try
            {
                var response = await _apiService.PutAsync<FarmActivityUsageDto, FarmActivityUsageDto>("OperationsAPI", $"api/farm-usages/{farmUsage.UsageId}", farmUsage).ConfigureAwait(false);
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

        public async Task<FarmActivityUsageDto> DeleteFarmActivityUsageAsync(Guid id)
        {
            try
            {
                var response = await _apiService.DeleteAsync<FarmActivityUsageDto>("OperationsAPI", $"api/farm-usages/{id}").ConfigureAwait(false);
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
