using Umanhan.Models.Dtos;
using Umanhan.Models.Models;

namespace Umanhan.WebPortal.Spa.Services
{
    public class SoilTypeService
    {
        private readonly ApiService _apiService;

        public SoilTypeService(ApiService apiService)
        {
            _apiService = apiService;
        }

        public Task<ApiResponse<IEnumerable<SoilTypeDto>>> GetAllSoilTypesAsync()
        {
            try
            {
                return _apiService.GetAsync<IEnumerable<SoilTypeDto>>("MasterdataAPI", "api/soil-types");
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Task<ApiResponse<SoilTypeDto>> GetSoilTypeByIdAsync(Guid id)
        {
            try
            {
                return _apiService.GetAsync<SoilTypeDto>("MasterdataAPI", $"api/soil-types/{id}");
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<SoilTypeDto> CreateSoilTypeAsync(SoilTypeDto soilType)
        {
            try
            {
                var response = await _apiService.PostAsync<SoilTypeDto, SoilTypeDto>("MasterdataAPI", "api/soil-types", soilType).ConfigureAwait(false);
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

        public async Task<SoilTypeDto> UpdateSoilTypeAsync(SoilTypeDto soilType)
        {
            try
            {
                var response = await _apiService.PutAsync<SoilTypeDto, SoilTypeDto>("MasterdataAPI", $"api/soil-types/{soilType.SoilId}", soilType).ConfigureAwait(false);
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

        public async Task<SoilTypeDto> DeleteSoilTypeAsync(Guid id)
        {
            try
            {
                var response = await _apiService.DeleteAsync<SoilTypeDto>("MasterdataAPI", $"api/soil-types/{id}").ConfigureAwait(false);
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
