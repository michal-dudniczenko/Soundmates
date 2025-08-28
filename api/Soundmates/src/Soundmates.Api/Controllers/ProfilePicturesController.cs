using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Soundmates.Api.DTOs.ProfilePictures;
using Soundmates.Api.DTOs.Users;
using Soundmates.Api.Extensions;
using Soundmates.Domain.Entities;
using Soundmates.Domain.Interfaces.Repositories;

namespace Soundmates.Api.Controllers;

[Route("profile-pictures")]
[ApiController]
public class ProfilePicturesController : ControllerBase
{
    private const string imagesPath = "images/";
    private const int maxImageSizeMb = 5;
    private const int maxImageSize = 5 * 1024 * 1024;
    private readonly string[] allowedFileExtensions = ["image/jpeg", "image/jpg"];
    private const int maxProfilePicturesCount = 5;

    private readonly IUserRepository _userRepository;
    private readonly IProfilePictureRepository _profilePictureRepository;

    public ProfilePicturesController(IUserRepository userRepository, IProfilePictureRepository profilePictureRepository)
    {
        _userRepository = userRepository;
        _profilePictureRepository = profilePictureRepository;
    }

    // GET /profile-pictures
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetSelfProfilePictures()
    {
        var authorizedUser = await this.GetAuthorizedUserAsync(userRepository: _userRepository, checkForFirstLogin: true);

        if (authorizedUser is null)
        {
            return Unauthorized("");
        }

        var profilePictures = await _profilePictureRepository.GetUserProfilePicturesAsync(authorizedUser.Id);

        var profilePicturesDtos = profilePictures.Select(pp => new SelfProfilePictureDto
        {
            Id = pp.Id,
            FileUrl = imagesPath + pp.FileName,
            DisplayOrder = pp.DisplayOrder
        });

        return Ok(profilePicturesDtos);
    }

    // GET /profile-pictures/{userId}
    [HttpGet("{userId}")]
    [Authorize]
    public async Task<IActionResult> GetUserProfilePictures(int userId)
    {
        var authorizedUser = await this.GetAuthorizedUserAsync(userRepository: _userRepository, checkForFirstLogin: true);

        if (authorizedUser is null)
        {
            return Unauthorized("");
        }

        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null || !user.IsActive || user.IsFirstLogin)
        {
            return NotFound(new { message = "No user with specified id." });
        }

        var profilePictures = await _profilePictureRepository.GetUserProfilePicturesAsync(userId);

        var profilePicturesDtos = profilePictures.Select(pp => new OtherUserProfilePictureDto
        {
            FileUrl = imagesPath + pp.FileName,
            DisplayOrder = pp.DisplayOrder
        });

        return Ok(profilePicturesDtos);
    }

    // POST /profile-pictures
    [HttpPost]
    [RequestSizeLimit((long)(maxImageSize * 1.2))]
    [Authorize]
    public async Task<IActionResult> UploadProfilePicture(IFormFile file)
    {
        var authorizedUser = await this.GetAuthorizedUserAsync(userRepository: _userRepository, checkForFirstLogin: true);

        if (authorizedUser is null)
        {
            return Unauthorized("");
        }

        if (file == null || file.Length == 0)
        {
            return BadRequest(new { message = "No file uploaded." });
        }

        if (!allowedFileExtensions.Contains(file.ContentType.ToLower()))
        {
            return BadRequest(new { message = $"Allowed file extensions: {String.Join(", ", allowedFileExtensions)}" });
        }

        if (file.Length > maxImageSize)
        {
            return BadRequest(new { message = $"File size cannot exceed {maxImageSizeMb} MB." });
        }

        var currentUserProfilePicturesCount = await _profilePictureRepository.GetUserProfilePicturesCountAsync(authorizedUser.Id);

        if (currentUserProfilePicturesCount >= maxProfilePicturesCount)
        {
            return BadRequest(new { message = $"User can upload maximum of {maxProfilePicturesCount} profile pictures." });
        }

        var extension = Path.GetExtension(file.FileName).ToLower();
        var fileName = $"{Guid.NewGuid()}{extension}";
        var filePath = Path.Combine("wwwroot", "images", fileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        var profilePicture = new ProfilePicture
        {
            FileName = fileName,
            UserId = authorizedUser.Id
        };

        await _profilePictureRepository.AddAsync(profilePicture);

        return Ok();
    }

    // DELETE /profile-pictures/{pictureId}
    [HttpDelete("{pictureId}")]
    [Authorize]
    public async Task<IActionResult> DeleteProfilePicture(int pictureId)
    {
        var authorizedUser = await this.GetAuthorizedUserAsync(userRepository: _userRepository, checkForFirstLogin: true);

        if (authorizedUser is null)
        {
            return Unauthorized("");
        }

        var profilePicture = await _profilePictureRepository.GetByIdAsync(pictureId);

        if (profilePicture is null)
        {
            return NotFound(new { message = "No profile picture with specified id." });
        }

        if (profilePicture.UserId != authorizedUser.Id)
        {
            return Unauthorized("");
        }

        var filePath = Path.Combine("wwwroot", "images", profilePicture.FileName);
        if (System.IO.File.Exists(filePath))
        {
            try
            {
                System.IO.File.Delete(filePath);
                await _profilePictureRepository.RemoveAsync(pictureId);
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Failed to delete profile picture file." });
            }
        }

        return Ok();
    }

    // POST /profile-pictures/move-display-order-up/{pictureId}
    [HttpPost("move-display-order-up/{pictureId}")]
    [Authorize]
    public async Task<IActionResult> MoveDisplayOrderUp(int pictureId)
    {
        var authorizedUser = await this.GetAuthorizedUserAsync(userRepository: _userRepository, checkForFirstLogin: true);

        if (authorizedUser is null)
        {
            return Unauthorized("");
        }

        var profilePicture = await _profilePictureRepository.GetByIdAsync(pictureId);

        if (profilePicture is null)
        {
            return NotFound(new { message = "No profile picture with specified id." });
        }

        if (profilePicture.UserId != authorizedUser.Id)
        {
            return Unauthorized("");
        }

        try
        {
            await _profilePictureRepository.MoveDisplayOrderUpAsync(pictureId);

        } catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }

        return Ok();
    }

    // POST /profile-pictures/move-display-order-down/{pictureId}
    [HttpPost("move-display-order-down/{pictureId}")]
    [Authorize]
    public async Task<IActionResult> MoveDisplayOrderDown(int pictureId)
    {
        var authorizedUser = await this.GetAuthorizedUserAsync(userRepository: _userRepository, checkForFirstLogin: true);

        if (authorizedUser is null)
        {
            return Unauthorized("");
        }

        var profilePicture = await _profilePictureRepository.GetByIdAsync(pictureId);

        if (profilePicture is null)
        {
            return NotFound(new { message = "No profile picture with specified id." });
        }

        if (profilePicture.UserId != authorizedUser.Id)
        {
            return Unauthorized("");
        }

        try
        {
            await _profilePictureRepository.MoveDisplayOrderDownAsync(pictureId);

        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }

        return Ok();
    }
}
