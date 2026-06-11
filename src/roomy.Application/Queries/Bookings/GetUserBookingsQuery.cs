using MediatR;
using roomy.Application.DTOs;

namespace roomy.Application.Queries.Bookings
{
    public record GetUserBookingsQuery(Guid UserId)
    : IRequest<List<BookingDto>>;
}
