using Moq;
using roomy.Application.Queries.Offices;
using roomy.Domain.Entities;
using roomy.Domain.Interfaces;

namespace roomy.Tests.Queries
{
    public class GetOfficeAvailabilityQueryHandlerTests
    {
        [Fact]
        public async Task HandleWhenOfficeExistsReturnsAvailabilityDto()
        {
            // Arrange
            var officeId = Guid.NewGuid();
            var date = new DateOnly(2026, 1, 15);

            var office = new Office
            {
                Id = officeId,
                Name = "HQ",
                Location = "NYC",
                TotalWorkspaces = 10,
                Bookings =
                {
                    new Booking { Id = Guid.NewGuid(), OfficeId = officeId, UserId = Guid.NewGuid(), Date = date.ToDateTime(TimeOnly.MinValue) },
                    new Booking { Id = Guid.NewGuid(), OfficeId = officeId, UserId = Guid.NewGuid(), Date = date.ToDateTime(TimeOnly.MinValue) },
                    new Booking { Id = Guid.NewGuid(), OfficeId = officeId, UserId = Guid.NewGuid(), Date = date.ToDateTime(TimeOnly.MinValue), CancelledAt = DateTime.UtcNow }
                }
            };

            var officeRepositoryMock = new Mock<IOfficeRepository>();
            officeRepositoryMock
                .Setup(x => x.GetByIdWithBookingsAsync(officeId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(office);

            var sut = new GetOfficeAvailabilityQueryHandler(officeRepositoryMock.Object);
            var query = new GetOfficeAvailabilityQuery(officeId, date);

            // Act
            var result = await sut.Handle(query, CancellationToken.None);

            // Assert
            Assert.Equal(officeId, result.Id);
            Assert.Equal("HQ", result.Name);
            Assert.Equal("NYC", result.Location);
            Assert.Equal(10, result.TotalWorkspaces);
            Assert.Equal(2, result.OccupiedWorkspaces);
            Assert.Equal(8, result.AvailableWorkspaces);
            Assert.Equal(date, result.Date);
        }
    }
}
