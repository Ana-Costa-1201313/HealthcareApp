using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Backoffice.Domain.Shared;
using Microsoft.AspNetCore.Http;

namespace Backoffice.Domain.Shared
{
    public class AuthService
    {
        private readonly IExternalApiServices _authService;

        public AuthService(IExternalApiServices authService)
        {
            _authService = authService;
        }

        public virtual async Task<bool> IsAuthorized(HttpRequest request, List<string> roles)
        {
            if (!request.Headers.TryGetValue("Authorization", out var tokenHeader))
            {
                throw new UnauthorizedAccessException("Error: Authorization header is missing");
            }

            try
            {
                return await _authService.checkHeader(roles, tokenHeader);
            }
            catch (Exception)
            {
                throw new UnauthorizedAccessException("Error: User not authenticated");
            }
        }

        public virtual async Task<bool> IsAuthorized(HttpRequest request, String email)
        {
            if (!request.Headers.TryGetValue("Authorization", out var tokenHeader))
            {
                throw new UnauthorizedAccessException("Error: Authorization header is missing");
            }

            try
            {
                return await _authService.checkHeaderEmail(email, tokenHeader);
            }
            catch (Exception)
            {
                throw new UnauthorizedAccessException("Error: User not authenticated");
            }
        }
    }
}
