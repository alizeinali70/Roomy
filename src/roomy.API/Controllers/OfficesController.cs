using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using roomy.Application.Commands.Offices;
using roomy.Application.DTOs;
using roomy.Application.Queries.Offices;

namespace roomy.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class OfficesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public OfficesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<ActionResult<OfficeDto>> CreateOffice([FromBody] CreateOfficeDto createOfficeDto)
        {
            ArgumentNullException.ThrowIfNull(createOfficeDto);

            try
            {
                var command = new CreateOfficeCommand(
                    createOfficeDto.Name,
                    createOfficeDto.Location,
                    createOfficeDto.TotalWorkspaces);

                var result = await _mediator.Send(command);
                return CreatedAtAction(nameof(GetOffice), new { id = result.Id }, result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(StatusCodes.Status403Forbidden, new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<OfficeDto>> UpdateOffice(Guid id, [FromBody] UpdateOfficeDto updateOfficeDto)
        {
            ArgumentNullException.ThrowIfNull(updateOfficeDto);

            try
            {
                var command = new UpdateOfficeCommand(id, updateOfficeDto.Name, updateOfficeDto.Location, updateOfficeDto.TotalWorkspaces);
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

        [HttpGet]
        public async Task<ActionResult<List<OfficeDto>>> ListOffices()
        {
            var query = new ListOfficesQuery();
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<OfficeDto>> GetOffice(Guid id)
        {
            var query = new ListOfficesQuery();
            var offices = await _mediator.Send(query);
            var office = offices.FirstOrDefault(o => o.Id == id);

            if (office == null)
                return NotFound();

            return Ok(office);
        }

        [HttpGet("{id}/availability")]
        public async Task<ActionResult<OfficeAvailabilityDto>> GetAvailability(Guid id, [FromQuery] DateOnly date)
        {
            try
            {
                var query = new GetOfficeAvailabilityQuery(id, date);
                var result = await _mediator.Send(query);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpGet("{id}/availability/range")]
        public async Task<ActionResult<OfficeAvailabilityRangeDto>> GetAvailabilityRange(Guid id, [FromQuery] DateOnly fromDate, [FromQuery] DateOnly toDate)
        {
            try
            {
                var query = new GetOfficeAvailabilityRangeQuery(id, fromDate, toDate);
                var result = await _mediator.Send(query);
                return Ok(result);
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

        [HttpGet("{id}/calendar")]
        public Task<ActionResult<OfficeAvailabilityRangeDto>> GetCalendar(Guid id, [FromQuery] int year, [FromQuery] int month)
        {
            var fromDate = new DateOnly(year, month, 1);
            var toDate = fromDate.AddMonths(1).AddDays(-1);
            return GetAvailabilityRange(id, fromDate, toDate);
        }
    }
}
