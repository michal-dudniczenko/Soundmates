using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Soundmates.Api.DTOs.Users;
using Soundmates.Api.Extensions;
using Soundmates.Application.Users.Commands.ChangePassword;
using Soundmates.Application.Users.Commands.DeactivateAccount;
using Soundmates.Application.Users.Commands.UpdateUserProfile;
using Soundmates.Application.Users.Queries.GetOtherUserProfile;
using Soundmates.Application.Users.Queries.GetPotentialMatches;
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
    public async Task<IActionResult> GetSelfUserProfile()
    {
        var subClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        var query = new GetSelfUserProfileQuery(
            SubClaim: subClaim);

        var result = await _mediator.Send(query);

        return this.ResultToHttpResponse(result);
    }

    // GET /users/{id}
    [HttpGet("{id}")]
    [Authorize]
    public async Task<IActionResult> GetOtherUserProfile(Guid id)
    {
        var subClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        var query = new GetOtherUserProfileQuery(
            OtherUserId: id,
            SubClaim: subClaim);

        var result = await _mediator.Send(query);

        return this.ResultToHttpResponse(result);
    }

    // GET /users?limit=20&offset=0
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetPotentialMatches(
        [FromQuery] int limit = 20,
        [FromQuery] int offset = 0)
    {
        var subClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        var query = new GetPotentialMatchesQuery(
            Limit: limit,
            Offset: offset,
            SubClaim: subClaim);

        var result = await _mediator.Send(query);

        return this.ResultToHttpResponse(result);
    }


    // PUT /users
    [HttpPut]
    [Authorize]
    public async Task<IActionResult> UpdateUserProfile(
        [FromBody] UpdateUserProfileDto updateUserDto)
    {
        var subClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        var command = new UpdateUserProfileCommand(
            Name: updateUserDto.Name,
            Description: updateUserDto.Description,
            BirthYear: updateUserDto.BirthYear,
            City: updateUserDto.City,
            Country: updateUserDto.Country,
            SubClaim: subClaim);

        var result = await _mediator.Send(command);

        return this.ResultToHttpResponse(result);
    }

    // DELETE /users
    [HttpDelete]
    [Authorize]
    public async Task<IActionResult> DeactivateAccount(
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
    public async Task<IActionResult> ChangePassword(
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
