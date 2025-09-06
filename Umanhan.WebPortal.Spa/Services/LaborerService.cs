using Umanhan.Dtos;
using Umanhan.Dtos.HelperModels;

namespace Umanhan.WebPortal.Spa.Services
{
    public class LaborerService
    {
        private readonly ApiService _apiService;

        public LaborerService(ApiService apiService)
        {
            _apiService = apiService;
        }

        public Task<ApiResponse<IEnumerable<LaborerDto>>> GetAllLaborersAsync()
        {
            try
            {
                return _apiService.GetAsync<IEnumerable<LaborerDto>>("MasterdataAPI", "api/laborers");
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Task<ApiResponse<LaborerDto>> GetLaborerByIdAsync(Guid id)
        {
            try
            {
                return _apiService.GetAsync<LaborerDto>("MasterdataAPI", $"api/laborers/{id}");
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<LaborerDto> CreateLaborerAsync(LaborerDto laborer)
        {
            try
            {
                var response = await _apiService.PostAsync<LaborerDto, LaborerDto>("MasterdataAPI", "api/laborers", laborer).ConfigureAwait(false);
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

        public async Task<LaborerDto> UpdateLaborerAsync(LaborerDto laborer)
        {
            try
            {
                var response = await _apiService.PutAsync<LaborerDto, LaborerDto>("MasterdataAPI", $"api/laborers/{laborer.LaborerId}", laborer).ConfigureAwait(false);
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

        public async Task<LaborerDto> DeleteLaborerAsync(Guid id)
        {
            try
            {
                var response = await _apiService.DeleteAsync<LaborerDto>("MasterdataAPI", $"api/laborers/{id}").ConfigureAwait(false);
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
