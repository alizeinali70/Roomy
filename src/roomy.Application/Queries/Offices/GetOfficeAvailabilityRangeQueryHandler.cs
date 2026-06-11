using MediatR;
using roomy.Application.DTOs;
using roomy.Domain.Interfaces;

namespace roomy.Application.Queries.Offices
{
    public class GetOfficeAvailabilityRangeQueryHandler : IRequestHandler<GetOfficeAvailabilityRangeQuery, OfficeAvailabilityRangeDto>
    {
        private readonly IOfficeRepository _officeRepository;

        public GetOfficeAvailabilityRangeQueryHandler(IOfficeRepository officeRepository)
        {
            _officeRepository = officeRepository;
        }

        public async Task<OfficeAvailabilityRangeDto> Handle(GetOfficeAvailabilityRangeQuery request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);

            if (request.FromDate > request.ToDate)
                throw new InvalidOperationException("FromDate cannot be after ToDate");

            var office = await _officeRepository.GetByIdWithBookingsAsync(request.OfficeId, cancellationToken);
            if (office == null)
                throw new KeyNotFoundException($"Office {request.OfficeId} not found");

            var entries = new List<OfficeAvailabilityEntryDto>();
            for (var date = request.FromDate; date <= request.ToDate; date = date.AddDays(1))
            {
                var occupied = office.GetOccupancyForDate(date);
                entries.Add(new OfficeAvailabilityEntryDto
                {
                    Date = date,
                    OccupiedWorkspaces = occupied,
                    AvailableWorkspaces = office.TotalWorkspaces - occupied
                });
            }

            return new OfficeAvailabilityRangeDto
            {
                Id = office.Id,
                Name = office.Name,
                Location = office.Location,
                TotalWorkspaces = office.TotalWorkspaces,
                FromDate = request.FromDate,
                ToDate = request.ToDate,
                Entries = entries
            };
        }
    }
}
