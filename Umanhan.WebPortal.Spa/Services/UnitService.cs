using Umanhan.Dtos;
using Umanhan.Dtos.HelperModels;

namespace Umanhan.WebPortal.Spa.Services
{
    public class UnitService
    {
        private readonly ApiService _apiService;

        public UnitService(ApiService apiService)
        {
            _apiService = apiService;
        }

        public Task<ApiResponse<IEnumerable<UnitDto>>> GetAllUnitsAsync()
        {
            try
            {
                return _apiService.GetAsync<IEnumerable<UnitDto>>("MasterdataAPI", "api/units");
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Task<ApiResponse<UnitDto>> GetUnitByIdAsync(Guid id)
        {
            try
            {
                return _apiService.GetAsync<UnitDto>("MasterdataAPI", $"api/units/{id}");
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<UnitDto> CreateUnitAsync(UnitDto unit)
        {
            try
            {
                var response = await _apiService.PostAsync<UnitDto, UnitDto>("MasterdataAPI", "api/units", unit).ConfigureAwait(false);
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

        public async Task<UnitDto> UpdateUnitAsync(UnitDto unit)
        {
            try
            {
                var response = await _apiService.PutAsync<UnitDto, UnitDto>("MasterdataAPI", $"api/units/{unit.UnitId}", unit).ConfigureAwait(false);
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

        public async Task<UnitDto> DeleteUnitAsync(Guid id)
        {
            try
            {
                var response = await _apiService.DeleteAsync<UnitDto>("MasterdataAPI", $"api/units/{id}").ConfigureAwait(false);
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
