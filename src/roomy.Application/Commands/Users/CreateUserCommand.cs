using MediatR;
using roomy.Application.DTOs;
using roomy.Domain.Entities;

namespace roomy.Application.Commands.Users
{
    public record CreateUserCommand(string Email, string Password, Role Role) : IRequest<UserDto>;
}
