using Microsoft.EntityFrameworkCore;
using roomy.Application.Interfaces;
using roomy.Domain.Entities;
using roomy.Infrastructure.Persistence;

namespace roomy.Infrastructure.Repositories
{
    public class BookingRepository : GenericRepository<Booking>, IBookingRepository
    {
        public BookingRepository(RoomyDbContext context) : base(context) { }

        public async Task<List<Booking>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Where(b => b.UserId == userId)
                .Include(b => b.Office)
                .OrderByDescending(b => b.Date)
                .ToListAsync(cancellationToken);
        }

        public Task<Booking?> GetByIdWithOfficeAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return _dbSet
                .Include(b => b.Office)
                .FirstOrDefaultAsync(b => b.Id == id, cancellationToken);
        }
    }
}
