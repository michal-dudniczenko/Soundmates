using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Soundmates.Api.Extensions;
using Soundmates.Api.RequestDTOs.Users;
using Soundmates.Application.ResponseDTOs.Users;
using Soundmates.Application.Users.Commands.ChangePassword;
using Soundmates.Application.Users.Commands.DeactivateAccount;
using Soundmates.Application.Users.Commands.UpdateUserProfile;
using Soundmates.Application.Users.Commands.UpdateUserProfile.UpdateUserProfileArtist;
using Soundmates.Application.Users.Commands.UpdateUserProfile.UpdateUserProfileBand;
using Soundmates.Application.Users.Queries.GetOtherUserProfile;
using Soundmates.Application.Users.Queries.GetSelfUserProfile;
using System.Security.Claims;

namespace Soundmates.Api.Controllers;

[Route("users")]
[ApiController]
public class UsersController(
    IMediator _mediator
) : ControllerBase
{
    // GET /users/profile
    [HttpGet("profile")]
    [Authorize]
    public async Task<ActionResult<SelfUserProfileDto>> GetSelfUserProfile()
    {
        var subClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        var query = new GetSelfUserProfileQuery(
            SubClaim: subClaim);

        var result = await _mediator.Send(query);

        return this.ResultToHttpResponse(result);
    }

    // PUT /users/profile
    [HttpPut("profile")]
    [Authorize]
    public async Task<ActionResult> UpdateUserProfile(
        [FromBody] UpdateUserProfileDto updateUserDto)
    {
        var subClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        UpdateUserProfileCommand command = updateUserDto switch
        {
            UpdateUserProfileArtistDto dto => new UpdateUserProfileArtistCommand(
                Name: dto.Name,
                Description: dto.Description,
                CountryId: dto.CountryId,
                CityId: dto.CityId,
                Tags: dto.Tags,
                BirthDate: dto.BirthDate,
                SubClaim: subClaim),
            UpdateUserProfileBandDto dto => new UpdateUserProfileBandCommand(
                Name: dto.Name,
                Description: dto.Description,
                CountryId: dto.CountryId,
                CityId: dto.CityId,
                Tags: dto.Tags,
                BandMembers: dto.BandMembers,
                SubClaim: subClaim),
            _ => throw new InvalidOperationException()
        };

        var result = await _mediator.Send(command);

        return this.ResultToHttpResponse(result);
    }

    // GET /users/{id}
    [HttpGet("{id}")]
    [Authorize]
    public async Task<ActionResult<OtherUserProfileDto>> GetOtherUserProfile(Guid id)
    {
        var subClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        var query = new GetOtherUserProfileQuery(
            OtherUserId: id,
            SubClaim: subClaim);

        var result = await _mediator.Send(query);

        return this.ResultToHttpResponse(result);
    }

    // DELETE /users
    [HttpDelete]
    [Authorize]
    public async Task<ActionResult> DeactivateAccount(
        [FromBody] PasswordDto passwordDto)
    {
        var subClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        var command = new DeactivateAccountCommand(
            Password: passwordDto.Password,
            SubClaim: subClaim);

        var result = await _mediator.Send(command);

        return this.ResultToHttpResponse(result);
    }

    // POST /users/change-password
    [HttpPost("change-password")]
    [Authorize]
    public async Task<ActionResult> ChangePassword(
        [FromBody] ChangePasswordDto changePasswordDto)
    {
        var subClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        var command = new ChangePasswordCommand(
            OldPassword: changePasswordDto.OldPassword,
            NewPassword: changePasswordDto.NewPassword,
            SubClaim: subClaim);

        var result = await _mediator.Send(command);

        return this.ResultToHttpResponse(result);
    }
}
