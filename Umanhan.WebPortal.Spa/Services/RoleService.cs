using Umanhan.Models.Dtos;
using Umanhan.Models.Entities;
using Umanhan.Models.Models;

namespace Umanhan.WebPortal.Spa.Services
{
    public class RoleService
    {
        private readonly ApiService _apiService;

        public RoleService(ApiService apiService)
        {
            _apiService = apiService;
        }

        public Task<ApiResponse<IEnumerable<RoleDto>>> GetAllRolesAsync(bool activeOnly = true)
        {
            try
            {
                if (activeOnly)
                    return _apiService.GetAsync<IEnumerable<RoleDto>>("UsersAPI", "api/user-mgr/roles/active");
                else
                    return _apiService.GetAsync<IEnumerable<RoleDto>>("UsersAPI", "api/user-mgr/roles");
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Task<ApiResponse<IEnumerable<RoleDto>>> GetRolesExceptAsync()
        {
            try
            {
                    return _apiService.GetAsync<IEnumerable<RoleDto>>("UsersAPI", "api/user-mgr/roles/except");
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Task<ApiResponse<IEnumerable<RoleDto>>> GetAllCognitoGroupsAsync()
        {
            try
            {
                return _apiService.GetAsync<IEnumerable<RoleDto>>("UsersAPI", "api/user-mgr/roles/cognito");
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Task<ApiResponse<RoleDto>> GetRoleByIdAsync(Guid id)
        {
            try
            {
                return _apiService.GetAsync<RoleDto>("UsersAPI", $"api/user-mgr/roles/{id}");
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<RoleDto> CreateRoleAsync(RoleDto role)
        {
            try
            {
                var response = await _apiService.PostAsync<RoleDto, RoleDto>("UsersAPI", "api/user-mgr/roles", role).ConfigureAwait(false);
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

        public async Task<RoleDto> UpdateRoleAsync(RoleDto role)
        {
            try
            {
                var response = await _apiService.PutAsync<RoleDto, RoleDto>("UsersAPI", $"api/user-mgr/roles/{role.RoleId}", role).ConfigureAwait(false);
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

        public async Task<RoleDto> DeleteRoleAsync(Guid id)
        {
            try
            {
                var response = await _apiService.DeleteAsync<RoleDto>("UsersAPI", $"api/user-mgr/roles/{id}").ConfigureAwait(false);
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
