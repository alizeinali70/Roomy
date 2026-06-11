using MediatR;
using roomy.Application.DTOs;

namespace roomy.Application.Queries.Offices
{
    public record GetOfficeAvailabilityRangeQuery(Guid OfficeId, DateOnly FromDate, DateOnly ToDate) : IRequest<OfficeAvailabilityRangeDto>;
}
