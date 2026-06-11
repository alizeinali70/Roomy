using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using roomy.Application.Commands.Bookings;
using roomy.Application.DTOs;
using roomy.Application.Queries.Bookings;
using AppAuthorizationService = roomy.Domain.Interfaces.IAuthorizationService;

namespace roomy.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class BookingsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly AppAuthorizationService _authorizationService;

        public BookingsController(IMediator mediator, AppAuthorizationService authorizationService)
        {
            _mediator = mediator;
            _authorizationService = authorizationService;
        }

        [HttpPost]
        public async Task<ActionResult<BookingDto>> BookOffice([FromBody] CreateBookingDto createBookingDto)
        {
            ArgumentNullException.ThrowIfNull(createBookingDto);

            try
            {
                var currentUserId = _authorizationService.GetCurrentUserId();
                var userId = _authorizationService.IsAdmin() && createBookingDto.UserId is Guid targetUserId
                    ? targetUserId
                    : currentUserId;

                var command = new BookOfficeCommand(createBookingDto.OfficeId, createBookingDto.Date, userId);
                var result = await _mediator.Send(command);
                return CreatedAtAction(nameof(GetUserBookings), new { userId }, result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<BookingDto>> UpdateBooking(Guid id, [FromBody] UpdateBookingDto updateBookingDto)
        {
            ArgumentNullException.ThrowIfNull(updateBookingDto);

            try
            {
                var command = new UpdateBookingCommand(id, updateBookingDto.OfficeId, updateBookingDto.Date);
                var result = await _mediator.Send(command);
                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(StatusCodes.Status403Forbidden, new { message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> CancelBooking(Guid id)
        {
            try
            {
                await _mediator.Send(new CancelBookingCommand(id));
                return NoContent();
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(StatusCodes.Status403Forbidden, new { message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpGet("me")]
        public async Task<ActionResult<List<BookingDto>>> GetMyBookings()
        {
            try
            {
                var userId = _authorizationService.GetCurrentUserId();
                var result = await _mediator.Send(new GetUserBookingsQuery(userId));
                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
        }

        [HttpGet("user/{userId}")]
        public async Task<ActionResult<List<BookingDto>>> GetUserBookings(Guid userId)
        {
            try
            {
                var currentUserId = _authorizationService.GetCurrentUserId();
                var isAdmin = _authorizationService.IsAdmin();

                if (currentUserId != userId && !isAdmin)
                    return Forbid();

                var query = new GetUserBookingsQuery(userId);
                var result = await _mediator.Send(query);
                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
        }
    }
}
