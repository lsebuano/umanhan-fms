using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;
using FluentValidation;
using Umanhan.Dtos;
using Umanhan.Services;
using Umanhan.Shared;

namespace Umanhan.UserManager.Api.Endpoints
{
    public class RoleEndpoints
    {
        private readonly RoleService _roleService;
        private readonly IValidator<RoleDto> _validator;
        private readonly ILogger<RoleEndpoints> _logger;
        //private readonly ICacheService _cacheService;

        private const string MODULE_CACHE_KEY = "role";

        public RoleEndpoints(RoleService roleService,
            IValidator<RoleDto> validator,
            ILogger<RoleEndpoints> logger
            )
        {
            _roleService = roleService;
            _validator = validator;
            _logger = logger;
            //_cacheService = cacheService;
        }

        public async Task<IResult> GetAllRolesAsync(bool activeOnly = false)
        {
            try
            {
                //var key = $"{MODULE_CACHE_KEY}:list";
                //var result = await _cacheService.GetOrSetAsync(key, async () =>
                //{
                var result = await _roleService.GetAllRolesAsync(activeOnly, "RolePermissions", "RolePermissions.Module", "RolePermissions.Permission").ConfigureAwait(false);
                //    return result;
                //});
                return Results.Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching all roles");
                return Results.Problem(ex.Message);
            }
        }

        public async Task<IResult> GetRolesExceptAsync(UserStateServiceForService userState)
        {
            try
            {
                string exceptRole = string.Empty;
                var superAdminRole = "SuperAdmin";
                bool isSuperAdmin = userState.IsSuperAdmin;
                if (!isSuperAdmin)
                    exceptRole = superAdminRole;

                var result = await _roleService.GetRolesExceptAsync(
                    except: x => x.Name.Equals(exceptRole), 
                    "RolePermissions.Module", "RolePermissions.Permission"
                );
                return Results.Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching all roles");
                return Results.Problem(ex.Message);
            }
        }

        public async Task<IResult> GetAllGroupsAsync(IAmazonCognitoIdentityProvider cognitoClient, string userPoolId)
        {
            try
            {
                var result = new List<GroupType>();
                string nextToken = null;

                do
                {
                    var request = new ListGroupsRequest
                    {
                        UserPoolId = userPoolId,
                        Limit = 60,
                        NextToken = nextToken
                    };

                    var response = await cognitoClient.ListGroupsAsync(request).ConfigureAwait(false);
                    result.AddRange(response.Groups);
                    nextToken = response.NextToken;

                } while (!string.IsNullOrEmpty(nextToken));

                var list = result.Select(x => new RoleDto
                {
                    GroupName = x.GroupName
                }).ToList();
                return Results.Ok(list);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching all groups from Cognito");
                return Results.Problem(ex.Message);
            }
        }

        public async Task<IResult> GetRoleByIdAsync(Guid id)
        {
            try
            {
                var farmRole = await _roleService.GetRoleByIdAsync(id).ConfigureAwait(false);
                return Results.Ok(farmRole);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching role by ID {Id}", id);
                return Results.Problem(ex.Message);
            }
        }

        public async Task<IResult> CreateRoleAsync(IAmazonCognitoIdentityProvider cognitoClient, RoleDto role, string userPoolId)
        {
            var validationResult = await _validator.ValidateAsync(role).ConfigureAwait(false);
            if (!validationResult.IsValid)
                return Results.ValidationProblem(validationResult.ToDictionary());

            try
            {
                var createReq = new CreateGroupRequest
                {
                    UserPoolId = userPoolId,
                    GroupName = role.GroupName,
                    Description = role.GroupName,
                    //Precedence = role.Precedence
                };
                var resp = await cognitoClient.CreateGroupAsync(createReq).ConfigureAwait(false);
                if (resp.HttpStatusCode != System.Net.HttpStatusCode.OK)
                    return Results.Problem("Failed to create group in Cognito");

                var newRole = await _roleService.CreateRoleAsync(role).ConfigureAwait(false);
                //_ = _cacheService.RemoveAsync($"{MODULE_CACHE_KEY}:list").ConfigureAwait(false);
                return Results.Created($"/api/user-mgr/roles/{newRole.RoleId}", newRole);
            }
            catch (GroupExistsException ex)
            {
                _logger.LogError(ex, "Group already exists: {GroupName}", role.GroupName);
                return Results.Problem($"Group already exists: {role.GroupName}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating role");
                return Results.Problem(ex.Message);
            }
        }

        public async Task<IResult> UpdateRoleAsync(IAmazonCognitoIdentityProvider cognitoClient, Guid id, RoleDto role, string userPoolId)
        {
            if (id != role.RoleId)
                return Results.BadRequest("Role ID mismatch");

            var validationResult = await _validator.ValidateAsync(role).ConfigureAwait(false);
            if (!validationResult.IsValid)
                return Results.ValidationProblem(validationResult.ToDictionary());

            try
            {
                var updateReq = new UpdateGroupRequest
                {
                    GroupName = role.GroupName,
                    UserPoolId = userPoolId,
                    Description = role.GroupName,
                    //Precedence = role.Precedence
                };
                var resp = await cognitoClient.UpdateGroupAsync(updateReq).ConfigureAwait(false);
                if (resp.HttpStatusCode != System.Net.HttpStatusCode.OK)
                    return Results.Problem("Failed to update group in Cognito");

                var updatedRole = await _roleService.UpdateRoleAsync(role).ConfigureAwait(false);
                if (updatedRole is not null)
                {
                    //// Clear cache for the list of modules
                    //_ = _cacheService.RemoveAsync($"{MODULE_CACHE_KEY}:list").ConfigureAwait(false);
                    return Results.Ok(updatedRole);
                }
                return Results.NotFound();
            }
            catch (ResourceNotFoundException)
            {
                _logger.LogError("Group not found: {GroupName}", role.GroupName);
                return Results.NotFound(new { Error = $"Group '{role.GroupName}' not found." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating role");
                return Results.Problem(ex.Message);
            }
        }

        public async Task<IResult> DeleteRoleAsync(IAmazonCognitoIdentityProvider cognitoClient, Guid id, string groupName, string userPoolId)
        {
            try
            {
                var resp = await cognitoClient.DeleteGroupAsync(new DeleteGroupRequest
                {
                    GroupName = groupName,
                    UserPoolId = userPoolId
                }).ConfigureAwait(false);

                if (resp.HttpStatusCode != System.Net.HttpStatusCode.OK)
                    return Results.Problem("Failed to delete group in Cognito");

                var deletedRole = await _roleService.DeleteRoleAsync(id).ConfigureAwait(false);
                if (deletedRole is not null)
                {
                    //// Clear cache for the list of modules
                    //_ = _cacheService.RemoveAsync($"{MODULE_CACHE_KEY}:list").ConfigureAwait(false);
                    return Results.Ok(deletedRole);
                }
                return Results.NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting role with ID {Id}", id);
                return Results.Problem(ex.Message);
            }
        }
    }
}
