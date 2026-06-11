using MediatR;
using roomy.Application.DTOs;

namespace roomy.Application.Queries.Offices
{
    public record GetOfficeAvailabilityQuery(Guid OfficeId, DateOnly Date)
    : IRequest<OfficeAvailabilityDto>;

    public record ListOfficesQuery()
        : IRequest<List<OfficeDto>>;
}
