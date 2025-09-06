using FluentValidation;
using Umanhan.Dtos;
using Umanhan.Services;

namespace Umanhan.UserManager.Api.Endpoints
{
    public class RolePermissionEndpoints
    {
        private readonly RolePermissionService _rolePermissionService;
        private readonly IValidator<RolePermissionDto> _validator;
        private readonly ILogger<RolePermissionEndpoints> _logger;
        //private readonly ICacheService _cacheService;

        private const string MODULE_CACHE_KEY = "rolepermission";
        private const string MODULE_CACHE_TAG = "rolepermission:list:all";
        private const string MODULE_CACHE_TAG_CLAIM = "claim:all";

        public RolePermissionEndpoints(RolePermissionService rolePermissionService,
            IValidator<RolePermissionDto> validator,
            ILogger<RolePermissionEndpoints> logger)
        {
            _rolePermissionService = rolePermissionService;
            _validator = validator;
            _logger = logger;
            //_cacheService = cacheService;
        }

        public async Task<IResult> GetAllRolePermissionsAsync()
        {
            try
            {
                var key = $"{MODULE_CACHE_KEY}:list";
                //var result = await _cacheService.GetOrSetAsync(key, async () =>
                //{
                var result = await _rolePermissionService.GetAllRolePermissionsAsync("Role", "Module", "Permission").ConfigureAwait(false);
                //    return result;
                //}, tag: MODULE_CACHE_TAG);
                return Results.Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching all role permissions");
                return Results.Problem(ex.Message);
            }
        }

        public async Task<IResult> GetPermissionsForRoleAsync(Guid roleId)
        {
            try
            {
                var key = $"{MODULE_CACHE_KEY}:list:{roleId}";
                //var result = await _cacheService.GetOrSetAsync(key, async () =>
                //{
                var result = await _rolePermissionService.GetPermissionsForRoleAsync(roleId, "Role", "Module", "Permission").ConfigureAwait(false);
                //    return result;
                //}, tag: MODULE_CACHE_TAG);
                return Results.Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching permissions for role {RoleId}", roleId);
                return Results.Problem(ex.Message);
            }
        }

        public async Task<IResult> GetRolePermissionByIdAsync(Guid id)
        {
            try
            {
                var farmRolePermission = await _rolePermissionService.GetRolePermissionByIdAsync(id).ConfigureAwait(false);
                return Results.Ok(farmRolePermission);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching role permission by ID {Id}", id);
                return Results.Problem(ex.Message);
            }
        }

        public async Task<IResult> CreateRolePermissionAsync(RolePermissionDto rolePermission)
        {
            var validationResult = await _validator.ValidateAsync(rolePermission).ConfigureAwait(false);
            if (!validationResult.IsValid)
                return Results.ValidationProblem(validationResult.ToDictionary());

            try
            {
                var newRolePermission = await _rolePermissionService.CreateRolePermissionAsync(rolePermission).ConfigureAwait(false);
                //_ = _cacheService.InvalidateTagAsync(MODULE_CACHE_TAG);
                //_ = _cacheService.InvalidateTagAsync(MODULE_CACHE_TAG_CLAIM);

                return Results.Created($"/api/user-mgr/role-permissions/{newRolePermission.RolePermissionId}", newRolePermission);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating role permission");
                return Results.Problem(ex.Message);
            }
        }

        public async Task<IResult> UpdateRolePermissionAsync(Guid id, RolePermissionDto rolePermission)
        {
            if (id != rolePermission.RolePermissionId)
                return Results.BadRequest("Role-Permission ID mismatch");

            var validationResult = await _validator.ValidateAsync(rolePermission).ConfigureAwait(false);
            if (!validationResult.IsValid)
                return Results.ValidationProblem(validationResult.ToDictionary());

            try
            {
                var updatedRolePermission = await _rolePermissionService.UpdateRolePermissionAsync(rolePermission).ConfigureAwait(false);
                if (updatedRolePermission is not null)
                {
                    //// Clear cache for the list of modules
                    //_ = _cacheService.InvalidateTagAsync(MODULE_CACHE_TAG);
                    //_ = _cacheService.InvalidateTagAsync(MODULE_CACHE_TAG_CLAIM);

                    return Results.Ok(updatedRolePermission);
                }
                return Results.NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating role permission with ID {Id}", id);
                return Results.Problem(ex.Message);
            }
        }

        public async Task<IResult> DeleteRolePermissionAsync(Guid id)
        {
            try
            {
                var deletedRolePermission = await _rolePermissionService.DeleteRolePermissionAsync(id).ConfigureAwait(false);
                if (deletedRolePermission is not null)
                {
                    //// Clear cache for the list of modules
                    //_ = _cacheService.InvalidateTagAsync(MODULE_CACHE_TAG);
                    //_ = _cacheService.InvalidateTagAsync(MODULE_CACHE_TAG_CLAIM);

                    return Results.Ok(deletedRolePermission);
                }
                return Results.NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting role permission with ID {Id}", id);
                return Results.Problem(ex.Message);
            }
        }
    }
}
