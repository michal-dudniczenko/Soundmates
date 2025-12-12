using MediatR;
using Soundmates.Application.Common;
using Soundmates.Domain.Interfaces.Repositories;
using Soundmates.Domain.Interfaces.Services.Auth;
using static Soundmates.Application.Common.UserMediaHelpers;


namespace Soundmates.Application.ProfilePictures.Commands.DeleteProfilePicture;

public class DeleteProfilePictureCommandHandler(
    IProfilePictureRepository _profilePictureRepository,
    IAuthService _authService
) : IRequestHandler<DeleteProfilePictureCommand, Result>
{
    public async Task<Result> Handle(DeleteProfilePictureCommand request, CancellationToken cancellationToken)
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
                errorMessage: "You can only delete your own profile pictures.");
        }

        var filePath = Path.Combine("wwwroot", GetProfilePictureUrl(profilePicture.FileName));
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
                    errorMessage: "Failed to delete profile picture file.");
            }
        }

        await _profilePictureRepository.RemoveAsync(request.ProfilePictureId);

        return Result.Success();
    }
}
