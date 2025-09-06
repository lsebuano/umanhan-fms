using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using System.Text.Json;

namespace Umanhan.WebPortal.Spa.Services
{
    public class UserStateService : IDisposable
    {
        private readonly AuthenticationStateProvider _authProvider;
        private ClaimsPrincipal _user = new ClaimsPrincipal(new ClaimsIdentity());
        private TaskCompletionSource<bool> _readyTcs = new(TaskCreationOptions.RunContinuationsAsynchronously);
        private bool _disposed;

        // public observability
        public event Action? OnChange;
        public bool IsReady { get; private set; }
        public Task WhenReady => _readyTcs.Task;

        public UserStateService(AuthenticationStateProvider authProvider)
        {
            _authProvider = authProvider ?? throw new ArgumentNullException(nameof(authProvider));
            _authProvider.AuthenticationStateChanged += HandleAuthStateChanged;
            _ = InitializeAsync(); // kick off initial load
        }

        private async Task InitializeAsync()
        {
            var authState = await _authProvider.GetAuthenticationStateAsync().ConfigureAwait(false);
            _user = authState.User ?? new ClaimsPrincipal(new ClaimsIdentity());
            SetReadyAndNotify();
        }

        private async void HandleAuthStateChanged(Task<AuthenticationState> task)
        {
            try
            {
                var authState = await task.ConfigureAwait(false);
                _user = authState.User ?? new ClaimsPrincipal(new ClaimsIdentity());
                SetReadyAndNotify();
            }
            catch
            {
                // swallow - AuthenticationStateChanged may throw in rare circumstances; keep service usable
            }
        }

        private void SetReadyAndNotify()
        {
            if (!IsReady)
            {
                IsReady = true;
                _readyTcs.TrySetResult(true);
            }
            OnChange?.Invoke();
        }

        // --- Debug helper: expose raw claims for diagnostics ---
        public IEnumerable<Claim> RawClaims => _user.Claims;

        // --- IsAuthenticated ---
        public bool IsAuthenticated => _user.Identity?.IsAuthenticated ?? false;
        public string Username => _user.Identity?.Name ?? "anonymous";
        public async Task<bool> IsAuthenticatedAsync()
        {
            var state = await _authProvider.GetAuthenticationStateAsync().ConfigureAwait(false);
            return state.User.Identity?.IsAuthenticated ?? false;
        }

        // --- Roles parsing ---
        // Accepts many claim types and robustly parses JSON arrays, comma lists, or single values
        private IEnumerable<string> ParseRolesFromClaimValue(string? raw)
        {
            if (string.IsNullOrWhiteSpace(raw)) yield break;

            raw = raw.Trim();

            // Try JSON array first
            if ((raw.StartsWith("[") && raw.EndsWith("]")) || raw.Contains("\""))
            {
                //try
                //{
                    var arr = JsonSerializer.Deserialize<List<string>>(raw);
                    if (arr != null)
                    {
                        foreach (var r in arr.Where(x => !string.IsNullOrWhiteSpace(x)))
                            yield return r.Trim();
                        yield break;
                    }
                //}
                //catch
                //{
                //    // fallthrough to other parsers
                //}
            }

            // Comma-separated
            if (raw.Contains(","))
            {
                foreach (var part in raw.Split(','))
                {
                    var val = part.Trim().Trim('"').Trim('[', ']');
                    if (!string.IsNullOrEmpty(val)) yield return val;
                }
                yield break;
            }

            // plain single value (could be quoted)
            yield return raw.Trim().Trim('"').Trim('[', ']');
        }

        private IEnumerable<string> GetRolesFromCognito(ClaimsPrincipal user)
        {
            try
            {
                var rawGroupClaims = user.FindAll("cognito:groups").Select(x => x.Value);

                var roles = new List<string>();

                foreach (var claimValue in rawGroupClaims)
                {
                    if (claimValue.TrimStart().StartsWith("["))
                    {
                        // Parse JSON array into a list
                        var parsed = System.Text.Json.JsonSerializer.Deserialize<List<string>>(claimValue);
                        if (parsed != null) roles.AddRange(parsed);
                    }
                    else
                    {
                        roles.Add(claimValue);
                    }
                }

                return roles;
            }
            catch
            {
                return Enumerable.Empty<string>();
            }
        }


        // Public Roles / helpers
        public IEnumerable<string> Roles => GetRolesFromCognito(_user);

        public async Task<IEnumerable<string>> GetRolesAsync()
        {
            var state = await _authProvider.GetAuthenticationStateAsync().ConfigureAwait(false);
            return GetRolesFromCognito(state.User ?? new ClaimsPrincipal(new ClaimsIdentity()));
        }

        public bool IsInRole(string role)
        {
            if (string.IsNullOrWhiteSpace(role)) return false;
            if (!IsAuthenticated) return false;

            return Roles.Any(r => string.Equals(r, role, StringComparison.OrdinalIgnoreCase));
        }

        public async Task<bool> IsInRoleAsync(string role)
        {
            if (string.IsNullOrWhiteSpace(role)) return false;
            var state = await _authProvider.GetAuthenticationStateAsync().ConfigureAwait(false);
            var user = state.User;
            if (!(user?.Identity?.IsAuthenticated ?? false)) return false;

            return GetRolesFromCognito(user).Any(r => string.Equals(r, role, StringComparison.OrdinalIgnoreCase));
        }

        public bool IsSuperAdmin => IsInRole("SuperAdmin");
        public Task<bool> IsSuperAdminAsync() => IsInRoleAsync("SuperAdmin");

        // dispose subscription
        public void Dispose()
        {
            if (_disposed) return;
            _authProvider.AuthenticationStateChanged -= HandleAuthStateChanged;
            _disposed = true;
        }
    }

}
