using MediatR;
using Soundmates.Application.Common;
using Soundmates.Application.ProfilePictures.Common;
using Soundmates.Domain.Interfaces.Repositories;
using Soundmates.Domain.Interfaces.Services.Auth;

namespace Soundmates.Application.ProfilePictures.Queries.GetSelfProfilePictures;

public class GetSelfProfilePicturesQueryHandler(
    IProfilePictureRepository _profilePictureRepository,
    IAuthService _authService
) : IRequestHandler<GetSelfProfilePicturesQuery, Result<List<SelfProfilePictureDto>>>
{
    private const string imagesDirectoryPath = "images";
    private const int MaxLimit = 50;
    public async Task<Result<List<SelfProfilePictureDto>>> Handle(GetSelfProfilePicturesQuery request, CancellationToken cancellationToken)
    {
        var validationResult = PaginationValidator.ValidateLimitOffset<List<SelfProfilePictureDto>>(request.Limit, request.Offset, MaxLimit);
        if (!validationResult.IsSuccess)
            return validationResult;

        var authorizedUser = await _authService.GetAuthorizedUserAsync(subClaim: request.SubClaim, checkForFirstLogin: true);

        if (authorizedUser is null)
        {
            return Result<List<SelfProfilePictureDto>>.Failure(
                errorType: ErrorType.Unauthorized,
                errorMessage: "Invalid access token.");
        }

        var profilePictures = await _profilePictureRepository.GetUserProfilePicturesAsync(
            userId: authorizedUser.Id, 
            limit: request.Limit, 
            offset: request.Offset);

        var profilePicturesDtos = profilePictures.Select(pp => new SelfProfilePictureDto
        {
            Id = pp.Id,
            FileUrl = imagesDirectoryPath + "/" + pp.FileName,
            DisplayOrder = pp.DisplayOrder
        }).ToList();

        return Result<List<SelfProfilePictureDto>>.Success(profilePicturesDtos);
    }
}
