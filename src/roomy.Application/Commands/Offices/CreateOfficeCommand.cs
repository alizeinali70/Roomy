using MediatR;
using roomy.Application.DTOs;

namespace roomy.Application.Commands.Offices
{
    public record CreateOfficeCommand(string Name, string Location, int TotalWorkspaces)
     : IRequest<OfficeDto>;
}
