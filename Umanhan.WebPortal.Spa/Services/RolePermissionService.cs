using Umanhan.Dtos;
using Umanhan.Dtos.HelperModels;

namespace Umanhan.WebPortal.Spa.Services
{
    public class RolePermissionService
    {
        private readonly ApiService _apiService;
        //private readonly CustomAuthStateProvider _authService;
        private readonly UserService _userService;

        public List<string> Permissions { get; set; } = [];

        public bool HasPermission(string permission)
        {
            return Permissions.Contains(permission);
        }

        public RolePermissionService(ApiService apiService, UserService userService)
        {
            _apiService = apiService;
            _userService = userService;
        }

        public async Task<List<string>> LoadAsync()
        {
            var result = await _userService.GetUserClaimsAsync().ConfigureAwait(false);
            if (result.IsSuccess)
            {
                var claims = result.Data;
                if (claims != null)
                {
                    Permissions = claims.Select(x => x.Access).ToList();
                }
            }

            return Permissions;
        }

        public Task<ApiResponse<IEnumerable<RolePermissionDto>>> GetAllRolePermissionsAsync()
        {
            try
            {
                return _apiService.GetAsync<IEnumerable<RolePermissionDto>>("UsersAPI", "api/user-mgr/role-permissions");
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Task<ApiResponse<IEnumerable<RolePermissionDto>>> GetPermissionsForRoleAsync(Guid roleId)
        {
            try
            {
                return _apiService.GetAsync<IEnumerable<RolePermissionDto>>("UsersAPI", $"api/user-mgr/role-permissions/role/{roleId}");
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Task<ApiResponse<RolePermissionDto>> GetRolePermissionByIdAsync(Guid id)
        {
            try
            {
                return _apiService.GetAsync<RolePermissionDto>("UsersAPI", $"api/user-mgr/role-permissions/{id}");
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<RolePermissionDto> CreateRolePermissionAsync(RolePermissionDto rolePermission)
        {
            try
            {
                var response = await _apiService.PostAsync<RolePermissionDto, RolePermissionDto>("UsersAPI", "api/user-mgr/role-permissions", rolePermission).ConfigureAwait(false);
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

        public async Task<RolePermissionDto> UpdateRolePermissionAsync(RolePermissionDto rolePermission)
        {
            try
            {
                var response = await _apiService.PutAsync<RolePermissionDto, RolePermissionDto>("UsersAPI", $"api/user-mgr/role-permissions/{rolePermission.RolePermissionId}", rolePermission).ConfigureAwait(false);
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

        public async Task<RolePermissionDto> DeleteRolePermissionAsync(Guid id)
        {
            try
            {
                var response = await _apiService.DeleteAsync<RolePermissionDto>("UsersAPI", $"api/user-mgr/role-permissions/{id}").ConfigureAwait(false);
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
