using Umanhan.Models.Dtos;
using Umanhan.Models.Entities;
using Umanhan.Models.Models;

namespace Umanhan.WebPortal.Spa.Services
{
    public class CropService
    {
        private readonly ApiService _apiService;

        public CropService(ApiService apiService)
        {
            _apiService = apiService;
        }

        public Task<ApiResponse<IEnumerable<CropDto>>> GetAllCropsAsync()
        {
            try
            {
                return _apiService.GetAsync<IEnumerable<CropDto>>("MasterdataAPI", "api/crops");
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Task<ApiResponse<CropDto>> GetCropByIdAsync(Guid id)
        {
            try
            {
                return _apiService.GetAsync<CropDto>("MasterdataAPI", $"api/crops/{id}");
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<CropDto> CreateCropAsync(CropDto crop)
        {
            try
            {
                var response = await _apiService.PostAsync<CropDto, CropDto>("MasterdataAPI", "api/crops", crop).ConfigureAwait(false);
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

        public async Task<CropDto> UpdateCropAsync(CropDto crop)
        {
            try
            {
                var response = await _apiService.PutAsync<CropDto, CropDto>("MasterdataAPI", $"api/crops/{crop.CropId}", crop).ConfigureAwait(false);
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

        public async Task<CropDto> DeleteCropAsync(Guid id)
        {
            try
            {
                var response = await _apiService.DeleteAsync<CropDto>("MasterdataAPI", $"api/crops/{id}").ConfigureAwait(false);
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
