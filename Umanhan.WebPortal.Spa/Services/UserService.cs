using Umanhan.Dtos;
using Umanhan.Dtos.HelperModels;

namespace Umanhan.WebPortal.Spa.Services
{
    public class UserService
    {
        private readonly ApiService _apiService;

        public UserService(ApiService apiService)
        {
            _apiService = apiService;
        }

        public Task<ApiResponse<IEnumerable<UserDto>>> GetAllUsersAsync()
        {
            try
            {
                return _apiService.GetAsync<IEnumerable<UserDto>>("UsersAPI", "api/user-mgr/users");
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Task<ApiResponse<IEnumerable<RolePermissionDto>>> GetUserClaimsAsync()
        {
            try
            {
                return _apiService.GetAsync<IEnumerable<RolePermissionDto>>("UsersAPI", "api/user-mgr/claims");
            }
            catch (Exception ex)
            {
                // log
                throw;
            }
        }

        public Task<ApiResponse<UserDto>> GetUserByEmailAsync(string email)
        {
            try
            {
                return _apiService.GetAsync<UserDto>("UsersAPI", $"api/user-mgr/users/{email}");
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<UserDto> CreateUserAsync(UserDto user)
        {
            try
            {
                user.Username = user.Email;
                //user.TemporaryPassword = "";// DO NOT GENERATE THIS HERE!!!

                var response = await _apiService.PostAsync<UserDto, UserDto>("UsersAPI", "api/user-mgr/users", user).ConfigureAwait(false);
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

        public async Task<UserDto> UpdateUserAsync(UserDto user)
        {
            try
            {
                var response = await _apiService.PutAsync<UserDto, UserDto>("UsersAPI", $"api/user-mgr/users/{user.UserId}", user).ConfigureAwait(false);
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

        public async Task<UserDto> DeleteUserAsync(Guid id)
        {
            try
            {
                var response = await _apiService.DeleteAsync<UserDto>("UsersAPI", $"api/user-mgr/users/{id}").ConfigureAwait(false);
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

        public async Task<string> DisableCognitoUserAsync(string username)
        {
            try
            {
                var response = await _apiService.PostAsync<string, object>("UsersAPI", $"api/user-mgr/users/{username}/disable", username).ConfigureAwait(false);
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
                return response.StatusCode.ToString();
            }
            catch (Exception ex)
            {

            }
            return null;
        }

        public async Task<string> LogoutCognitoUserAsync(string username)
        {
            try
            {
                var response = await _apiService.PostAsync<string, object>("UsersAPI", $"api/user-mgr/users/{username}/force-logout", username).ConfigureAwait(false);
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
                return response.StatusCode.ToString();
            }
            catch (Exception ex)
            {

            }
            return null;
        }

        public async Task<string> RemoveCognitoUserFromGroupAsync(string username, string groupName)
        {
            try
            {
                var response = await _apiService.PostAsync<string, string>("UsersAPI", $"api/user-mgr/users/{username}/remove-from/{groupName}", null).ConfigureAwait(false);
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
                return response.StatusCode.ToString();
            }
            catch (Exception ex)
            {

            }
            return null;
        }

        public async Task<string> AddUserToRolesAsync(string username, IEnumerable<string> groups)
        {
            try
            {
                var response = await _apiService.PostAsJsonAsync<IEnumerable<string>, object>("UsersAPI", $"api/user-mgr/users/{username}/groups", groups).ConfigureAwait(false);
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
                return response.StatusCode.ToString();
            }
            catch (Exception ex)
            {

            }
            return null;
        }
    }
}
