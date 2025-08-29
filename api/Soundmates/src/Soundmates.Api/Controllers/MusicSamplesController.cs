using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Soundmates.Api.DTOs.MusicSamples;
using Soundmates.Api.Extensions;
using Soundmates.Domain.Entities;
using Soundmates.Domain.Interfaces.Mp3;
using Soundmates.Domain.Interfaces.Repositories;

namespace Soundmates.Api.Controllers;

[Route("music-samples")]
[ApiController]
public class MusicSamplesController : ControllerBase
{
    private const string samplesDirectoryPath = "mp3";
    private const int maxSampleSizeMb = 5;
    private const int maxSampleSize = maxSampleSizeMb * 1024 * 1024;
    private readonly string[] allowedFileExtensions = ["audio/mpeg"];
    private const int maxMusicSamplesCount = 5;
    private readonly TimeSpan maxSampleDuration = TimeSpan.FromMinutes(5);

    private readonly IMp3Service _mp3Service;

    private readonly IUserRepository _userRepository;
    private readonly IMusicSampleRepository _musicSampleRepository;

    public MusicSamplesController(IMp3Service mp3Service, IUserRepository userRepository, IMusicSampleRepository musicSampleRepository)
    {
        _mp3Service = mp3Service;
        _userRepository = userRepository;
        _musicSampleRepository = musicSampleRepository;
    }

    // GET /music-samples
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetSelfMusicSamples()
    {
        var authorizedUser = await this.GetAuthorizedUserAsync(userRepository: _userRepository, checkForFirstLogin: true);

        if (authorizedUser is null)
        {
            return Unauthorized("");
        }

        var musicSamples = await _musicSampleRepository.GetUserMusicSamplesAsync(authorizedUser.Id);

        var musicSamplesDtos = musicSamples.Select(ms => new SelfMusicSampleDto
        {
            Id = ms.Id,
            FileUrl = samplesDirectoryPath + "/" + ms.FileName,
            DisplayOrder = ms.DisplayOrder
        });

        return Ok(musicSamplesDtos);
    }

    // GET /music-samples/{userId}
    [HttpGet("{userId}")]
    [Authorize]
    public async Task<IActionResult> GetUserMusicSamples(int userId)
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

        var musicSamples = await _musicSampleRepository.GetUserMusicSamplesAsync(userId);

        var musicSamplesDtos = musicSamples.Select(ms => new OtherUserMusicSampleDto
        {
            FileUrl = samplesDirectoryPath + "/" + ms.FileName,
            DisplayOrder = ms.DisplayOrder
        });

        return Ok(musicSamplesDtos);
    }

    // POST /music-samples
    [HttpPost]
    [RequestSizeLimit((long)(maxSampleSize * 1.2))]
    [Authorize]
    public async Task<IActionResult> UploadMusicSample(IFormFile file)
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

        if (file.Length > maxSampleSize)
        {
            return BadRequest(new { message = $"File size cannot exceed {maxSampleSizeMb} MB." });
        }

        try
        {
            using var stream = file.OpenReadStream();
            var duration = _mp3Service.GetMp3FileDuration(stream);

            if (duration > maxSampleDuration)
                return BadRequest(new { message = $"Sample is too long. Maximum duration is {maxSampleDuration.TotalSeconds} seconds." });
        }
        catch
        {
            return BadRequest(new { message = "Invalid or corrupted MP3 file." });
        }

        var currentUserMusicSamplesCount = await _musicSampleRepository.GetUserMusicSamplesCountAsync(authorizedUser.Id);

        if (currentUserMusicSamplesCount >= maxMusicSamplesCount)
        {
            return BadRequest(new { message = $"User can upload maximum of {maxMusicSamplesCount} music samples." });
        }

        var extension = Path.GetExtension(file.FileName).ToLower();
        var fileName = $"{Guid.NewGuid()}{extension}";
        var filePath = Path.Combine("wwwroot", samplesDirectoryPath, fileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        var musicSample = new MusicSample
        {
            FileName = fileName,
            UserId = authorizedUser.Id
        };

        await _musicSampleRepository.AddAsync(musicSample);

        return Ok();
    }

    // DELETE /music-samples/{sampleId}
    [HttpDelete("{sampleId}")]
    [Authorize]
    public async Task<IActionResult> DeleteMusicSample(int sampleId)
    {
        var authorizedUser = await this.GetAuthorizedUserAsync(userRepository: _userRepository, checkForFirstLogin: true);

        if (authorizedUser is null)
        {
            return Unauthorized("");
        }

        var musicSample = await _musicSampleRepository.GetByIdAsync(sampleId);

        if (musicSample is null)
        {
            return NotFound(new { message = "No music sample with specified id." });
        }

        if (musicSample.UserId != authorizedUser.Id)
        {
            return Unauthorized("");
        }

        var filePath = Path.Combine("wwwroot", samplesDirectoryPath, musicSample.FileName);
        if (System.IO.File.Exists(filePath))
        {
            try
            {
                System.IO.File.Delete(filePath);
                await _musicSampleRepository.RemoveAsync(sampleId);
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Failed to delete music sample file." });
            }
        }

        return Ok();
    }

    // POST /music-samples/move-display-order-up/{sampleId}
    [HttpPost("move-display-order-up/{sampleId}")]
    [Authorize]
    public async Task<IActionResult> MoveDisplayOrderUp(int sampleId)
    {
        var authorizedUser = await this.GetAuthorizedUserAsync(userRepository: _userRepository, checkForFirstLogin: true);

        if (authorizedUser is null)
        {
            return Unauthorized("");
        }

        var musicSample = await _musicSampleRepository.GetByIdAsync(sampleId);

        if (musicSample is null)
        {
            return NotFound(new { message = "No music sample with specified id." });
        }

        if (musicSample.UserId != authorizedUser.Id)
        {
            return Unauthorized("");
        }

        try
        {
            await _musicSampleRepository.MoveDisplayOrderUpAsync(sampleId);

        } catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }

        return Ok();
    }

    // POST /music-samples/move-display-order-down/{sampleId}
    [HttpPost("move-display-order-down/{sampleId}")]
    [Authorize]
    public async Task<IActionResult> MoveDisplayOrderDown(int sampleId)
    {
        var authorizedUser = await this.GetAuthorizedUserAsync(userRepository: _userRepository, checkForFirstLogin: true);

        if (authorizedUser is null)
        {
            return Unauthorized("");
        }

        var musicSample = await _musicSampleRepository.GetByIdAsync(sampleId);

        if (musicSample is null)
        {
            return NotFound(new { message = "No music sample with specified id." });
        }

        if (musicSample.UserId != authorizedUser.Id)
        {
            return Unauthorized("");
        }

        try
        {
            await _musicSampleRepository.MoveDisplayOrderDownAsync(sampleId);

        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }

        return Ok();
    }
}
