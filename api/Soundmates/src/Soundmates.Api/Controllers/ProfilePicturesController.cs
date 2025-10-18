using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Soundmates.Api.Extensions;
using Soundmates.Application.ProfilePictures.Commands.DeleteProfilePicture;
using Soundmates.Application.ProfilePictures.Commands.UploadProfilePicture;
using System.Security.Claims;

namespace Soundmates.Api.Controllers;

[Route("profile-pictures")]
[ApiController]
public class ProfilePicturesController(
    IMediator _mediator
) : ControllerBase
{
    // POST /profile-pictures
    [HttpPost]
    [Authorize]
    public async Task<ActionResult> UploadProfilePicture(IFormFile file)
    {
        var subClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (file == null || file.Length == 0)
        {
            return BadRequest(new { message = "No file uploaded." });
        }

        using var stream = file.OpenReadStream();

        var command = new UploadProfilePictureCommand(
            FileStream: stream,
            FileName: file.FileName,
            FileLength: file.Length,
            ContentType: file.ContentType,
            SubClaim: subClaim
        );

        var result = await _mediator.Send(command);

        return this.ResultToHttpResponse(result);
    }

    // DELETE /profile-pictures/{pictureId}
    [HttpDelete("{pictureId}")]
    [Authorize]
    public async Task<ActionResult> DeleteProfilePicture(Guid pictureId)
    {
        var subClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        var command = new DeleteProfilePictureCommand(
            ProfilePictureId: pictureId,
            SubClaim: subClaim
        );

        var result = await _mediator.Send(command);

        return this.ResultToHttpResponse(result);
    }
}
