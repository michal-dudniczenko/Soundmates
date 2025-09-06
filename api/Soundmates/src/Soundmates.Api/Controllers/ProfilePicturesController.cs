using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Soundmates.Api.Extensions;
using Soundmates.Application.ProfilePictures.Commands.DeleteProfilePicture;
using Soundmates.Application.ProfilePictures.Commands.MovePictureDisplayOrderDown;
using Soundmates.Application.ProfilePictures.Commands.MovePictureDisplayOrderUp;
using Soundmates.Application.ProfilePictures.Commands.UploadProfilePicture;
using Soundmates.Application.ProfilePictures.Queries.GetOtherUserProfilePictures;
using Soundmates.Application.ProfilePictures.Queries.GetSelfProfilePictures;
using System.Security.Claims;

namespace Soundmates.Api.Controllers;

[Route("profile-pictures")]
[ApiController]
public class ProfilePicturesController(
    IMediator _mediator
) : ControllerBase
{
    // GET /profile-pictures
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetSelfProfilePictures(
        [FromQuery] int limit = 20,
        [FromQuery] int offset = 0)
    {
        var subClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        var query = new GetSelfProfilePicturesQuery(
            Limit: limit,
            Offset: offset,
            SubClaim: subClaim);

        var result = await _mediator.Send(query);

        return this.ResultToHttpResponse(result);
    }

    // GET /profile-pictures/{userId}
    [HttpGet("{userId}")]
    [Authorize]
    public async Task<IActionResult> GetOtherUserProfilePictures(
        int userId,
        [FromQuery] int limit = 20,
        [FromQuery] int offset = 0)
    {
        var subClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        var query = new GetOtherUserProfilePicturesQuery(
            OtherUserId: userId,
            Limit: limit,
            Offset: offset,
            SubClaim: subClaim);

        var result = await _mediator.Send(query);

        return this.ResultToHttpResponse(result);
    }

    // POST /profile-pictures
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> UploadProfilePicture(IFormFile file)
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
    public async Task<IActionResult> DeleteProfilePicture(int pictureId)
    {
        var subClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        var command = new DeleteProfilePictureCommand(
            ProfilePictureId: pictureId,
            SubClaim: subClaim
        );

        var result = await _mediator.Send(command);

        return this.ResultToHttpResponse(result);
    }

    // POST /profile-pictures/move-display-order-up/{pictureId}
    [HttpPost("move-display-order-up/{pictureId}")]
    [Authorize]
    public async Task<IActionResult> MovePictureDisplayOrderUp(int pictureId)
    {
        var subClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        var command = new MovePictureDisplayOrderUpCommand(
            ProfilePictureId: pictureId,
            SubClaim: subClaim
        );

        var result = await _mediator.Send(command);

        return this.ResultToHttpResponse(result);
    }

    // POST /profile-pictures/move-display-order-down/{pictureId}
    [HttpPost("move-display-order-down/{pictureId}")]
    [Authorize]
    public async Task<IActionResult> MovePictureDisplayOrderDown(int pictureId)
    {
        var subClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        var command = new MovePictureDisplayOrderDownCommand(
            ProfilePictureId: pictureId,
            SubClaim: subClaim
        );

        var result = await _mediator.Send(command);

        return this.ResultToHttpResponse(result);
    }
}
