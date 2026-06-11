using roomy.Domain.Entities;

namespace roomy.Domain.Interfaces
{
    public interface IJwtTokenGenerator
    {
        string GenerateToken(Guid userId, string email, Role role);
    }
}
