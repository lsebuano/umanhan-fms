using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Umanhan.Models.Attributes;
using Umanhan.Models.Models;
using Umanhan.Services;
using Umanhan.Services.Interfaces;

namespace Umanhan.Masterdata.Api
{
    public class PermissionHandler : AuthorizationHandler<PermissionRequirement>
    {
        private readonly IPermissionService _permissionService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PermissionHandler(IPermissionService permissionService,
                                 IHttpContextAccessor httpContextAccessor)
        {
            _permissionService = permissionService;
            _httpContextAccessor = httpContextAccessor;
        }

        protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            PermissionRequirement requirement)
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext == null) return;

            var endpoint = httpContext.GetEndpoint();
            if (endpoint == null) return;

            var attribute = endpoint.Metadata.GetMetadata<RequiresPermissionAttribute>();
            if (attribute == null) return; // no permission needed

            var requiredPermission = attribute.PermissionName; // e.g. "Farm.Read" or "Users.Full"

            if (!context.User.Identity!.IsAuthenticated)
                return;

            var groupClaims = context.User
                .FindAll("cognito:groups")
                .Select(c => c.Value)
                .ToList();

            if (!groupClaims.Any())
                return;

            var hasAccess = await _permissionService.HasPermissionAsync(groupClaims, requiredPermission).ConfigureAwait(false);
            if (hasAccess)
                context.Succeed(requirement);

            // else do nothing, final result is 403 Forbidden
        }
    }
}
