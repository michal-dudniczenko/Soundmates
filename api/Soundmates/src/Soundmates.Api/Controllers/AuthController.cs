using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Soundmates.Api.DTOs.Auth;
using Soundmates.Api.Extensions;
using Soundmates.Domain.Entities;
using Soundmates.Domain.Interfaces.Auth;
using Soundmates.Domain.Interfaces.Repositories;

namespace Soundmates.Api.Controllers;

[Route("auth")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IUserRepository _userRepository;
    private readonly IAuthService _authService;

    public AuthController(IUserRepository userRepository, IAuthService authService)
    {
        _userRepository = userRepository;
        _authService = authService;
    }

    // POST /users/register
    [HttpPost("register")]
    public async Task<IActionResult> Register(
        [FromBody] RegisterDto registerUserDto)
    {
        if (await _userRepository.CheckIfEmailExistsAsync(registerUserDto.Email))
        {
            return BadRequest(new { message = "User with that email address already exists." });
        }

        var user = new User
        {
            Email = registerUserDto.Email,
            PasswordHash = _authService.GetPasswordHash(registerUserDto.Password)
        };

        await _userRepository.AddAsync(user);

        return Ok(new { message = "User registered successfully." });
    }

    // POST /users/login
    [HttpPost("login")]
    public async Task<IActionResult> Login(
        [FromBody] LoginDto loginDto)
    {
        var user = await _userRepository.GetByEmailAsync(email: loginDto.Email);

        if (user is null)
        {
            return Unauthorized(new { message = "Invalid email or password." });
        }

        var result = _authService.VerifyPasswordHash(password: loginDto.Password, passwordHash: user.PasswordHash);

        if (!result)
        {
            return Unauthorized(new { message = "Invalid email or password." });
        }

        if (!user.IsActive)
        {
            return Unauthorized(new { message = "Your account has been deactivated. Contact administrator" });
        }

        var accessToken = _authService.GenerateAccessToken(userId: user.Id);
        var refreshToken = _authService.GenerateRefreshToken(userId: user.Id);

        await _userRepository.LogInUserAsync(userId: user.Id, newRefreshToken: refreshToken);

        return Ok(new AccessRefreshTokensDto { AccessToken = accessToken, RefreshToken = refreshToken });
    }

    // POST /users/refresh
    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh(
        [FromBody] RefreshTokenDto refreshTokenDto)
    {
        var userId = await _userRepository.CheckRefreshTokenGetUserIdAsync(refreshToken: refreshTokenDto.RefreshToken);
        if (userId is null)
        {
            return Unauthorized(new { message = "Invalid refresh token. Log in to get a new one." });
        }

        var accessToken = _authService.GenerateAccessToken(userId: (int)userId);

        return Ok(new AccessTokenDto { AccessToken = accessToken });
    }

    // POST /users/logout
    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        var authorizedUser = await this.GetAuthorizedUserAsync(userRepository: _userRepository, checkForFirstLogin: false);

        if (authorizedUser is null)
        {
            return Unauthorized("");
        }

        await _userRepository.LogOutUserAsync(userId: authorizedUser.Id);

        return Ok(new { message = "Logged out successfully." });
    }
}
