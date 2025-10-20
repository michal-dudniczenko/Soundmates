using MediatR;
using Soundmates.Application.Common;
using Soundmates.Domain.Entities;
using Soundmates.Domain.Interfaces.Repositories;
using Soundmates.Domain.Interfaces.Services.Auth;
using Soundmates.Domain.Interfaces.Services.Mp3;
using static Soundmates.Application.Common.UserMediaHelpers;

namespace Soundmates.Application.MusicSamples.Commands.UploadMusicSample;

public class UploadMusicSampleCommandHandler(
    IMusicSampleRepository _musicSampleRepository,
    IAuthService _authService,
    IMp3Service _mp3Service
) : IRequestHandler<UploadMusicSampleCommand, Result>
{
    private const int MaxSampleSizeMb = 1000;
    private static readonly int MaxSampleSize = MaxSampleSizeMb * 1024 * 1024;
    private static readonly string[] AllowedContentTypes = ["audio/mpeg"];
    private static readonly string[] AllowedExtensions = [".mp3"];
    private const int MaxMusicSamplesCount = 5;
    private static readonly TimeSpan MaxSampleDuration = TimeSpan.FromMinutes(5);

    public async Task<Result> Handle(UploadMusicSampleCommand request, CancellationToken cancellationToken)
    {
        var authorizedUser = await _authService.GetAuthorizedUserAsync(subClaim: request.SubClaim, checkForFirstLogin: true);

        if (authorizedUser is null)
        {
            return Result.Failure(
                errorType: ErrorType.Unauthorized,
                errorMessage: "Invalid access token.");
        }

        if (!AllowedContentTypes.Contains(request.ContentType.ToLower())
            || !AllowedExtensions.Contains(Path.GetExtension(request.FileName).ToLower()))
        {
            return Result.Failure(
                errorType: ErrorType.BadRequest,
                errorMessage: $"Allowed file extensions: {String.Join(", ", AllowedExtensions)}");
        }

        if (request.FileLength > MaxSampleSize)
        {
            return Result.Failure(
                errorType: ErrorType.BadRequest,
                errorMessage: $"File size cannot exceed {MaxSampleSizeMb} MB.");
        }

        if (string.Equals(request.ContentType, "audio/mpeg", StringComparison.OrdinalIgnoreCase))
        {
            try
            {
                var duration = _mp3Service.GetMp3FileDuration(request.FileStream);

                if (duration > MaxSampleDuration)
                {
                    return Result.Failure(
                        errorType: ErrorType.BadRequest,
                        errorMessage: $"Sample is too long. Maximum duration is {MaxSampleDuration.TotalSeconds} seconds.");
                }
            }
            catch
            {
                return Result.Failure(
                        errorType: ErrorType.BadRequest,
                        errorMessage: "Invalid or corrupted MP3 file.");
            }
        }

        if (authorizedUser.MusicSamples.Count >= MaxMusicSamplesCount)
        {
            return Result.Failure(
                    errorType: ErrorType.BadRequest,
                    errorMessage: $"User can upload maximum of {MaxMusicSamplesCount} music samples.");
        }

        var extension = Path.GetExtension(request.FileName).ToLower();
        var fileName = $"{Guid.NewGuid()}{extension}";
        var filePath = Path.Combine("wwwroot", GetMusicSampleUrl(fileName));

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await request.FileStream.CopyToAsync(stream, cancellationToken);
        }

        var musicSample = new MusicSample
        {
            FileName = fileName,
            DisplayOrder = authorizedUser.MusicSamples.Count,
            UserId = authorizedUser.Id
        };

        await _musicSampleRepository.AddAsync(musicSample);

        return Result.Success();
    }
}
