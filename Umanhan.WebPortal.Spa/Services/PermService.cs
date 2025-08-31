using Umanhan.Models.Dtos;
using Umanhan.Models.Entities;
using Umanhan.Models.Models;

namespace Umanhan.WebPortal.Spa.Services
{
    public class PermService
    {
        private readonly ApiService _apiService;

        public PermService(ApiService apiService)
        {
            _apiService = apiService;
        }

        public Task<ApiResponse<IEnumerable<PermissionDto>>> GetAllPermissionsAsync()
        {
            try
            {
                return _apiService.GetAsync<IEnumerable<PermissionDto>>("MasterdataAPI", "api/permissions");
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Task<ApiResponse<PermissionDto>> GetPermissionByIdAsync(Guid id)
        {
            try
            {
                return _apiService.GetAsync<PermissionDto>("MasterdataAPI", $"api/permissions/{id}");
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<PermissionDto> CreatePermissionAsync(PermissionDto permission)
        {
            try
            {
                var response = await _apiService.PostAsync<PermissionDto, PermissionDto>("MasterdataAPI", "api/permissions", permission).ConfigureAwait(false);
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

        public async Task<PermissionDto> UpdatePermissionAsync(PermissionDto permission)
        {
            try
            {
                var response = await _apiService.PutAsync<PermissionDto, PermissionDto>("MasterdataAPI", $"api/permissions/{permission.PermissionId}", permission).ConfigureAwait(false);
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

        public async Task<PermissionDto> DeletePermissionAsync(Guid id)
        {
            try
            {
                var response = await _apiService.DeleteAsync<PermissionDto>("MasterdataAPI", $"api/permissions/{id}").ConfigureAwait(false);
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
