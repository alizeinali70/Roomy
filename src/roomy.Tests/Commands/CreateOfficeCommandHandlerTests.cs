using Moq;
using roomy.Application.Commands.Offices;
using roomy.Domain.Entities;
using roomy.Domain.Interfaces;

namespace roomy.Tests.Commands
{
    public class CreateOfficeCommandHandlerTests
    {
        [Fact]
        public async Task HandleWhenUserIsAdminCreatesOfficeAndReturnsDto()
        {
            var officeRepositoryMock = new Mock<IOfficeRepository>();
            var authorizationServiceMock = new Mock<IAuthorizationService>();

            authorizationServiceMock.Setup(x => x.IsAdmin()).Returns(true);

            var sut = new CreateOfficeCommandHandler(officeRepositoryMock.Object, authorizationServiceMock.Object);
            var command = new CreateOfficeCommand("Main Office", "NYC", 25);

            var result = await sut.Handle(command, CancellationToken.None);

            Assert.Equal("Main Office", result.Name);
            Assert.Equal("NYC", result.Location);
            Assert.Equal(25, result.TotalWorkspaces);

            officeRepositoryMock.Verify(
                x => x.AddAsync(
                    It.Is<Office>(o => o.Name == "Main Office" && o.Location == "NYC" && o.TotalWorkspaces == 25),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task HandleWhenUserIsNotAdminThrowsUnauthorizedAccessException()
        {
            var officeRepositoryMock = new Mock<IOfficeRepository>();
            var authorizationServiceMock = new Mock<IAuthorizationService>();

            authorizationServiceMock.Setup(x => x.IsAdmin()).Returns(false);

            var sut = new CreateOfficeCommandHandler(officeRepositoryMock.Object, authorizationServiceMock.Object);

            await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
                sut.Handle(new CreateOfficeCommand("Main Office", "NYC", 25), CancellationToken.None));
        }
    }
}
