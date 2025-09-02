using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace aspcts_backend.Helpers
{
    public class JwtHelper
    {
        public static Guid GetUserIdFromToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jsonToken = tokenHandler.ReadJwtToken(token);

            var userIdClaim = jsonToken.Claims.FirstOrDefault(x => x.Type == "userId");
            if (userIdClaim != null && Guid.TryParse(userIdClaim.Value, out var userId))
            {
                return userId;
            }

            throw new UnauthorizedAccessException("Token inválido");
        }

        public static string GetRoleFromToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jsonToken = tokenHandler.ReadJwtToken(token);

            var roleClaim = jsonToken.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Role);
            return roleClaim?.Value ?? throw new UnauthorizedAccessException("Token inválido");
        }

        public static string GetEmailFromToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jsonToken = tokenHandler.ReadJwtToken(token);

            var emailClaim = jsonToken.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email);
            return emailClaim?.Value ?? throw new UnauthorizedAccessException("Token inválido");
        }
    }
}