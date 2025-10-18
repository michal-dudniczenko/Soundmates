using MediatR;
using Soundmates.Application.Common;
using Soundmates.Application.Users.Commands.UpdateUserProfile.UpdateUserProfileArtist;
using Soundmates.Domain.Entities;
using Soundmates.Domain.Interfaces.Repositories;
using Soundmates.Domain.Interfaces.Services.Auth;

namespace Soundmates.Application.Users.Commands.UpdateUserProfile.UpdateUserProfileBand;

public class UpdateUserProfileBandCommandHandler(
    IUserRepository _userRepository,
    IAuthService _authService
) : IRequestHandler<UpdateUserProfileArtistCommand, Result>
{
    public async Task<Result> Handle(UpdateUserProfileArtistCommand request, CancellationToken cancellationToken)
    {
        var authorizedUser = await _authService.GetAuthorizedUserAsync(subClaim: request.SubClaim, checkForFirstLogin: false);

        if (authorizedUser is null)
        {
            return Result.Failure(
                errorType: ErrorType.Unauthorized,
                errorMessage: "Invalid access token.");
        }

        var updatedUser = new UserBase
        {
            Id = authorizedUser.Id,
            Email = authorizedUser.Email,
            PasswordHash = authorizedUser.PasswordHash,
            Name = request.Name,
            Description = request.Description,
            BirthYear = request.BirthYear,
            City = request.City,
            Country = request.Country,
            IsActive = authorizedUser.IsActive,
            IsFirstLogin = false,
            IsEmailConfirmed = authorizedUser.IsEmailConfirmed,
            IsLoggedOut = authorizedUser.IsLoggedOut
        };

        var updateResult = await _userRepository.UpdateAsync(updatedUser);

        if (!updateResult)
        {
            return Result.Failure(
                errorType: ErrorType.InternalServerError,
                errorMessage: $"Something went wrong. Failed to update user with id: {authorizedUser.Id}"
                );
        }

        return Result.Success();
    }
}
