namespace Umanhan.Weather.Api
{
    public class AuthorizationHelper
    {
        public static bool UserHasRole(HttpContext context, params string[] allowedRoles)
        {
            var rolesClaim = context.User.FindFirst("custom:groups")?.Value;
            var userRoles = context.User.FindFirst("cognito:groups")?.Value?
                                        .Replace("[", "").Replace("]", "")
                                        .Split(' ', StringSplitOptions.RemoveEmptyEntries) ?? new string[] { };
            return userRoles.Any(r => allowedRoles.Contains(r));
        }

        public static bool IsAuthorized(HttpContext context)
        {
            return context.User?.Identity?.IsAuthenticated ?? false;
        }
    }
}
