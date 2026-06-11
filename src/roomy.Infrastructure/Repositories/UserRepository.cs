using Microsoft.EntityFrameworkCore;
using roomy.Domain.Entities;
using roomy.Domain.Interfaces;
using roomy.Infrastructure.Persistence;

namespace roomy.Infrastructure.Repositories
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        public UserRepository(RoomyDbContext context) : base(context)
        {
        }

        public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
        {
            return await _dbSet.FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
        }

        public Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default)
        {
            return _dbSet.AnyAsync(u => u.Email == email, cancellationToken);
        }
    }
}
