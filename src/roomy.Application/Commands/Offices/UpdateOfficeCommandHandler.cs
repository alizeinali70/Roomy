using MediatR;
using roomy.Application.DTOs;
using roomy.Domain.Interfaces;

namespace roomy.Application.Commands.Offices
{
    public class UpdateOfficeCommandHandler : IRequestHandler<UpdateOfficeCommand, OfficeDto>
    {
        private readonly IOfficeRepository _officeRepository;
        private readonly IAuthorizationService _authorizationService;

        public UpdateOfficeCommandHandler(IOfficeRepository officeRepository, IAuthorizationService authorizationService)
        {
            _officeRepository = officeRepository;
            _authorizationService = authorizationService;
        }

        public async Task<OfficeDto> Handle(UpdateOfficeCommand request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);

            if (!_authorizationService.IsAdmin())
                throw new UnauthorizedAccessException("Only administrators can update offices");

            var office = await _officeRepository.GetByIdWithBookingsAsync(request.OfficeId, cancellationToken);
            if (office == null)
                throw new KeyNotFoundException($"Office {request.OfficeId} not found");

            var currentOccupancy = office.Bookings
                .Where(b => !b.CancelledAt.HasValue)
                .Select(b => new { b.UserId, Date = DateOnly.FromDateTime(b.Date) })
                .Distinct()
                .GroupBy(x => x.Date)
                .Select(x => x.Count())
                .DefaultIfEmpty(0)
                .Max();

            if (request.TotalWorkspaces < currentOccupancy)
                throw new InvalidOperationException("Total workspaces cannot be lower than the current occupancy");

            office.Name = request.Name;
            office.Location = request.Location;
            office.TotalWorkspaces = request.TotalWorkspaces;
            office.UpdatedAt = DateTime.UtcNow;

            await _officeRepository.UpdateAsync(office, cancellationToken);

            return new OfficeDto
            {
                Id = office.Id,
                Name = office.Name,
                Location = office.Location,
                TotalWorkspaces = office.TotalWorkspaces,
                CreatedAt = office.CreatedAt,
                UpdatedAt = office.UpdatedAt
            };
        }
    }
}
