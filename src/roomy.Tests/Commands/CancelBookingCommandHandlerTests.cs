using Moq;
using roomy.Application.Commands.Bookings;
using roomy.Application.Interfaces;
using roomy.Domain.Entities;
using roomy.Domain.Interfaces;

namespace roomy.Tests.Commands
{
    public class CancelBookingCommandHandlerTests
    {
        [Fact]
        public async Task HandleWhenEmployeeOwnsBookingCancelsBooking()
        {
            var bookingId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            var bookingRepositoryMock = new Mock<IBookingRepository>();
            var authorizationServiceMock = new Mock<IAuthorizationService>();

            var booking = new Booking
            {
                Id = bookingId,
                UserId = userId,
                Office = new Office { Id = Guid.NewGuid(), Name = "HQ" }
            };

            bookingRepositoryMock.Setup(x => x.GetByIdWithOfficeAsync(bookingId, It.IsAny<CancellationToken>())).ReturnsAsync(booking);
            authorizationServiceMock.Setup(x => x.IsAdmin()).Returns(false);
            authorizationServiceMock.Setup(x => x.IsOwnerOf(userId)).Returns(true);

            var sut = new CancelBookingCommandHandler(bookingRepositoryMock.Object, authorizationServiceMock.Object);

            await sut.Handle(new CancelBookingCommand(bookingId), CancellationToken.None);

            Assert.True(booking.CancelledAt.HasValue);
            bookingRepositoryMock.Verify(x => x.UpdateAsync(It.Is<Booking>(b => b.Id == bookingId && b.CancelledAt.HasValue), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
