using MediatR;
using roomy.Application.DTOs;
using roomy.Domain.Entities;
using roomy.Domain.Interfaces;

namespace roomy.Application.Commands.Offices
{
    public class CreateOfficeCommandHandler : IRequestHandler<CreateOfficeCommand, OfficeDto>
    {
        private readonly IOfficeRepository _officeRepository;
        private readonly IAuthorizationService _authorizationService;

        public CreateOfficeCommandHandler(IOfficeRepository officeRepository, IAuthorizationService authorizationService)
        {
            _officeRepository = officeRepository;
            _authorizationService = authorizationService;
        }

        public async Task<OfficeDto> Handle(CreateOfficeCommand request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);

            if (!_authorizationService.IsAdmin())
                throw new UnauthorizedAccessException("Only administrators can create offices");

            var office = new Office
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Location = request.Location,
                TotalWorkspaces = request.TotalWorkspaces
            };

            await _officeRepository.AddAsync(office, cancellationToken);

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
