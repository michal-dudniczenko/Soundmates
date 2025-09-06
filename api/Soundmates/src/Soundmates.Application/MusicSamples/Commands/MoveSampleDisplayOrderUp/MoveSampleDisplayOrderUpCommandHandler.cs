using MediatR;
using Soundmates.Application.Common;
using Soundmates.Domain.Exceptions.MusicSamples;
using Soundmates.Domain.Interfaces.Repositories;
using Soundmates.Domain.Interfaces.Services.Auth;

namespace Soundmates.Application.MusicSamples.Commands.MoveSampleDisplayOrderUp;

public class MoveSampleDisplayOrderUpCommandHandler(
    IMusicSampleRepository _musicSampleRepository,
    IAuthService _authService
) : IRequestHandler<MoveSampleDisplayOrderUpCommand, Result>
{
    public async Task<Result> Handle(MoveSampleDisplayOrderUpCommand request, CancellationToken cancellationToken)
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
            await _musicSampleRepository.MoveDisplayOrderUpAsync(request.MusicSampleId);

        }
        catch (DisplayOrderAlreadyLastException)
        {
            return Result.Failure(
                errorType: ErrorType.BadRequest,
                errorMessage: "This music sample is already last.");
        }

        return Result.Success();
    }
}
