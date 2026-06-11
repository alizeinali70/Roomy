using Microsoft.AspNetCore.Http;
using roomy.Domain.Interfaces;
using System.Security.Claims;

namespace roomy.Infrastructure.Auth
{
    public class AuthorizationService : IAuthorizationService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthorizationService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public Guid GetCurrentUserId()
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
                throw new UnauthorizedAccessException("User not authenticated");
            return userId;
        }

        public bool IsAdmin()
        {
            var roleClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.Role)?.Value;
            return roleClaim == "Admin";
        }

        public bool IsOwnerOf(Guid resourceOwnerId)
        {
            return GetCurrentUserId() == resourceOwnerId;
        }
    }
}
