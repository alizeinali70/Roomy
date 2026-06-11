using MediatR;
using Microsoft.AspNetCore.Mvc;
using roomy.Application.Commands.Auths;
using roomy.Application.DTOs;

namespace roomy.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("login")]
        public async Task<ActionResult<LoginResponseDto>> Login([FromBody] LoginDto loginDto)
        {
            ArgumentNullException.ThrowIfNull(loginDto);

            try
            {
                var command = new LoginCommand(loginDto.Email, loginDto.Password);
                var result = await _mediator.Send(command);
                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
        }
    }
}
