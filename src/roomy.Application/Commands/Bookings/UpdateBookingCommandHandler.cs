using MediatR;
using roomy.Application.DTOs;
using roomy.Application.Interfaces;
using roomy.Domain.Interfaces;

namespace roomy.Application.Commands.Bookings
{
    public class UpdateBookingCommandHandler : IRequestHandler<UpdateBookingCommand, BookingDto>
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly IOfficeRepository _officeRepository;
        private readonly IAuthorizationService _authorizationService;

        public UpdateBookingCommandHandler(IBookingRepository bookingRepository, IOfficeRepository officeRepository, IAuthorizationService authorizationService)
        {
            _bookingRepository = bookingRepository;
            _officeRepository = officeRepository;
            _authorizationService = authorizationService;
        }

        public async Task<BookingDto> Handle(UpdateBookingCommand request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);

            var booking = await _bookingRepository.GetByIdWithOfficeAsync(request.BookingId, cancellationToken);
            if (booking == null || booking.CancelledAt.HasValue)
                throw new KeyNotFoundException($"Booking {request.BookingId} not found");

            if (!_authorizationService.IsAdmin() && !_authorizationService.IsOwnerOf(booking.UserId))
                throw new UnauthorizedAccessException("You are not allowed to update this booking");

            var office = await _officeRepository.GetByIdWithBookingsAsync(request.OfficeId, cancellationToken);
            if (office == null)
                throw new KeyNotFoundException($"Office {request.OfficeId} not found");

            var bookingDate = DateOnly.FromDateTime(request.Date);
            var activeBookings = office.Bookings
                .Where(b => !b.CancelledAt.HasValue && DateOnly.FromDateTime(b.Date) == bookingDate && b.Id != booking.Id)
                .ToList();

            if (activeBookings.Any(b => b.UserId == booking.UserId))
                throw new InvalidOperationException("The user already has an active booking for this office on the selected date");

            var occupied = activeBookings
                .Select(b => b.UserId)
                .Distinct()
                .Count();

            if (occupied >= office.TotalWorkspaces)
                throw new InvalidOperationException("No available workspaces for this date");

            booking.OfficeId = office.Id;
            booking.Office = office;
            booking.Date = request.Date;
            booking.UpdatedAt = DateTime.UtcNow;

            await _bookingRepository.UpdateAsync(booking, cancellationToken);

            return new BookingDto
            {
                Id = booking.Id,
                OfficeId = booking.OfficeId,
                OfficeName = office.Name,
                Date = booking.Date,
                CreatedAt = booking.CreatedAt,
                UpdatedAt = booking.UpdatedAt
            };
        }
    }
}
