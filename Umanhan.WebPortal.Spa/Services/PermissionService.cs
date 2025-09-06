using Microsoft.AspNetCore.Components.Authorization;
using Umanhan.WebPortal.Spa.Services.Interfaces;

namespace Umanhan.WebPortal.Spa.Services
{
    public class PermissionService : IPermissionService
    {
        private readonly AuthenticationStateProvider _auth;

        public PermissionService(AuthenticationStateProvider auth) => _auth = auth;

        public async Task<IEnumerable<string>> GetPermissions()
        {
            var user = (await _auth.GetAuthenticationStateAsync()).User;
            return user.Claims.Where(x => x.Type == "permission").Select(x => x.Value);
        }

        public async Task<bool> HasPermissionAsync(string permission)
        {
            var user = (await _auth.GetAuthenticationStateAsync()).User;
            return user.HasPermission(permission);
        }
    }

}
