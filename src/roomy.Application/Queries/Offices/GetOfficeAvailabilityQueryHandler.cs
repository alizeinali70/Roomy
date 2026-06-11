using MediatR;
using roomy.Application.DTOs;
using roomy.Domain.Interfaces;

namespace roomy.Application.Queries.Offices
{
    public class GetOfficeAvailabilityQueryHandler : IRequestHandler<GetOfficeAvailabilityQuery, OfficeAvailabilityDto>
    {
        private readonly IOfficeRepository _officeRepository;

        public GetOfficeAvailabilityQueryHandler(IOfficeRepository officeRepository)
        {
            _officeRepository = officeRepository;
        }

        public async Task<OfficeAvailabilityDto> Handle(GetOfficeAvailabilityQuery request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request, nameof(request));

            var office = await _officeRepository.GetByIdWithBookingsAsync(request.OfficeId, cancellationToken);
            if (office == null)
                throw new KeyNotFoundException($"Office {request.OfficeId} not found");

            var occupied = office.GetOccupancyForDate(request.Date);
            var available = office.TotalWorkspaces - occupied;

            return new OfficeAvailabilityDto
            {
                Id = office.Id,
                Name = office.Name,
                Location = office.Location,
                TotalWorkspaces = office.TotalWorkspaces,
                OccupiedWorkspaces = occupied,
                AvailableWorkspaces = available,
                Date = request.Date
            };
        }
    }
}
