using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Umanhan.Shared
{
    public class UserStateServiceForService
    {
        private readonly ClaimsPrincipal _user;

        private IEnumerable<string> GetRolesFromCognito(ClaimsPrincipal user)
        {
            try
            {
                var rawGroupClaims = user.FindAll("cognito:groups");
                return rawGroupClaims.Select(x => x.Value);
            }
            catch
            {
                return []; // fallback if malformed
            }
        }

        public UserStateServiceForService(ClaimsPrincipal user)
        {
            _user = user;
        }

        public bool IsAuthenticated => _user?.Identity?.IsAuthenticated == true;

        public string? Username => _user?.Identity?.Name;

        public IEnumerable<string> Roles
        {
            get
            {
                return GetRolesFromCognito(_user);
            }
        }

        public bool IsInRole(string role)
        {
            if (!IsAuthenticated || string.IsNullOrWhiteSpace(role))
                return false;

            var exists = Roles.Any(c => c.Equals(role, StringComparison.OrdinalIgnoreCase));
            return exists;
        }

        public bool IsAdmin => IsInRole("Admin");

        public bool IsSuperAdmin => IsInRole("SuperAdmin");
    }
}
