using MediatR;
using roomy.Application.DTOs;

namespace roomy.Application.Commands.Bookings
{
    public record UpdateBookingCommand(Guid BookingId, Guid OfficeId, DateTime Date) : IRequest<BookingDto>;
}
