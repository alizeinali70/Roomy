using roomy.Domain.Entities;

namespace roomy.Application.DTOs
{
    public class LoginResponseDto
    {
        public Guid UserId { get; set; }
        public string Email { get; set; } = string.Empty;
        public Role Role { get; set; }
        public string AccessToken { get; set; } = string.Empty;
    }
}
