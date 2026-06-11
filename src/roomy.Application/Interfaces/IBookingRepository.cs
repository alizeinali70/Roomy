using roomy.Domain.Entities;
using roomy.Domain.Interfaces;

namespace roomy.Application.Interfaces
{
    public interface IBookingRepository : IRepository<Booking>
    {
        Task<List<Booking>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
        Task<Booking?> GetByIdWithOfficeAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
