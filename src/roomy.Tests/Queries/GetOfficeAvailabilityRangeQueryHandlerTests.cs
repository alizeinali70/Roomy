using Moq;
using roomy.Application.Queries.Offices;
using roomy.Domain.Entities;
using roomy.Domain.Interfaces;

namespace roomy.Tests.Queries
{
    public class GetOfficeAvailabilityRangeQueryHandlerTests
    {
        [Fact]
        public async Task HandleWhenOfficeExistsReturnsAvailabilityEntriesForRange()
        {
            var officeId = Guid.NewGuid();
            var fromDate = new DateOnly(2026, 1, 15);
            var toDate = new DateOnly(2026, 1, 17);

            var office = new Office
            {
                Id = officeId,
                Name = "HQ",
                Location = "NYC",
                TotalWorkspaces = 5,
                Bookings =
                {
                    new Booking { Id = Guid.NewGuid(), OfficeId = officeId, UserId = Guid.NewGuid(), Date = fromDate.ToDateTime(TimeOnly.MinValue) },
                    new Booking { Id = Guid.NewGuid(), OfficeId = officeId, UserId = Guid.NewGuid(), Date = fromDate.AddDays(1).ToDateTime(TimeOnly.MinValue) }
                }
            };

            var officeRepositoryMock = new Mock<IOfficeRepository>();
            officeRepositoryMock.Setup(x => x.GetByIdWithBookingsAsync(officeId, It.IsAny<CancellationToken>())).ReturnsAsync(office);

            var sut = new GetOfficeAvailabilityRangeQueryHandler(officeRepositoryMock.Object);

            var result = await sut.Handle(new GetOfficeAvailabilityRangeQuery(officeId, fromDate, toDate), CancellationToken.None);

            Assert.Equal(3, result.Entries.Count);
            Assert.Equal(1, result.Entries[0].OccupiedWorkspaces);
            Assert.Equal(1, result.Entries[1].OccupiedWorkspaces);
            Assert.Equal(0, result.Entries[2].OccupiedWorkspaces);
        }
    }
}
