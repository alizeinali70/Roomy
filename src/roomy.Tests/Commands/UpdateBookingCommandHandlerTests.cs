using Moq;
using roomy.Application.Commands.Bookings;
using roomy.Application.Interfaces;
using roomy.Domain.Entities;
using roomy.Domain.Interfaces;

namespace roomy.Tests.Commands
{
    public class UpdateBookingCommandHandlerTests
    {
        [Fact]
        public async Task HandleWhenEmployeeOwnsBookingUpdatesBookingAndReturnsDto()
        {
            var bookingId = Guid.NewGuid();
            var officeId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var bookingDate = new DateTime(2026, 1, 20);

            var bookingRepositoryMock = new Mock<IBookingRepository>();
            var officeRepositoryMock = new Mock<IOfficeRepository>();
            var authorizationServiceMock = new Mock<IAuthorizationService>();

            var booking = new Booking
            {
                Id = bookingId,
                OfficeId = officeId,
                UserId = userId,
                Date = new DateTime(2026, 1, 19),
                Office = new Office { Id = officeId, Name = "Old Office", TotalWorkspaces = 5 }
            };

            var office = new Office
            {
                Id = officeId,
                Name = "HQ",
                Location = "Berlin",
                TotalWorkspaces = 5
            };

            bookingRepositoryMock.Setup(x => x.GetByIdWithOfficeAsync(bookingId, It.IsAny<CancellationToken>())).ReturnsAsync(booking);
            officeRepositoryMock.Setup(x => x.GetByIdWithBookingsAsync(officeId, It.IsAny<CancellationToken>())).ReturnsAsync(office);
            authorizationServiceMock.Setup(x => x.IsAdmin()).Returns(false);
            authorizationServiceMock.Setup(x => x.IsOwnerOf(userId)).Returns(true);

            var sut = new UpdateBookingCommandHandler(bookingRepositoryMock.Object, officeRepositoryMock.Object, authorizationServiceMock.Object);
            var command = new UpdateBookingCommand(bookingId, officeId, bookingDate);

            var result = await sut.Handle(command, CancellationToken.None);

            Assert.Equal(officeId, result.OfficeId);
            Assert.Equal("HQ", result.OfficeName);
            Assert.Equal(bookingDate, result.Date);

            bookingRepositoryMock.Verify(x => x.UpdateAsync(It.Is<Booking>(b => b.Id == bookingId && b.Date == bookingDate), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
