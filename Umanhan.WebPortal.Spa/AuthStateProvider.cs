using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace Umanhan.WebPortal.Spa
{
    public class AuthStateProvider : AuthenticationStateProvider
    {
        private Task<AuthenticationState> _authStateTask = Task.FromResult(new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity())));

        public override Task<AuthenticationState> GetAuthenticationStateAsync() => _authStateTask;

        public void LoginAsSuperAdmin()
        {
            var user = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, "SuperAdmin") }, "Custom authentication"));
            var claimsIdentity = new ClaimsIdentity(user.Claims, "???");
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
            _authStateTask = Task.FromResult(new AuthenticationState(claimsPrincipal));
            NotifyAuthenticationStateChanged(_authStateTask);
        }
    }
}
