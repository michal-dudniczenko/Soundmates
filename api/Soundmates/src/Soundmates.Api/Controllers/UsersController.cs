using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Soundmates.Api.DTOs.Users;
using Soundmates.Api.Extensions;
using Soundmates.Domain.Entities;
using Soundmates.Domain.Interfaces.Auth;
using Soundmates.Domain.Interfaces.Repositories;

namespace Soundmates.Api.Controllers;

[Route("users")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly IUserRepository _userRepository;
    private readonly IAuthService _authService;

    public UsersController(IUserRepository userRepository, IAuthService authService)
    {
        _userRepository = userRepository;
        _authService = authService;
    }

    // GET /users/profile
    [HttpGet("profile")]
    [Authorize]
    public async Task<ActionResult> GetUserProfile()
    {
        var authorizedUser = await this.GetAuthorizedUserAsync(userRepository: _userRepository, checkForFirstLogin: false);

        if (authorizedUser is null)
        {
            return Unauthorized("");
        }

        return Ok(new SelfUserProfileDto
        {
            Id = authorizedUser.Id,
            Email = authorizedUser.Email,
            Name = authorizedUser.Name,
            Description = authorizedUser.Description,
            BirthYear = authorizedUser.BirthYear,
            City = authorizedUser.City,
            Country = authorizedUser.Country
        });
    }

    // GET /users/{id}
    [HttpGet("{id}")]
    [Authorize]
    public async Task<ActionResult> GetUser(int id)
    {
        var authorizedUser = await this.GetAuthorizedUserAsync(userRepository: _userRepository, checkForFirstLogin: true);

        if (authorizedUser is null)
        {
            return Unauthorized("");
        }

        var user = await _userRepository.GetByIdAsync(id);
        if (user == null || !user.IsActive || user.IsFirstLogin)
        {
            return NotFound(new { message = "No user with specified id." });
        }

        return Ok(new OtherUserProfileDto
        {
            Id = user.Id,
            Name = user.Name!,
            Description = user.Description!,
            BirthYear = (int)user.BirthYear!,
            City = user.City!,
            Country = user.Country!
        });
    }

    // GET /users?limit=20&offset=0
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetPotentialMatches(
        [FromQuery] int limit = 20,
        [FromQuery] int offset = 0)
    {
        var authorizedUser = await this.GetAuthorizedUserAsync(userRepository: _userRepository, checkForFirstLogin: true);

        if (authorizedUser is null)
        {
            return Unauthorized("");
        }

        try
        {
            var users = await _userRepository.GetPotentialMatchesAsync(authorizedUser.Id, limit, offset);

            return Ok(users.Select(user =>
                new OtherUserProfileDto
                {
                    Id = user.Id,
                    Name = user.Name!,
                    Description = user.Description!,
                    BirthYear = (int)user.BirthYear!,
                    City = user.City!,
                    Country = user.Country!
                }));
                
        } 
        catch (ArgumentOutOfRangeException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }


    // PUT /users
    [HttpPut]
    [Authorize]
    public async Task<IActionResult> UpdateUser(
        [FromBody] UpdateUserProfileDto updateUserDto)
    {
        var authorizedUser = await this.GetAuthorizedUserAsync(userRepository: _userRepository, checkForFirstLogin: false);

        if (authorizedUser is null)
        {
            return Unauthorized("");
        }

        var updatedUser = new User
        {
            Id = authorizedUser.Id,
            Email = authorizedUser.Email,
            PasswordHash = authorizedUser.PasswordHash,
            Name = updateUserDto.Name,
            Description = updateUserDto.Description,
            BirthYear = updateUserDto.BirthYear,
            City = updateUserDto.City,
            Country = updateUserDto.Country,
            RefreshTokenHash = authorizedUser.RefreshTokenHash,
            RefreshTokenExpiresAt = authorizedUser.RefreshTokenExpiresAt,
            IsActive = authorizedUser.IsActive,
            IsFirstLogin = authorizedUser.IsFirstLogin,
            IsEmailConfirmed = authorizedUser.IsEmailConfirmed,
            IsLoggedOut = authorizedUser.IsLoggedOut
        };

        try
        {
            await _userRepository.UpdateAsync(updatedUser);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }

        return Ok();
    }

    // DELETE /users
    [HttpDelete]
    [Authorize]
    public async Task<IActionResult> DeactivateUserAccount(
        [FromBody] PasswordDto passwordDto)
    {
        var authorizedUser = await this.GetAuthorizedUserAsync(userRepository: _userRepository, checkForFirstLogin: false);

        if (authorizedUser is null)
        {
            return Unauthorized("");
        }

        if (!_authService.VerifyPasswordHash(password: passwordDto.Password, passwordHash: authorizedUser.PasswordHash))
        {
            return Unauthorized(new { message = "Invalid password." });
        }

        await _userRepository.DeactivateUserAccountAsync(authorizedUser.Id);

        return Ok(new { message = "Your account has been deactivated." });
    }

    // POST /users/change-password
    [HttpPost("change-password")]
    [Authorize]
    public async Task<IActionResult> ChangePassword(
        [FromBody] ChangePasswordDto changePasswordDto)
    {
        var authorizedUser = await this.GetAuthorizedUserAsync(userRepository: _userRepository, checkForFirstLogin: false);

        if (authorizedUser is null)
        {
            return Unauthorized("");
        }

        if (!_authService.VerifyPasswordHash(password: changePasswordDto.OldPassword, passwordHash: authorizedUser.PasswordHash))
        {
            return Unauthorized(new { message = "Old password is incorrect." });
        }

        if (!_authService.ValidatePasswordStrength(changePasswordDto.NewPassword))
        {
            return BadRequest(new { message = "New password is invalid." });
        }

        await _userRepository.UpdateUserPasswordAsync(userId: authorizedUser.Id, newPassword: changePasswordDto.NewPassword);

        return Ok(new { message = "Password changed successfully." });
    }
}
