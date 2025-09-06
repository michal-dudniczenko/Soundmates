using MediatR;
using Soundmates.Application.Common;
using Soundmates.Application.ProfilePictures.Common;
using Soundmates.Domain.Interfaces.Repositories;
using Soundmates.Domain.Interfaces.Services.Auth;

namespace Soundmates.Application.ProfilePictures.Queries.GetOtherUserProfilePictures;

public class GetOtherUserProfilePicturesQueryHandler(
    IUserRepository _userRepository,
    IProfilePictureRepository _profilePictureRepository,
    IAuthService _authService
) : IRequestHandler<GetOtherUserProfilePicturesQuery, Result<List<OtherUserProfilePictureDto>>>
{
    private const string imagesDirectoryPath = "images";
    private const int MaxLimit = 50;
    public async Task<Result<List<OtherUserProfilePictureDto>>> Handle(GetOtherUserProfilePicturesQuery request, CancellationToken cancellationToken)
    {
        var validationResult = PaginationValidator.ValidateLimitOffset<List<OtherUserProfilePictureDto>>(request.Limit, request.Offset, MaxLimit);
        if (!validationResult.IsSuccess)
            return validationResult;

        var authorizedUser = await _authService.GetAuthorizedUserAsync(subClaim: request.SubClaim, checkForFirstLogin: true);

        if (authorizedUser is null)
        {
            return Result<List<OtherUserProfilePictureDto>>.Failure(
                errorType: ErrorType.Unauthorized,
                errorMessage: "Invalid access token.");
        }

        var otherUserExists = await _userRepository.CheckIfExistsActiveAsync(request.OtherUserId);

        if (!otherUserExists)
        {
            return Result<List<OtherUserProfilePictureDto>>.Failure(
                errorType: ErrorType.NotFound,
                errorMessage: $"No user with ID: {request.OtherUserId}");
        }

        var profilePictures = await _profilePictureRepository.GetUserProfilePicturesAsync(
            userId: request.OtherUserId,
            limit: request.Limit,
            offset: request.Offset);

        var profilePicturesDtos = profilePictures.Select(pp => new OtherUserProfilePictureDto
        {
            FileUrl = imagesDirectoryPath + "/" + pp.FileName,
            DisplayOrder = pp.DisplayOrder
        }).ToList();

        return Result<List<OtherUserProfilePictureDto>>.Success(profilePicturesDtos);
    }
}
