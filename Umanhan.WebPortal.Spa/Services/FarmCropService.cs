using Umanhan.Dtos;
using Umanhan.Dtos.HelperModels;

namespace Umanhan.WebPortal.Spa.Services
{
    public class FarmCropService
    {
        private readonly ApiService _apiService;

        public FarmCropService(ApiService apiService)
        {
            _apiService = apiService;
        }

        public Task<ApiResponse<IEnumerable<FarmCropDto>>> GetFarmCropsAsync()
        {
            try
            {
                return _apiService.GetAsync<IEnumerable<FarmCropDto>>("OperationsAPI", $"api/farm-crops");
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Task<ApiResponse<FarmCropDto>> GetFarmCropByIdAsync(Guid id)
        {
            try
            {
                return _apiService.GetAsync<FarmCropDto>("OperationsAPI", $"api/farm-crops/{id}");
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Task<ApiResponse<FarmCropDto>> GetFarmCropByCropIdAsync(Guid id)
        {
            try
            {
                return _apiService.GetAsync<FarmCropDto>("OperationsAPI", $"api/farm-crops/crop/{id}");
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<FarmCropDto> CreateFarmCropAsync(FarmCropDto farmCrop)
        {
            try
            {
                var response = await _apiService.PostAsync<FarmCropDto, FarmCropDto>("OperationsAPI", $"api/farm-crops", farmCrop).ConfigureAwait(false);
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

        public async Task<FarmCropDto> UpdateFarmCropAsync(FarmCropDto farmCrop)
        {
            try
            {
                var response = await _apiService.PutAsync<FarmCropDto, FarmCropDto>("OperationsAPI", $"api/farm-crops", farmCrop).ConfigureAwait(false);
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

        public async Task<FarmCropDto> CreateUpdateFarmCropAsync(FarmCropDto farmCrop)
        {
            try
            {
                var response = await _apiService.PostAsync<FarmCropDto, FarmCropDto>("OperationsAPI", $"api/farm-crops/crop", farmCrop).ConfigureAwait(false);
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
