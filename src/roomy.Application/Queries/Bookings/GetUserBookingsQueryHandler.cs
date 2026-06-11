using MediatR;
using roomy.Application.DTOs;
using roomy.Application.Interfaces;

namespace roomy.Application.Queries.Bookings
{
    public class GetUserBookingsQueryHandler : IRequestHandler<GetUserBookingsQuery, List<BookingDto>>
    {
        private readonly IBookingRepository _bookingRepository;

        public GetUserBookingsQueryHandler(IBookingRepository bookingRepository)
        {
            _bookingRepository = bookingRepository;
        }

        public async Task<List<BookingDto>> Handle(GetUserBookingsQuery request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);

            var bookings = await _bookingRepository.GetByUserIdAsync(request.UserId, cancellationToken);

            return bookings
                .Where(b => b.IsActive)
                .Select(b => new BookingDto
                {
                    Id = b.Id,
                    OfficeId = b.OfficeId,
                    OfficeName = b.Office.Name,
                    Date = b.Date,
                    CreatedAt = b.CreatedAt,
                    UpdatedAt = b.UpdatedAt
                })
                .ToList();
        }
    }
}
