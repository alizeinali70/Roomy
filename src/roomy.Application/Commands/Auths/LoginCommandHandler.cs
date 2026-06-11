using MediatR;
using roomy.Application.DTOs;
using roomy.Domain.Interfaces;
using System.Security.Cryptography;
using System.Text;

namespace roomy.Application.Commands.Auths
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResponseDto>
    {
        private readonly IUserRepository _userRepository;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;


        public LoginCommandHandler(IUserRepository userRepository, IJwtTokenGenerator jwtTokenGenerator)
        {
            _userRepository = userRepository;
            _jwtTokenGenerator = jwtTokenGenerator;
        }

        public async Task<LoginResponseDto> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);

            var user = await _userRepository.GetByEmailAsync(request.Email, cancellationToken).ConfigureAwait(false);

            if (user == null)
                throw new UnauthorizedAccessException(nameof(user));

            if (!VerifyPassword(request.Password, user.PasswordHash))
                throw new UnauthorizedAccessException("Invalid password.");

            var token = _jwtTokenGenerator.GenerateToken(user.Id, user.Email, user.Role);
            return new LoginResponseDto
            {
                UserId = user.Id,
                Email = user.Email,
                Role = user.Role,
                AccessToken = token
            };
        }


        private static bool VerifyPassword(string password, string passwordHash)
        {
            var hashOfInput = HashPassword(password);
            return hashOfInput == passwordHash;
        }

        private static string HashPassword(string password)
        {
            var hashedBytes = SHA256.HashData(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }
    }
}