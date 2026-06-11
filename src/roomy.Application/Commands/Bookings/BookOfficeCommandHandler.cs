using MediatR;
using roomy.Application.DTOs;
using roomy.Application.Interfaces;
using roomy.Domain.Entities;
using roomy.Domain.Interfaces;

namespace roomy.Application.Commands.Bookings
{
    public class BookOfficeCommandHandler : IRequestHandler<BookOfficeCommand, BookingDto>
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly IOfficeRepository _officeRepository;

        public BookOfficeCommandHandler(IBookingRepository bookingRepository, IOfficeRepository officeRepository)
        {
            _bookingRepository = bookingRepository;
            _officeRepository = officeRepository;
        }

        public async Task<BookingDto> Handle(BookOfficeCommand request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);

            var office = await _officeRepository.GetByIdWithBookingsAsync(request.OfficeId, cancellationToken);
            if (office == null)
                throw new KeyNotFoundException($"Office {request.OfficeId} not found");

            var bookingDate = DateOnly.FromDateTime(request.BookingDate);
            var activeBookings = office.Bookings
                .Where(b => !b.CancelledAt.HasValue && DateOnly.FromDateTime(b.Date) == bookingDate)
                .ToList();

            if (activeBookings.Any(b => b.UserId == request.UserId))
                throw new InvalidOperationException("The user already has an active booking for this office on the selected date");

            var occupied = activeBookings
                .Select(b => b.UserId)
                .Distinct()
                .Count();

            if (occupied >= office.TotalWorkspaces)
                throw new InvalidOperationException("No available workspaces for this date");

            var booking = new Booking
            {
                Id = Guid.NewGuid(),
                UserId = request.UserId,
                OfficeId = request.OfficeId,
                Date = request.BookingDate
            };

            await _bookingRepository.AddAsync(booking, cancellationToken);

            return new BookingDto
            {
                Id = booking.Id,
                OfficeId = booking.OfficeId,
                OfficeName = office.Name,
                Date = booking.Date,
                CreatedAt = booking.CreatedAt
            };
        }
    }
}
