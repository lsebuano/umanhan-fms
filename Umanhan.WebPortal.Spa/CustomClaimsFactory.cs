using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication.Internal;
using System.Net.Http.Json;
using System.Security.Claims;
using Umanhan.Models.Models;
using Umanhan.Models.Dtos;
using System.Diagnostics;
using System;

namespace Umanhan.WebPortal.Spa
{
    public class CustomClaimsFactory : AccountClaimsPrincipalFactory<RemoteUserAccount>
    {
        private readonly HttpClient _http;
        private readonly WebAppSetting _settings;

        public CustomClaimsFactory(IAccessTokenProviderAccessor accessor, HttpClient http, WebAppSetting settings) : base(accessor)
        {
            _http = http;
            _settings = settings;
        }

        public async override ValueTask<ClaimsPrincipal> CreateUserAsync(RemoteUserAccount account, RemoteAuthenticationUserOptions options)
        {
            // Get the base principal (with sub, name, email, etc.)
            var user = await base.CreateUserAsync(account, options);
            var identity = (ClaimsIdentity)user.Identity!;

            // Grab the access token so the local API sees this user as authenticated
            var tokenResult = await base.TokenProvider.RequestAccessToken();
            if (tokenResult.TryGetToken(out var token))
            {
                _http.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token.Value);

                // Call the API endpoint that returns List<string> of permissions
                Uri baseUri = new Uri(_settings.WebApiUrlUsers);
                UriBuilder uriBuilder = new UriBuilder(baseUri)
                {
                    Path = "api/user-mgr/claims"
                };
                string url = uriBuilder.ToString();
                var claimsInfo = await _http.GetFromJsonAsync<RoleClaimDto>(url);
                if (claimsInfo is not null)
                {
                    var permissions = claimsInfo.Permissions;
                    foreach (var p in permissions)
                        identity.AddClaim(new Claim("permission", p.Access));

                    identity.AddClaim(new Claim("dashboard_component", claimsInfo.DashboardComponent));
                }
            }

            return user;
        }
    }
}
