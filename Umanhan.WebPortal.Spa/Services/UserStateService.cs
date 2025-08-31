using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;

namespace Umanhan.WebPortal.Spa.Services
{
    //public class UserStateService
    //{
    //    private readonly AuthenticationStateProvider? _authProvider;
    //    private readonly IHttpContextAccessor? _httpContextAccessor;

    //    public UserStateService(AuthenticationStateProvider authProvider)
    //    {
    //        _authProvider = authProvider;
    //    }

    //    public UserStateService(IHttpContextAccessor httpContextAccessor)
    //    {
    //        _httpContextAccessor = httpContextAccessor;
    //    }

    //    private ClaimsPrincipal GetCurrentUserSync()
    //    {
    //        if (_httpContextAccessor?.HttpContext?.User != null)
    //            return _httpContextAccessor.HttpContext.User;

    //        // Fallback if running in Blazor Server/WASM without HttpContext
    //        return new ClaimsPrincipal(new ClaimsIdentity());
    //    }

    //    private async Task<ClaimsPrincipal> GetCurrentUserAsync()
    //    {
    //        if (_authProvider != null)
    //        {
    //            var authState = await _authProvider.GetAuthenticationStateAsync();
    //            return authState.User;
    //        }

    //        // Fallback for Minimal API sync context
    //        return GetCurrentUserSync();
    //    }

    //    private IEnumerable<string> GetRolesFromCognito(ClaimsPrincipal user)
    //    {
    //        try
    //        {
    //            var list = user.FindAll("cognito:groups")
    //                       .Select(x => x.Value)
    //                       .ToList();
    //            Console.WriteLine($"UserStateService:GetRolesFromCognito:list: {string.Join(",", list)}");
    //            return list;
    //        }
    //        catch
    //        {
    //            return Enumerable.Empty<string>();
    //        }
    //    }

    //    public async Task<bool> IsInRoleAsync(string role)
    //    {
    //        var user = await GetCurrentUserAsync();
    //        if (!user.Identity?.IsAuthenticated ?? false) return false;

    //        var roles = GetRolesFromCognito(user);
    //        return roles.Any(c => c.Equals(role, StringComparison.OrdinalIgnoreCase));
    //    }

    //    public bool IsInRole(string role)
    //    {
    //        var user = GetCurrentUserSync();
    //        if (!user.Identity?.IsAuthenticated ?? false) return false;

    //        var roles = GetRolesFromCognito(user);
    //        return roles.Any(c => c.Equals(role, StringComparison.OrdinalIgnoreCase));
    //    }

    //    public async Task<bool> IsSuperAdminAsync() => await IsInRoleAsync("SuperAdmin");

    //    public bool IsSuperAdmin => IsInRole("SuperAdmin");

    //    public async Task<IEnumerable<string>> GetRolesAsync()
    //    {
    //        var user = await GetCurrentUserAsync();
    //        return GetRolesFromCognito(user);
    //    }

    //    public IEnumerable<string> Roles => GetRolesFromCognito(GetCurrentUserSync());

    //    public bool IsAuthenticated => GetCurrentUserSync().Identity?.IsAuthenticated ?? false;
    //}
    //public class UserStateService
    //{
    //    private readonly AuthenticationStateProvider _authProvider;
    //    private ClaimsPrincipal _user = new ClaimsPrincipal(new ClaimsIdentity());
    //    private TaskCompletionSource<bool> _readyTcs = new();

    //    public bool IsReady { get; private set; }
    //    public bool IsAuthenticated => _user.Identity?.IsAuthenticated ?? false;
    //    public bool IsAdmin => IsInRole("Admin");
    //    public bool IsSuperAdmin => IsInRole("SuperAdmin");
    //    public IEnumerable<string> Roles => GetRolesFromCognito(_user);

    //    public Task WhenReady => _readyTcs.Task;

    //    public UserStateService(AuthenticationStateProvider authProvider)
    //    {
    //        _authProvider = authProvider;
    //        _authProvider.AuthenticationStateChanged += OnAuthStateChanged;
    //        _ = InitializeAsync(); // fire-and-forget initialization
    //    }

    //    private async Task InitializeAsync()
    //    {
    //        var authState = await _authProvider.GetAuthenticationStateAsync();
    //        _user = authState.User;
    //        SetReady();
    //    }

    //    private async void OnAuthStateChanged(Task<AuthenticationState> task)
    //    {
    //        var authState = await task;
    //        _user = authState.User;
    //        SetReady();
    //    }

    //    private void SetReady()
    //    {
    //        if (!IsReady)
    //        {
    //            IsReady = true;
    //            _readyTcs.TrySetResult(true);
    //        }
    //    }

    //    private IEnumerable<string> GetRolesFromCognito(ClaimsPrincipal user)
    //    {
    //        try
    //        {
    //            return user.FindAll("cognito:groups")
    //                       .Select(x => x.Value)
    //                       .ToList();
    //        }
    //        catch
    //        {
    //            return Enumerable.Empty<string>();
    //        }
    //    }

    //    public bool IsInRole(string role)
    //    {
    //        if (string.IsNullOrWhiteSpace(role)) return false;
    //        if (!IsAuthenticated) return false;

    //        var roles = GetRolesFromCognito(_user);
    //        return roles.Any(c => c.Equals(role, StringComparison.OrdinalIgnoreCase));
    //    }
    //}
    using System.Security.Claims;
    using System.Text.Json;

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
