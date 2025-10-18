using MediatR;
using Soundmates.Application.Common;
using Soundmates.Application.ResponseDTOs.Users;
using Soundmates.Domain.Interfaces.Repositories;
using Soundmates.Domain.Interfaces.Services.Auth;

namespace Soundmates.Application.Users.Queries.GetOtherUserProfile;

public class GetOtherUserProfileQueryHandler(
    IUserRepository _userRepository,
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
        if (otherUser is null || !otherUser.IsActive || otherUser.IsFirstLogin || !otherUser.IsEmailConfirmed)
        {
            return Result<OtherUserProfileDto>.Failure(
                errorType: ErrorType.NotFound,
                errorMessage: $"No user with ID: {request.OtherUserId}");
        }

        var otherUserProfile = new OtherUserProfileDto
        {
            Id = otherUser.Id,
            Name = otherUser.Name!,
            Description = otherUser.Description!,
            BirthYear = (int)otherUser.BirthYear!,
            City = otherUser.City!,
            Country = otherUser.Country!
        };

        return Result<OtherUserProfileDto>.Success(otherUserProfile);
    }
}
