using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using aspcts_backend.Services.Interfaces;
using aspcts_backend.Helpers;

namespace aspcts_backend.Middleware
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;

        public JwtMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, IAuthService authService)
        {
            var token = context.Request.Headers["Authorization"]
                .FirstOrDefault()?.Split(" ").Last();

            if (token != null)
            {
                await AttachUserToContext(context, authService, token);
            }

            await _next(context);
        }

        private async Task AttachUserToContext(HttpContext context, IAuthService authService, string token)
        {
            try
            {
                var isValid = await authService.ValidateTokenAsync(token);
                if (isValid)
                {
                    var userId = JwtHelper.GetUserIdFromToken(token);
                    var role = JwtHelper.GetRoleFromToken(token);
                    var email = JwtHelper.GetEmailFromToken(token);

                    // Add user info to context for easy access in controllers
                    context.Items["UserId"] = userId;
                    context.Items["UserRole"] = role;
                    context.Items["UserEmail"] = email;
                }
            }
            catch
            {
                // Do nothing if JWT validation fails
                // User will be unauthorized for protected routes
            }
        }
    }
}