using roomy.Domain.Entities;

namespace roomy.Domain.Interfaces
{
    public interface IOfficeRepository : IRepository<Office>
    {
        Task<Office?> GetByIdWithBookingsAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
