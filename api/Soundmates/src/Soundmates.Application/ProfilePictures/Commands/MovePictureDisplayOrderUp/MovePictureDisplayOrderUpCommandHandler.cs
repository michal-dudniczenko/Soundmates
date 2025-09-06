using MediatR;
using Soundmates.Application.Common;
using Soundmates.Domain.Exceptions.MusicSamples;
using Soundmates.Domain.Interfaces.Repositories;
using Soundmates.Domain.Interfaces.Services.Auth;

namespace Soundmates.Application.ProfilePictures.Commands.MovePictureDisplayOrderUp;

public class MovePictureDisplayOrderUpCommandHandler(
    IProfilePictureRepository _profilePictureRepository,
    IAuthService _authService
) : IRequestHandler<MovePictureDisplayOrderUpCommand, Result>
{
    public async Task<Result> Handle(MovePictureDisplayOrderUpCommand request, CancellationToken cancellationToken)
    {
        var authorizedUser = await _authService.GetAuthorizedUserAsync(subClaim: request.SubClaim, checkForFirstLogin: true);

        if (authorizedUser is null)
        {
            return Result.Failure(
                errorType: ErrorType.Unauthorized,
                errorMessage: "Invalid access token.");
        }

        var profilePicture = await _profilePictureRepository.GetByIdAsync(request.ProfilePictureId);

        if (profilePicture is null)
        {
            return Result.Failure(
                errorType: ErrorType.NotFound,
                errorMessage: "No profile picture with specified id.");
        }

        if (profilePicture.UserId != authorizedUser.Id)
        {
            return Result.Failure(
                errorType: ErrorType.Unauthorized,
                errorMessage: "You can manage only your own profile pictures.");
        }

        try
        {
            await _profilePictureRepository.MoveDisplayOrderUpAsync(request.ProfilePictureId);

        }
        catch (DisplayOrderAlreadyLastException)
        {
            return Result.Failure(
                errorType: ErrorType.BadRequest,
                errorMessage: "This profile picture is already last.");
        }

        return Result.Success();
    }
}
