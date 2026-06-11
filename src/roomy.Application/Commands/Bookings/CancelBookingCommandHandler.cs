using MediatR;
using roomy.Application.Interfaces;
using roomy.Domain.Interfaces;

namespace roomy.Application.Commands.Bookings
{
    public class CancelBookingCommandHandler : IRequestHandler<CancelBookingCommand>
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly IAuthorizationService _authorizationService;

        public CancelBookingCommandHandler(IBookingRepository bookingRepository, IAuthorizationService authorizationService)
        {
            _bookingRepository = bookingRepository;
            _authorizationService = authorizationService;
        }

        public async Task Handle(CancelBookingCommand request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);

            var booking = await _bookingRepository.GetByIdWithOfficeAsync(request.BookingId, cancellationToken);
            if (booking == null || booking.CancelledAt.HasValue)
                throw new KeyNotFoundException($"Booking {request.BookingId} not found");

            if (!_authorizationService.IsAdmin() && !_authorizationService.IsOwnerOf(booking.UserId))
                throw new UnauthorizedAccessException("You are not allowed to cancel this booking");

            booking.CancelledAt = DateTime.UtcNow;
            booking.UpdatedAt = booking.CancelledAt;

            await _bookingRepository.UpdateAsync(booking, cancellationToken);
        }
    }
}
