using Umanhan.Models.Dtos;
using Umanhan.Models.Models;

namespace Umanhan.WebPortal.Spa.Services
{
    public class FarmActivityLaborerService
    {
        private readonly ApiService _apiService;

        public FarmActivityLaborerService(ApiService apiService)
        {
            _apiService = apiService;
        }

        public Task<ApiResponse<IEnumerable<FarmActivityLaborerDto>>> GetFarmActivityLaborersAsync(Guid farmId)
        {
            try
            {
                return _apiService.GetAsync<IEnumerable<FarmActivityLaborerDto>>("OperationsAPI", $"api/farm-labors/farm-id/{farmId}");
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Task<ApiResponse<FarmActivityLaborerDto>> GetFarmActivityLaborerByIdAsync(Guid id)
        {
            try
            {
                return _apiService.GetAsync<FarmActivityLaborerDto>("OperationsAPI", $"api/farm-labors/{id}");
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Task<ApiResponse<IEnumerable<FarmActivityLaborerDto>>> GetFarmActivityLaborerByActivityAsync(Guid activityId)
        {
            try
            {
                return _apiService.GetAsync<IEnumerable<FarmActivityLaborerDto>>("OperationsAPI", $"api/farm-labors/activity/{activityId}");
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<FarmActivityLaborerDto> CreateFarmActivityLaborAsync(FarmActivityLaborerDto farmLabor)
        {
            try
            {
                var response = await _apiService.PostAsync<FarmActivityLaborerDto, FarmActivityLaborerDto>("OperationsAPI", $"api/farm-labors", farmLabor).ConfigureAwait(false);
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

        public async Task<FarmActivityLaborerDto> UpdateFarmActivityLaborAsync(FarmActivityLaborerDto farmLabor)
        {
            try
            {
                var response = await _apiService.PutAsync<FarmActivityLaborerDto, FarmActivityLaborerDto>("OperationsAPI", $"api/farm-labors/{farmLabor.LaborActivityId}", farmLabor).ConfigureAwait(false);
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

        public async Task<FarmActivityLaborerDto> DeleteFarmActivityLaborAsync(Guid id)
        {
            try
            {
                var response = await _apiService.DeleteAsync<FarmActivityLaborerDto>("OperationsAPI", $"api/farm-labors/{id}").ConfigureAwait(false);
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
