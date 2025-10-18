using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Soundmates.Api.Extensions;
using Soundmates.Application.MusicSamples.Commands.DeleteMusicSample;
using Soundmates.Application.MusicSamples.Commands.UploadMusicSample;
using System.Security.Claims;

namespace Soundmates.Api.Controllers;

[Route("music-samples")]
[ApiController]
public class MusicSamplesController(
    IMediator _mediator
) : ControllerBase
{
    // POST /music-samples
    [HttpPost]
    [Authorize]
    public async Task<ActionResult> UploadMusicSample(IFormFile file)
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
    public async Task<ActionResult> DeleteMusicSample(Guid sampleId)
    {
        var subClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        var command = new DeleteMusicSampleCommand(
            MusicSampleId: sampleId,
            SubClaim: subClaim
        );

        var result = await _mediator.Send(command);

        return this.ResultToHttpResponse(result);
    }
}
