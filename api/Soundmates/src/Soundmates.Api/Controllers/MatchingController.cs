using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Soundmates.Api.DTOs.Matching;
using Soundmates.Api.Extensions;
using Soundmates.Application.Matching.Commands.CreateDislike;
using Soundmates.Application.Matching.Commands.CreateLike;
using Soundmates.Application.Matching.Queries.GetMatches;
using System.Security.Claims;

namespace Soundmates.Api.Controllers;

[Route("matching")]
[ApiController]
public class MatchingController(
    IMediator _mediator
) : ControllerBase
{
    // POST /matching/like
    [HttpPost("like")]
    [Authorize]
    public async Task<IActionResult> CreateLike(
        [FromBody] SwipeDto swipeDto)
    {
        var subClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        var command = new CreateLikeCommand(
            ReceiverId: swipeDto.ReceiverId,
            SubClaim: subClaim);

        var result = await _mediator.Send(command);

        return this.ResultToHttpResponse(result);
    }

    // POST /matching/dislike
    [HttpPost("dislike")]
    [Authorize]
    public async Task<IActionResult> CreateDislike(
        [FromBody] SwipeDto swipeDto)
    {
        var subClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        var command = new CreateDislikeCommand(
            ReceiverId: swipeDto.ReceiverId,
            SubClaim: subClaim);

        var result = await _mediator.Send(command);

        return this.ResultToHttpResponse(result);
    }

    // GET /matching/matches?limit=20&offset=0
    [HttpGet("matches")]
    [Authorize]
    public async Task<IActionResult> GetMatches(
        [FromQuery] int limit = 20,
        [FromQuery] int offset = 0)
    {
        var subClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        var query = new GetMatchesQuery(
            Limit: limit,
            Offset: offset,
            SubClaim: subClaim);

        var result = await _mediator.Send(query);

        return this.ResultToHttpResponse(result);
    }
}
