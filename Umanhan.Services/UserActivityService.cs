using Microsoft.Extensions.Logging;
using Umanhan.Dtos;
using Umanhan.Models.LoggerEntities;
using Umanhan.Repositories.LoggerContext.Interfaces;
using Umanhan.Services.Interfaces;

namespace Umanhan.Services
{
    public class UserActivityService
    {
        private readonly ILoggerUnitOfWork _unitOfWork;
        private readonly IUserContextService _userContext;
        private readonly ILogger<UserActivityService> _logger;

        public UserActivityService(ILoggerUnitOfWork unitOfWork, 
            IUserContextService userContext,
            ILogger<UserActivityService> logger)
        {
            this._unitOfWork = unitOfWork;
            this._userContext = userContext;
            this._logger = logger;
        }

        private static List<UserActivityDto> ToUserActivityDto(IEnumerable<UserActivity> userActivities)
        {
            return userActivities.Select(x => new UserActivityDto
            {
                UserActivityId = x.Id,
                FarmId = x.FarmId,
                Timestamp = x.Timestamp,
                IpAddress = x.IpAddress,
                ModuleName = x.ModuleName,
                Path = x.Path,
                Properties = x.Properties,
                Username = x.Username,
            })
            .OrderByDescending(x => x.Timestamp)
            .ToList();
        }

        private static UserActivityDto ToUserActivityDto(UserActivity userActivity)
        {
            return new UserActivityDto
            {
                UserActivityId = userActivity.Id,
                FarmId = userActivity.FarmId,
                Timestamp = userActivity.Timestamp,
                IpAddress = userActivity.IpAddress,
                ModuleName = userActivity.ModuleName,
                Path = userActivity.Path,
                Properties = userActivity.Properties,
                Username = userActivity.Username
            };
        }

        public async Task<IEnumerable<UserActivityDto>> GetUserActivitiesAsync(Guid farmId, DateTime date)
        {
            var obj = await _unitOfWork.UserActivities.GetUserActivitiesAsync(farmId, date).ConfigureAwait(false);
            if (obj == null)
                return null;
            return ToUserActivityDto(obj);
        }

        public async Task<IEnumerable<UserActivityDto>> GetUserActivitiesAsync(Guid farmId, string username)
        {
            var obj = await _unitOfWork.UserActivities.GetUserActivitiesAsync(farmId, username).ConfigureAwait(false);
            if (obj == null)
                return null;
            return ToUserActivityDto(obj);
        }

        public async Task<UserActivityDto> GetUserActivityByIdAsync(Guid id, params string[] includeEntities)
        {
            var obj = await _unitOfWork.UserActivities.GetByIdAsync(id, includeEntities).ConfigureAwait(false);
            if (obj == null)
                return null;
            return ToUserActivityDto(obj);
        }

        public async Task<UserActivityDto> CreateUserActivityAsync(UserActivityDto userActivity)
        {
            var newUserActivity = new UserActivity
            {
                Properties = userActivity.Properties,
                Timestamp = userActivity.Timestamp,
                IpAddress = userActivity.IpAddress,
                ModuleName = userActivity.ModuleName,
                Path = userActivity.Path,
                Username = userActivity.Username,
                FarmId = userActivity.FarmId,
            };

            try
            {
                var createdUserActivity = await _unitOfWork.UserActivities.AddAsync(newUserActivity).ConfigureAwait(false);
                await _unitOfWork.CommitAsync().ConfigureAwait(false);

                return ToUserActivityDto(createdUserActivity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating user activity for user {Username} on farm {FarmId}", 
                    userActivity.Username, userActivity.FarmId);
                throw;
            }
        }
    }
}
