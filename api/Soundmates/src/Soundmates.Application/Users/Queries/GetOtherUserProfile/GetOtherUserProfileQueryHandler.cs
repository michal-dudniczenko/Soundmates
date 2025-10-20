using MediatR;
using Soundmates.Application.Common;
using Soundmates.Application.ResponseDTOs.Mappings;
using Soundmates.Application.ResponseDTOs.Users;
using Soundmates.Domain.Interfaces.Repositories;
using Soundmates.Domain.Interfaces.Services.Auth;

namespace Soundmates.Application.Users.Queries.GetOtherUserProfile;

public class GetOtherUserProfileQueryHandler(
    IUserRepository _userRepository,
    IArtistRepository _artistRepository,
    IBandRepository _bandRepository,
    IAuthService _authService
) : IRequestHandler<GetOtherUserProfileQuery, Result<OtherUserProfileDto>>
{
    public async Task<Result<OtherUserProfileDto>> Handle(GetOtherUserProfileQuery request, CancellationToken cancellationToken)
    {
        var authorizedUser = await _authService.GetAuthorizedUserAsync(subClaim: request.SubClaim, checkForFirstLogin: true);

        if (authorizedUser is null)
        {
            return Result<OtherUserProfileDto>.Failure(
                errorType: ErrorType.Unauthorized,
                errorMessage: "Invalid access token.");
        }

        var otherUser = await _userRepository.GetByIdAsync(request.OtherUserId);
        if (otherUser is null || !otherUser.IsActive || otherUser.IsFirstLogin || !otherUser.IsEmailConfirmed || otherUser.IsBand is null)
        {
            return Result<OtherUserProfileDto>.Failure(
                errorType: ErrorType.NotFound,
                errorMessage: $"No user with ID: {request.OtherUserId}");
        }

        OtherUserProfileDto otherUserProfile;

        if ((bool)otherUser.IsBand)
        {
            var band = await _bandRepository.GetByUserIdAsync(otherUser.Id);
            if (band is null)
            {
                return Result<OtherUserProfileDto>.Failure(
                    errorType: ErrorType.NotFound,
                    errorMessage: $"Band with userId = {otherUser.Id} not found.");
            }

            otherUserProfile = band.ToOtherUserProfileDto();
        }
        else
        {
            var artist = await _artistRepository.GetByUserIdAsync(otherUser.Id);
            if (artist is null)
            {
                return Result<OtherUserProfileDto>.Failure(
                    errorType: ErrorType.NotFound,
                    errorMessage: $"Artist with userId = {otherUser.Id} not found.");
            }

            otherUserProfile = artist.ToOtherUserProfileDto();
        }

        return Result<OtherUserProfileDto>.Success(otherUserProfile);
    }
}
