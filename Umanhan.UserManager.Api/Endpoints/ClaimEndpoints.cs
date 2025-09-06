using Microsoft.AspNetCore.OutputCaching;
using System.Security.Claims;
using System.Text.Json;
using Umanhan.Dtos;
using Umanhan.Services;

namespace Umanhan.UserManager.Api.Endpoints
{
    public class ClaimEndpoints
    {
        private readonly RoleService _roleService;
        private readonly RolePermissionService _rolePermissionService;
        private readonly ILogger<ClaimEndpoints> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        //private readonly ICacheService _cacheService;

        private const string MODULE_CACHE_KEY = "claim";
        private const string MODULE_CACHE_TAG = "claim:all";

        public ClaimEndpoints(RoleService roleService, 
            RolePermissionService rolePermissionService, 
            ILogger<ClaimEndpoints> logger, 
            IHttpContextAccessor httpContextAccessor
            //ICacheService cacheService
            )
        {
            _roleService = roleService;
            _rolePermissionService = rolePermissionService;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            //_cacheService = cacheService;
        }

        public async Task<IResult> GetUserClaimsAsync(IOutputCacheStore cache, CancellationToken cancellationToken)
        {
            try
            {
                ClaimsPrincipal principal = _httpContextAccessor.HttpContext.User;
                var identity = principal.Identity as ClaimsIdentity;

                var validRoles = new List<RoleDto>();
                var permissions = new List<RolePermissionDto>();
                var cognitoGroups = principal.FindAll("cognito:groups").Select(c => c.Value).ToList();

                string username = identity.Claims.First(x => x.Type == "username").Value;
                string cacheKeyRoles = $"permissions-{username}";
                var cachedDataRoles = await cache.GetAsync(cacheKeyRoles, cancellationToken).ConfigureAwait(false);
                if (cachedDataRoles != null)
                {
                    var cachedJson = System.Text.Encoding.UTF8.GetString(cachedDataRoles);
                    validRoles = JsonSerializer.Deserialize<List<RoleDto>>(cachedJson);
                }
                else
                {
                    var localRoles = await _roleService.GetAllRolesAsync().ConfigureAwait(false);
                    validRoles = localRoles.Where(x => cognitoGroups.Contains(x.GroupName)).ToList();

                    var rolesJson = JsonSerializer.SerializeToUtf8Bytes(validRoles.Distinct());
                    await cache.SetAsync(cacheKeyRoles, rolesJson, null, TimeSpan.FromMinutes(30), cancellationToken);
                }

                foreach (var role in validRoles)
                {
                    string cacheKey = $"permissions-{role.GroupName}";
                    var cachedData = await cache.GetAsync(cacheKey, cancellationToken);
                    if (cachedData != null)
                    {
                        var cachedJson = System.Text.Encoding.UTF8.GetString(cachedData);
                        var list = JsonSerializer.Deserialize<List<RolePermissionDto>>(cachedJson);
                        permissions.AddRange(list);
                    }
                    else
                    {
                        // Retrieve permissions for each role
                        var list = await _rolePermissionService.GetPermissionsForRoleAsync(role.RoleId, "Role", "Module", "Permission").ConfigureAwait(false);
                        permissions.AddRange(list);

                        var permissionsJson = JsonSerializer.SerializeToUtf8Bytes(permissions);
                        await cache.SetAsync(cacheKey, permissionsJson, null, TimeSpan.FromMinutes(30), cancellationToken);
                    }
                }

                var roleClaimObj = new RoleClaimDto
                {
                    Permissions = permissions.Distinct(),
                    DashboardComponent = validRoles.FirstOrDefault(x => !string.IsNullOrEmpty(x.DashboardTemplateComponentName))?.DashboardTemplateComponentName
                };
                return Results.Ok(roleClaimObj);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching user claims for {Username}", _httpContextAccessor.HttpContext.User.Identity?.Name);
                return Results.Problem(ex.Message, statusCode: 500);
            }
        }

        public async Task<IResult> GetUserClaimsAsync()
        {
            try
            {
                var principal = _httpContextAccessor.HttpContext.User;
                var identity = principal.Identity as ClaimsIdentity;

                var cognitoGroups = principal.FindAll("cognito:groups").Select(c => c.Value).ToList();
                var username = identity.Claims.First(x => x.Type == "username").Value;

                // 1. Cache roles assigned to the user
                //var cacheKeyRoles = $"{MODULE_CACHE_KEY}:user:{username}";

                //var validRoles = await _cacheService.GetOrSetAsync(cacheKeyRoles, async () =>
                //{
                //    var allRoles = await _roleService.GetAllRolesAsync().ConfigureAwait(false);
                //    return allRoles.Where(x => cognitoGroups.Contains(x.GroupName)).ToList();
                //}, tag: MODULE_CACHE_TAG);
                var validRoles = await _roleService.GetAllRolesAsync().ConfigureAwait(false);
                validRoles = validRoles.Where(x => cognitoGroups.Contains(x.GroupName) &&
                                                   x.IsActive
                                             ).ToList();

                // 2. Cache permissions per role (by groupName)
                var permissions = new List<RolePermissionDto>();

                foreach (var role in validRoles)
                {
                    string cacheKeyPermissions = $"{MODULE_CACHE_KEY}:role:{role.GroupName}";
                    //var rolePermissions = await _cacheService.GetOrSetAsync(cacheKeyPermissions, async () =>
                    //{
                    var rolePermissions = await _rolePermissionService
                            .GetPermissionsForRoleAsync(role.RoleId, "Role", "Module", "Permission")
                            .ConfigureAwait(false);
                    //}, tag: MODULE_CACHE_TAG);

                    permissions.AddRange(rolePermissions);
                }

                // 3. Assemble the user claims
                var roleClaimObj = new RoleClaimDto
                {
                    Permissions = permissions.Distinct(),
                    DashboardComponent = validRoles.FirstOrDefault(x => !string.IsNullOrWhiteSpace(x.DashboardTemplateComponentName))?.DashboardTemplateComponentName ?? string.Empty
                };

                return Results.Ok(roleClaimObj);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching user claims for {Username}", _httpContextAccessor.HttpContext.User.Identity?.Name);
                return Results.Problem(ex.Message, statusCode: 500);
            }
        }
    }
}
