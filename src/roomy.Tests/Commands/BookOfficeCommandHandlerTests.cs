using Moq;
using roomy.Application.Commands.Bookings;
using roomy.Application.Interfaces;
using roomy.Domain.Entities;
using roomy.Domain.Interfaces;

namespace roomy.Tests.Commands
{
    public class BookOfficeCommandHandlerTests
    {
        [Fact]
        public async Task HandleWhenOfficeExistsAndHasAvailabilityReturnsBookingDto()
        {
            var officeId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var bookingDate = DateTime.UtcNow.Date.AddDays(1);

            var office = new Office
            {
                Id = officeId,
                Name = "HQ",
                TotalWorkspaces = 10
            };

            var bookingRepositoryMock = new Mock<IBookingRepository>();
            var officeRepositoryMock = new Mock<IOfficeRepository>();

            officeRepositoryMock
                .Setup(x => x.GetByIdWithBookingsAsync(officeId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(office);

            var sut = new BookOfficeCommandHandler(bookingRepositoryMock.Object, officeRepositoryMock.Object);
            var command = new BookOfficeCommand(officeId, bookingDate, userId);

            var result = await sut.Handle(command, CancellationToken.None);

            Assert.Equal(officeId, result.OfficeId);
            Assert.Equal("HQ", result.OfficeName);
            Assert.Equal(bookingDate, result.Date);

            bookingRepositoryMock.Verify(
                x => x.AddAsync(It.Is<Booking>(b => b.OfficeId == officeId && b.UserId == userId), It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task HandleWhenOfficeIsFullThrowsInvalidOperationException()
        {
            var officeId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var bookingDate = new DateTime(2026, 1, 15);

            var office = new Office
            {
                Id = officeId,
                Name = "HQ",
                TotalWorkspaces = 1,
                Bookings =
                {
                    new Booking
                    {
                        Id = Guid.NewGuid(),
                        OfficeId = officeId,
                        UserId = Guid.NewGuid(),
                        Date = bookingDate
                    }
                }
            };

            var bookingRepositoryMock = new Mock<IBookingRepository>();
            var officeRepositoryMock = new Mock<IOfficeRepository>();

            officeRepositoryMock
                .Setup(x => x.GetByIdWithBookingsAsync(officeId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(office);

            var sut = new BookOfficeCommandHandler(bookingRepositoryMock.Object, officeRepositoryMock.Object);

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                sut.Handle(new BookOfficeCommand(officeId, bookingDate, userId), CancellationToken.None));
        }
    }
}
