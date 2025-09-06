using FluentValidation;
using Umanhan.Dtos;
using Umanhan.Services;

namespace Umanhan.Settings.Api.Endpoints
{
    public class SystemSettingEndpoints
    {
        private readonly SystemSettingService _systemSettingService;
        private readonly IValidator<SystemSettingDto> _validator;
        private readonly ILogger<SystemSettingEndpoints> _logger;
        //private readonly ICacheService _cacheService;

        private const string MODULE_CACHE_KEY = "systemsettings";

        public SystemSettingEndpoints(SystemSettingService systemSettingService,
            IValidator<SystemSettingDto> validator,
            ILogger<SystemSettingEndpoints> logger
            )
        {
            _systemSettingService = systemSettingService;
            _validator = validator;
            _logger = logger;
            //_cacheService = cacheService;
        }

        public async Task<IResult> GetAllSystemSettingsAsync()
        {
            try
            {
                string key = $"{MODULE_CACHE_KEY}:list";
                //var result = await _cacheService.GetOrSetAsync(key, async () =>
                //{
                var result = await _systemSettingService.GetAllSystemSettingsAsync().ConfigureAwait(false);
                //    return result;
                //});
                return Results.Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all system settings");
                return Results.Problem(ex.Message);
            }
        }

        public async Task<IResult> GetSystemSettingByIdAsync(Guid id)
        {
            try
            {
                var systemSetting = await _systemSettingService.GetSystemSettingByIdAsync(id).ConfigureAwait(false);
                return systemSetting is not null ? Results.Ok(systemSetting) : Results.NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving system setting with ID {SystemSettingId}", id);
                return Results.Problem(ex.Message);
            }
        }

        public async Task<IResult> GetSystemSettingByNameAsync(string name)
        {
            try
            {
                var systemSetting = await _systemSettingService.GetSystemSettingByNameAsync(name).ConfigureAwait(false);
                return systemSetting is not null ? Results.Ok(systemSetting) : Results.NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving system setting with name {SystemSettingName}", name);
                return Results.Problem(ex.Message);
            }
        }

        public async Task<IResult> CreateSystemSettingAsync(SystemSettingDto systemSetting)
        {
            var validationResult = await _validator.ValidateAsync(systemSetting).ConfigureAwait(false);
            if (!validationResult.IsValid)
                return Results.ValidationProblem(validationResult.ToDictionary());

            try
            {
                var newSystemSetting = await _systemSettingService.CreateSystemSettingAsync(systemSetting).ConfigureAwait(false);

                //// Clear cache for the list of system settings
                //_ = _cacheService.RemoveAsync($"{MODULE_CACHE_KEY}:list");

                return Results.Created($"/api/system-settings/{newSystemSetting.SettingId}", newSystemSetting);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating system setting");
                return Results.Problem(ex.Message);
            }
        }

        public async Task<IResult> UpdateSystemSettingAsync(Guid id, SystemSettingDto systemSetting)
        {
            if (id != systemSetting.SettingId)
                return Results.BadRequest("Setting ID mismatch");

            var validationResult = await _validator.ValidateAsync(systemSetting).ConfigureAwait(false);
            if (!validationResult.IsValid)
            {
                return Results.ValidationProblem(validationResult.ToDictionary());
            }

            try
            {
                var updatedSystemSetting = await _systemSettingService.UpdateSystemSettingAsync(systemSetting).ConfigureAwait(false);
                if (updatedSystemSetting is not null)
                {
                    //// Clear cache for the list of system settings
                    //_ = _cacheService.RemoveAsync($"{MODULE_CACHE_KEY}:list");
                    return Results.Ok(updatedSystemSetting);
                }
                return Results.NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating system setting with ID {SystemSettingId}", id);
                return Results.Problem(ex.Message);
            }
        }

        public async Task<IResult> DeleteSystemSettingAsync(Guid id)
        {
            try
            {
                var deletedSystemSetting = await _systemSettingService.DeleteSystemSettingAsync(id).ConfigureAwait(false);
                if (deletedSystemSetting is not null)
                {
                    //// Clear cache for the list of system settings
                    //_ = _cacheService.RemoveAsync($"{MODULE_CACHE_KEY}:list");
                    return Results.Ok(deletedSystemSetting);
                }
                return Results.NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting system setting with ID {SystemSettingId}", id);
                return Results.Problem(ex.Message);
            }
        }
    }
}
