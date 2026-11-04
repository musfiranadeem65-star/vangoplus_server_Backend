using System.Security.Claims;

namespace vangoplus_server.Application.Auth
{
    public static class ClaimsPrincipalExtensions
    {
        /// <summary>
        /// The id of the signed-in user, taken from the token rather than from
        /// anything the caller put in the request. Returns null when the
        /// request is not signed in.
        /// </summary>
        public static int? UserId(this ClaimsPrincipal principal)
        {
            var raw =
                principal.FindFirstValue(ClaimTypes.NameIdentifier) ??
                principal.FindFirstValue("sub");

            return int.TryParse(raw, out var id) ? id : null;
        }

        public static bool IsAdmin(this ClaimsPrincipal principal) =>
            principal.IsInRole("Admin");
    }
}
