using Moq;
using roomy.Application.Commands.Users;
using roomy.Domain.Entities;
using roomy.Domain.Interfaces;

namespace roomy.Tests.Commands
{
    public class CreateUserCommandHandlerTests
    {
        [Fact]
        public async Task HandleWhenUserIsAdminCreatesEmployeeAndReturnsDto()
        {
            var userRepositoryMock = new Mock<IUserRepository>();
            var authorizationServiceMock = new Mock<IAuthorizationService>();

            authorizationServiceMock.Setup(x => x.IsAdmin()).Returns(true);
            userRepositoryMock.Setup(x => x.ExistsByEmailAsync("employee@roomy.local", It.IsAny<CancellationToken>())).ReturnsAsync(false);

            var sut = new CreateUserCommandHandler(userRepositoryMock.Object, authorizationServiceMock.Object);
            var command = new CreateUserCommand("employee@roomy.local", "pass123", Role.Employee);

            var result = await sut.Handle(command, CancellationToken.None);

            Assert.Equal("employee@roomy.local", result.Email);
            Assert.Equal(Role.Employee, result.Role);

            userRepositoryMock.Verify(x => x.AddAsync(It.Is<User>(u => u.Email == "employee@roomy.local" && u.Role == Role.Employee), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
