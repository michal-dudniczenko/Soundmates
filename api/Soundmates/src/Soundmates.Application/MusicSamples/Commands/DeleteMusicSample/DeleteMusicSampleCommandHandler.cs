using MediatR;
using Soundmates.Application.Common;
using Soundmates.Domain.Interfaces.Repositories;
using Soundmates.Domain.Interfaces.Services.Auth;

namespace Soundmates.Application.MusicSamples.Commands.DeleteMusicSample;

public class DeleteMusicSampleCommandHandler(
    IMusicSampleRepository _musicSampleRepository,
    IAuthService _authService
) : IRequestHandler<DeleteMusicSampleCommand, Result>
{
    private const string SamplesDirectoryPath = "mp3";

    public async Task<Result> Handle(DeleteMusicSampleCommand request, CancellationToken cancellationToken)
    {
        var authorizedUser = await _authService.GetAuthorizedUserAsync(subClaim: request.SubClaim, checkForFirstLogin: true);

        if (authorizedUser is null)
        {
            return Result.Failure(
                errorType: ErrorType.Unauthorized,
                errorMessage: "Invalid access token.");
        }

        var musicSample = await _musicSampleRepository.GetByIdAsync(request.MusicSampleId);

        if (musicSample is null)
        {
            return Result.Failure(
                errorType: ErrorType.NotFound,
                errorMessage: "No music sample with specified id.");
        }

        if (musicSample.UserId != authorizedUser.Id)
        {
            return Result.Failure(
                errorType: ErrorType.Unauthorized,
                errorMessage: "You can only delete your own music samples.");
        }

        var filePath = Path.Combine("wwwroot", SamplesDirectoryPath, musicSample.FileName);
        if (File.Exists(filePath))
        {
            try
            {
                File.Delete(filePath);
            }
            catch (Exception)
            {
                return Result.Failure(
                    errorType: ErrorType.InternalServerError,
                    errorMessage: "Failed to delete music sample file.");
            }
        }

        await _musicSampleRepository.RemoveAsync(request.MusicSampleId);

        return Result.Success();
    }
}
