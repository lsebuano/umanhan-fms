using System.Security.Claims;

namespace Umanhan.WebPortal.Spa
{
    public static class ClaimsPrincipalExtensions
    {
        // The hierarchy: Read=1, Write=2, Full=3
        private static readonly Dictionary<string, int> _levelRank = new(StringComparer.OrdinalIgnoreCase)
        {
            ["Read"] = 1,
            ["Write"] = 2,
            ["Full"] = 3
        };

        public static bool HasPermission(this ClaimsPrincipal user, string permission)
        {
            if (user == null || string.IsNullOrWhiteSpace(permission))
                return false;

            var parts = permission.Split('.');
            if (parts.Length != 2 || !_levelRank.TryGetValue(parts[1], out int requiredRank))
                return false;

            string requiredModule = parts[0];

            var userPermissions = user.Claims
                                      .Where(c => c.Type == "permission")
                                      .Select(c => c.Value);

            foreach (var perm in userPermissions)
            {
                var parts2 = perm.Split('.', StringSplitOptions.RemoveEmptyEntries);
                if (parts2.Length != 2)
                    continue;

                var module = parts2[0];
                var level = parts2[1];

                if (!string.Equals(module, requiredModule, StringComparison.OrdinalIgnoreCase))
                    continue;

                if (!_levelRank.TryGetValue(level, out var permRank))
                    continue;

                if (permRank >= requiredRank)
                    return true;
            }

            return false;
        }
    }

}
