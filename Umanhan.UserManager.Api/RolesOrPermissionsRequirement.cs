using Microsoft.AspNetCore.Authorization;

namespace Umanhan.UserManager.Api
{
    public class RolesOrPermissionsRequirement : IAuthorizationRequirement
    {
        public string[] RequiredClaims { get; }

        public RolesOrPermissionsRequirement(string[] requiredClaims)
        {
            RequiredClaims = requiredClaims;
        }
    }

    public class RolesOrPermissionsHandler : AuthorizationHandler<RolesOrPermissionsRequirement>
    {
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            RolesOrPermissionsRequirement requirement)
        {
            foreach (var claim in requirement.RequiredClaims)
            {
                if (context.User.HasClaim("permission", claim) || context.User.IsInRole(claim))
                {
                    context.Succeed(requirement);
                    break;
                }
            }

            return Task.CompletedTask;
        }
    }

}
