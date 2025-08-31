using Umanhan.Models.Dtos;
using Umanhan.Models.Entities;
using Umanhan.Models.Models;

namespace Umanhan.WebPortal.Spa.Services
{
    public class UserActivityService
    {
        private readonly ApiService _apiService;

        public UserActivityService(ApiService apiService)
        {
            _apiService = apiService;
        }

        public Task<ApiResponse<IEnumerable<UserActivityDto>>> GetUserActivitiesAsync(Guid farmId, DateTime date)
        {
            try
            {
                return _apiService.GetAsync<IEnumerable<UserActivityDto>>("LoggerAPI", $"api/user-activities/{farmId}/{date:O}");
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Task<ApiResponse<IEnumerable<UserActivityDto>>> GetUserActivitiesAsync(Guid farmId, string username)
        {
            try
            {
                return _apiService.GetAsync<IEnumerable<UserActivityDto>>("LoggerAPI", $"api/user-activities/{farmId}/username/{username}");
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Task<ApiResponse<UserActivityDto>> GetUserActivityByIdAsync(Guid id)
        {
            try
            {
                return _apiService.GetAsync<UserActivityDto>("LoggerAPI", $"api/user-activities/{id}");
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<UserActivityDto> CreateUserActivityAsync(UserActivityDto userActivity)
        {
            try
            {
                var response = await _apiService.PostAsync<UserActivityDto, UserActivityDto>("LoggerAPI", "api/user-activities", userActivity).ConfigureAwait(false);
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
