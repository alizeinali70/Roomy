using roomy.Domain.Entities;

namespace roomy.Application.DTOs
{
    public class CreateUserDto
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public Role Role { get; set; } = Role.Employee;
    }
}
