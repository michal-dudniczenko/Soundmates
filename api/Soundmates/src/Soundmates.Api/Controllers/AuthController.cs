using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Soundmates.Api.Extensions;
using Soundmates.Api.RequestDTOs.Auth;
using Soundmates.Application.Auth.Commands.LogIn;
using Soundmates.Application.Auth.Commands.LogOut;
using Soundmates.Application.Auth.Commands.Refresh;
using Soundmates.Application.Auth.Commands.Register;
using Soundmates.Application.ResponseDTOs.Auth;
using System.Security.Claims;

namespace Soundmates.Api.Controllers;

[Route("auth")]
[ApiController]
public class AuthController(
    IMediator _mediator
) : ControllerBase
{
    // POST /users/register
    [HttpPost("register")]
    public async Task<ActionResult> Register(
        [FromBody] RegisterDto registerUserDto)
    {
        var command = new RegisterCommand(
            Email: registerUserDto.Email, 
            Password: registerUserDto.Password);

        var result = await _mediator.Send(command);

        return this.ResultToHttpResponse(result);
    }

    // POST /users/login
    [HttpPost("login")]
    public async Task<ActionResult<AccessRefreshTokensDto>> LogIn(
        [FromBody] LoginDto loginDto)
    {
        var command = new LogInCommand(
            Email: loginDto.Email.NormalizeEmail(),
            Password: loginDto.Password);

        var result = await _mediator.Send(command);

        return this.ResultToHttpResponse(result);
    }

    // POST /users/refresh
    [HttpPost("refresh")]
    public async Task<ActionResult<AccessTokenDto>> Refresh(
        [FromBody] RefreshTokenDto refreshTokenDto)
    {
        var command = new RefreshCommand(
            RefreshToken: refreshTokenDto.RefreshToken);

        var result = await _mediator.Send(command);

        return this.ResultToHttpResponse(result);
    }

    // POST /users/logout
    [HttpPost("logout")]
    [Authorize]
    public async Task<ActionResult> LogOut()
    {
        var subClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        var command = new LogOutCommand(
            SubClaim: subClaim);

        var result = await _mediator.Send(command);

        return this.ResultToHttpResponse(result);
    }
}
