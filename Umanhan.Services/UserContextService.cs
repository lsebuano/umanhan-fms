using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Umanhan.Models.Entities;
using Umanhan.Services.Interfaces;

namespace Umanhan.Services
{
    public class UserContextService : IUserContextService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<UserContextService> _logger;

        private static readonly string[] _usernameClaimTypes =
        {
            "email",
            "cognito:username",
            "preferred_username",
            ClaimTypes.Email,
            ClaimTypes.NameIdentifier,
            ClaimTypes.Name,
        };

        public UserContextService(IHttpContextAccessor httpContextAccessor,
            ILogger<UserContextService> logger)
        {
            _httpContextAccessor = httpContextAccessor;
            this._logger = logger;
        }

        public string Username
        {
            get
            {
                var user = _httpContextAccessor.HttpContext?.User;
                if (user?.Identity?.IsAuthenticated != true)
                    return "anonymous";

                // Try each potential claim type in order
                foreach (var claimType in _usernameClaimTypes)
                {
                    var claim = user.FindFirst(claimType);
                    if (claim != null && !string.IsNullOrWhiteSpace(claim.Value))
                        return claim.Value;
                }
                //return user.Claims.FirstOrDefault(c => c.Type == "email")?.Value ?? "anonymous";
                return "anonymous";
            }
        }
    }

}
