using FluentValidation;
using Umanhan.Dtos;
using Umanhan.Services.Interfaces;

namespace Umanhan.Masterdata.Api.Endpoints
{
    public class PermissionEndpoints
    {
        private readonly IPermissionService _permissionService;
        private readonly IValidator<PermissionDto> _validator;
        private readonly ILogger<PermissionEndpoints> _logger;
        //private readonly ICacheService _cacheService;

        private const string MODULE_CACHE_KEY = "permission";

        public PermissionEndpoints(IPermissionService permissionService, IValidator<PermissionDto> validator, ILogger<PermissionEndpoints> logger)
        {
            _permissionService = permissionService;
            _validator = validator;
            _logger = logger;
            //_cacheService = cacheService;
        }

        public async Task<IResult> GetAllPermissionsAsync()
        {
            try
            {
                string key = $"{MODULE_CACHE_KEY}:list";
                //var result = await _cacheService.GetOrSetAsync(key, async () =>
                //{
                var result = await _permissionService.GetAllPermissionsAsync().ConfigureAwait(false);
                //    return result;
                //});
                return Results.Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving permissions");
                return Results.Problem(ex.Message);
            }
        }

        public async Task<IResult> GetPermissionByIdAsync(Guid id)
        {
            try
            {
                var permission = await _permissionService.GetPermissionByIdAsync(id).ConfigureAwait(false);
                return permission is not null ? Results.Ok(permission) : Results.NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving permission with ID {PermissionId}", id);
                return Results.Problem(ex.Message);
            }
        }

        public async Task<IResult> CreatePermissionAsync(PermissionDto permission)
        {
            var validationResult = await _validator.ValidateAsync(permission).ConfigureAwait(false);
            if (!validationResult.IsValid)
                return Results.ValidationProblem(validationResult.ToDictionary());

            try
            {
                var newPermission = await _permissionService.CreatePermissionAsync(permission).ConfigureAwait(false);

                //string key = $"{MODULE_CACHE_KEY}:list";
                //_ = _cacheService.RemoveAsync(key);

                return Results.Created($"/api/permissions/{newPermission.PermissionId}", newPermission);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating permission");
                return Results.Problem(ex.Message);
            }
        }

        public async Task<IResult> UpdatePermissionAsync(Guid id, PermissionDto permission)
        {
            if (id != permission.PermissionId)
                return Results.BadRequest("Permission ID mismatch");

            var validationResult = await _validator.ValidateAsync(permission).ConfigureAwait(false);
            if (!validationResult.IsValid)
            {
                return Results.ValidationProblem(validationResult.ToDictionary());
            }

            try
            {
                var updatedPermission = await _permissionService.UpdatePermissionAsync(permission).ConfigureAwait(false);
                if (updatedPermission is not null)
                {
                    //// Clear cache for the list of system settings
                    //_ = _cacheService.RemoveAsync($"{MODULE_CACHE_KEY}:list");
                    return Results.Ok(updatedPermission);
                }
                return Results.NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating permission with ID {PermissionId}", id);
                return Results.Problem(ex.Message);
            }
        }

        public async Task<IResult> DeletePermissionAsync(Guid id)
        {
            try
            {
                var deletedPermission = await _permissionService.DeletePermissionAsync(id).ConfigureAwait(false);
                if (deletedPermission is not null)
                {
                    //// Clear cache for the list of system settings
                    //_ = _cacheService.RemoveAsync($"{MODULE_CACHE_KEY}:list");
                    return Results.Ok(deletedPermission);
                }
                return Results.NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting permission with ID {PermissionId}", id);
                return Results.Problem(ex.Message);
            }
        }
    }
}