using Umanhan.Dtos;
using Umanhan.Dtos.HelperModels;

namespace Umanhan.WebPortal.Spa.Services
{
    public class InventoryService
    {
        private readonly ApiService _apiService;

        public InventoryService(ApiService apiService)
        {
            _apiService = apiService;
        }

        public Task<ApiResponse<IEnumerable<InventoryDto>>> GetAllInventoriesAsync()
        {
            try
            {
                return _apiService.GetAsync<IEnumerable<InventoryDto>>("MasterdataAPI", "api/inventories");
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Task<ApiResponse<InventoryDto>> GetInventoryByIdAsync(Guid id)
        {
            try
            {
                return _apiService.GetAsync<InventoryDto>("MasterdataAPI", $"api/inventories/{id}");
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<InventoryDto> CreateInventoryAsync(InventoryDto inventory)
        {
            try
            {
                var response = await _apiService.PostAsync<InventoryDto, InventoryDto>("MasterdataAPI", "api/inventories", inventory).ConfigureAwait(false);
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

        public async Task<InventoryDto> UpdateInventoryAsync(InventoryDto inventory)
        {
            try
            {
                var response = await _apiService.PutAsync<InventoryDto, InventoryDto>("MasterdataAPI", $"api/inventories/{inventory.InventoryId}", inventory).ConfigureAwait(false);
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

        public async Task<InventoryDto> DeleteInventoryAsync(Guid id)
        {
            try
            {
                var response = await _apiService.DeleteAsync<InventoryDto>("MasterdataAPI", $"api/inventories/{id}").ConfigureAwait(false);
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
