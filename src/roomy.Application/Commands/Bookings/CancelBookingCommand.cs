using MediatR;

namespace roomy.Application.Commands.Bookings
{
    public record CancelBookingCommand(Guid BookingId) : IRequest;
}
