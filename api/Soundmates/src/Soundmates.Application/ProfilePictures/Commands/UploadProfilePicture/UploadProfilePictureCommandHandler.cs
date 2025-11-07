using MediatR;
using Soundmates.Application.Common;
using Soundmates.Domain.Entities;
using Soundmates.Domain.Interfaces.Repositories;
using Soundmates.Domain.Interfaces.Services.Auth;
using static Soundmates.Application.Common.UserMediaHelpers;
using static Soundmates.Domain.Constants.AppConstants;

namespace Soundmates.Application.ProfilePictures.Commands.UploadProfilePicture;

public class UploadProfilePictureCommandHandler(
    IProfilePictureRepository _profilePictureRepository,
    IAuthService _authService
) : IRequestHandler<UploadProfilePictureCommand, Result>
{
    public async Task<Result> Handle(UploadProfilePictureCommand request, CancellationToken cancellationToken)
    {
        var authorizedUser = await _authService.GetAuthorizedUserAsync(subClaim: request.SubClaim, checkForFirstLogin: true);

        if (authorizedUser is null)
        {
            return Result.Failure(
                errorType: ErrorType.Unauthorized,
                errorMessage: "Invalid access token.");
        }

        if (!AllowedImageContentTypes.Contains(request.ContentType.ToLower())
            || !AllowedImageFileExtensions.Contains(Path.GetExtension(request.FileName).ToLower()))
        {
            return Result.Failure(
                errorType: ErrorType.BadRequest,
                errorMessage: $"Allowed file extensions: {string.Join(", ", AllowedImageFileExtensions)}");
        }

        if (request.FileLength > MaxImageSize)
        {
            return Result.Failure(
                errorType: ErrorType.BadRequest,
                errorMessage: $"File size cannot exceed {MaxImageSizeMb} MB.");
        }

        var currentUserProfilePicturesCount = await _profilePictureRepository.GetUserProfilePicturesCountAsync(authorizedUser.Id);

        if (currentUserProfilePicturesCount >= MaxProfilePicturesCount)
        {
            return Result.Failure(
                    errorType: ErrorType.BadRequest,
                    errorMessage: $"User can upload maximum of {MaxProfilePicturesCount} profile pictures.");
        }

        var extension = Path.GetExtension(request.FileName).ToLower();
        var fileName = $"{Guid.NewGuid()}{extension}";
        var filePath = Path.Combine("wwwroot", GetProfilePictureUrl(fileName));

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await request.FileStream.CopyToAsync(stream, cancellationToken);
        }

        var profilePicture = new ProfilePicture
        {
            FileName = fileName,
            DisplayOrder = authorizedUser.ProfilePictures.Count,
            UserId = authorizedUser.Id
        };

        await _profilePictureRepository.AddAsync(profilePicture);

        return Result.Success();
    }
}
