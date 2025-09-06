using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Soundmates.Api.Extensions;
using Soundmates.Application.MusicSamples.Commands.DeleteMusicSample;
using Soundmates.Application.MusicSamples.Commands.MoveSampleDisplayOrderDown;
using Soundmates.Application.MusicSamples.Commands.MoveSampleDisplayOrderUp;
using Soundmates.Application.MusicSamples.Commands.UploadMusicSample;
using Soundmates.Application.MusicSamples.Queries.GetOtherUserMusicSamples;
using Soundmates.Application.MusicSamples.Queries.GetSelfMusicSamples;
using System.Security.Claims;

namespace Soundmates.Api.Controllers;

[Route("music-samples")]
[ApiController]
public class MusicSamplesController(
    IMediator _mediator
) : ControllerBase
{
    // GET /music-samples?limit=20&offset=0
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetSelfMusicSamples(
        [FromQuery] int limit = 20,
        [FromQuery] int offset = 0)
    {
        var subClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        var query = new GetSelfMusicSamplesQuery(
            Limit: limit,
            Offset: offset,
            SubClaim: subClaim);

        var result = await _mediator.Send(query);

        return this.ResultToHttpResponse(result);
    }

    // GET /music-samples/{userId}
    [HttpGet("{userId}")]
    [Authorize]
    public async Task<IActionResult> GetOtherUserMusicSamples(
        Guid userId,
        [FromQuery] int limit = 20,
        [FromQuery] int offset = 0)
    {
        var subClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        var query = new GetOtherUserMusicSamplesQuery(
            OtherUserId: userId,
            Limit: limit,
            Offset: offset,
            SubClaim: subClaim);

        var result = await _mediator.Send(query);

        return this.ResultToHttpResponse(result);
    }

    // POST /music-samples
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> UploadMusicSample(IFormFile file)
    {
        var subClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (file == null || file.Length == 0)
        {
            return BadRequest(new { message = "No file uploaded." });
        }

        using var stream = file.OpenReadStream();

        var command = new UploadMusicSampleCommand(
            FileStream: stream,
            FileName: file.FileName,
            FileLength: file.Length,
            ContentType: file.ContentType,
            SubClaim: subClaim
        );

        var result = await _mediator.Send(command);

        return this.ResultToHttpResponse(result);
    }

    // DELETE /music-samples/{sampleId}
    [HttpDelete("{sampleId}")]
    [Authorize]
    public async Task<IActionResult> DeleteMusicSample(Guid sampleId)
    {
        var subClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        var command = new DeleteMusicSampleCommand(
            MusicSampleId: sampleId,
            SubClaim: subClaim
        );

        var result = await _mediator.Send(command);

        return this.ResultToHttpResponse(result);
    }

    // POST /music-samples/move-display-order-up/{sampleId}
    [HttpPost("move-display-order-up/{sampleId}")]
    [Authorize]
    public async Task<IActionResult> MoveSampleDisplayOrderUp(Guid sampleId)
    {
        var subClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        var command = new MoveSampleDisplayOrderUpCommand(
            MusicSampleId: sampleId,
            SubClaim: subClaim
        );

        var result = await _mediator.Send(command);

        return this.ResultToHttpResponse(result);
    }

    // POST /music-samples/move-display-order-down/{sampleId}
    [HttpPost("move-display-order-down/{sampleId}")]
    [Authorize]
    public async Task<IActionResult> MoveSampleDisplayOrderDown(Guid sampleId)
    {
        var subClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        var command = new MoveSampleDisplayOrderDownCommand(
            MusicSampleId: sampleId,
            SubClaim: subClaim
        );

        var result = await _mediator.Send(command);

        return this.ResultToHttpResponse(result);
    }
}
