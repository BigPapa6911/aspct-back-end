using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;

namespace aspcts_backend.Helpers
{
    public static class Extensions
    {
        public static Guid GetUserId(this ClaimsPrincipal user)
        {
            var userIdClaim = user.FindFirst("userId");
            if (userIdClaim != null && Guid.TryParse(userIdClaim.Value, out var userId))
            {
                return userId;
            }

            throw new UnauthorizedAccessException("Token inválido");
        }

        public static string GetUserRole(this ClaimsPrincipal user)
        {
            var roleClaim = user.FindFirst(ClaimTypes.Role);
            return roleClaim?.Value ?? throw new UnauthorizedAccessException("Token inválido");
        }

        public static string GetUserEmail(this ClaimsPrincipal user)
        {
            var emailClaim = user.FindFirst(ClaimTypes.Email);
            return emailClaim?.Value ?? throw new UnauthorizedAccessException("Token inválido");
        }

        public static bool IsInRole(this ClaimsPrincipal user, params string[] roles)
        {
            var userRole = user.GetUserRole();
            return roles.Contains(userRole);
        }
    }
}