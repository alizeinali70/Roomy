using MediatR;
using roomy.Application.DTOs;
using roomy.Domain.Interfaces;

namespace roomy.Application.Queries.Offices
{
    public class ListOfficesQueryHandler : IRequestHandler<ListOfficesQuery, List<OfficeDto>>
    {
        private readonly IOfficeRepository _officeRepository;

        public ListOfficesQueryHandler(IOfficeRepository officeRepository)
        {
            _officeRepository = officeRepository;
        }

        public async Task<List<OfficeDto>> Handle(ListOfficesQuery request, CancellationToken cancellationToken)
        {
            var offices = await _officeRepository.GetAllAsync(cancellationToken);
            return offices
                .Select(o => new OfficeDto
                {
                    Id = o.Id,
                    Name = o.Name,
                    Location = o.Location,
                    TotalWorkspaces = o.TotalWorkspaces,
                    CreatedAt = o.CreatedAt,
                    UpdatedAt = o.UpdatedAt
                })
                .ToList();
        }
    }
}
