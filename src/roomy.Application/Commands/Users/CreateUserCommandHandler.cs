using MediatR;
using roomy.Application.DTOs;
using roomy.Domain.Entities;
using roomy.Domain.Interfaces;
using System.Security.Cryptography;
using System.Text;

namespace roomy.Application.Commands.Users
{
    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, UserDto>
    {
        private readonly IUserRepository _userRepository;
        private readonly IAuthorizationService _authorizationService;

        public CreateUserCommandHandler(IUserRepository userRepository, IAuthorizationService authorizationService)
        {
            _userRepository = userRepository;
            _authorizationService = authorizationService;
        }

        public async Task<UserDto> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);

            if (!_authorizationService.IsAdmin())
                throw new UnauthorizedAccessException("Only administrators can create users");

            if (await _userRepository.ExistsByEmailAsync(request.Email, cancellationToken))
                throw new InvalidOperationException($"User with email {request.Email} already exists");

            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = request.Email,
                PasswordHash = HashPassword(request.Password),
                Role = request.Role
            };

            await _userRepository.AddAsync(user, cancellationToken);

            return new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                Role = user.Role,
                CreatedAt = user.CreatedAt
            };
        }

        private static string HashPassword(string password)
        {
            var hashedBytes = SHA256.HashData(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }
    }
}
