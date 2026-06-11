using MediatR;
using roomy.Application.DTOs;

namespace roomy.Application.Commands.Auths
{
    public record LoginCommand(string Email, string Password) : IRequest<LoginResponseDto>;

}
