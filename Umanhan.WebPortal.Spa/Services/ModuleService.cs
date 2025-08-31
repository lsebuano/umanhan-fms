using Umanhan.Models.Dtos;
using Umanhan.Models.Entities;
using Umanhan.Models.Models;

namespace Umanhan.WebPortal.Spa.Services
{
    public class ModuleService
    {
        private readonly ApiService _apiService;

        public ModuleService(ApiService apiService)
        {
            _apiService = apiService;
        }

        public Task<ApiResponse<IEnumerable<ModuleDto>>> GetAllModulesAsync()
        {
            try
            {
                return _apiService.GetAsync<IEnumerable<ModuleDto>>("MasterdataAPI", "api/modules");
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Task<ApiResponse<ModuleDto>> GetModuleByIdAsync(Guid id)
        {
            try
            {
                return _apiService.GetAsync<ModuleDto>("MasterdataAPI", $"api/modules/{id}");
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<ModuleDto> CreateModuleAsync(ModuleDto module)
        {
            try
            {
                var response = await _apiService.PostAsync<ModuleDto, ModuleDto>("MasterdataAPI", "api/modules", module).ConfigureAwait(false);
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

        public async Task<ModuleDto> UpdateModuleAsync(ModuleDto module)
        {
            try
            {
                var response = await _apiService.PutAsync<ModuleDto, ModuleDto>("MasterdataAPI", $"api/modules/{module.ModuleId}", module).ConfigureAwait(false);
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

        public async Task<ModuleDto> DeleteModuleAsync(Guid id)
        {
            try
            {
                var response = await _apiService.DeleteAsync<ModuleDto>("MasterdataAPI", $"api/modules/{id}").ConfigureAwait(false);
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
