using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Soundmates.Api.RequestDTOs.Reports;
using Soundmates.Application.Reports.Commands.ReportUser;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Soundmates.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ReportsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ReportsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("user")]
        public async Task<IActionResult> ReportUser([FromBody] ReportUserDto dto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var command = new ReportUserCommand(userId, dto.ReportedUserId, dto.Reason, dto.Description);
            var result = await _mediator.Send(command);

            if (result.IsSuccess)
            {
                return Ok();
            }

            return BadRequest(result.ErrorMessage);
        }
    }
}
