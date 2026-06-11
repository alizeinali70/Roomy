using MediatR;
using roomy.Application.DTOs;

namespace roomy.Application.Commands.Bookings
{
    public record BookOfficeCommand(Guid OfficeId, DateTime BookingDate, Guid UserId)
    : IRequest<BookingDto>;
}
