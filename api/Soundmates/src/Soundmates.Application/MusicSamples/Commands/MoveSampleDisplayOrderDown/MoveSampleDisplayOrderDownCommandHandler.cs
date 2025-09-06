using MediatR;
using Soundmates.Application.Common;
using Soundmates.Domain.Exceptions.MusicSamples;
using Soundmates.Domain.Interfaces.Repositories;
using Soundmates.Domain.Interfaces.Services.Auth;

namespace Soundmates.Application.MusicSamples.Commands.MoveSampleDisplayOrderDown;

public class MoveSampleDisplayOrderDownCommandHandler(
    IMusicSampleRepository _musicSampleRepository,
    IAuthService _authService
) : IRequestHandler<MoveSampleDisplayOrderDownCommand, Result>
{
    public async Task<Result> Handle(MoveSampleDisplayOrderDownCommand request, CancellationToken cancellationToken)
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
                errorMessage: "You can manage only your own music samples.");
        }

        try
        {
            await _musicSampleRepository.MoveDisplayOrderDownAsync(request.MusicSampleId);

        }
        catch (DisplayOrderAlreadyFirstException)
        {
            return Result.Failure(
                errorType: ErrorType.BadRequest,
                errorMessage: "This music sample is already first.");
        }

        return Result.Success();
    }
}
