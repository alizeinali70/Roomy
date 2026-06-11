using Microsoft.EntityFrameworkCore;
using roomy.Domain.Entities;
using roomy.Domain.Interfaces;
using roomy.Infrastructure.Persistence;

namespace roomy.Infrastructure.Repositories
{
    public class OfficeRepository : GenericRepository<Office>, IOfficeRepository
    {
        public OfficeRepository(RoomyDbContext context) : base(context)
        {
        }

        public Task<Office?> GetByIdWithBookingsAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return _dbSet
                .Include(o => o.Bookings)
                .FirstOrDefaultAsync(o => o.Id == id, cancellationToken);
        }
    }
}
