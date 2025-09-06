using MediatR;
using Soundmates.Application.Common;
using Soundmates.Application.Users.Common;
using Soundmates.Domain.Interfaces.Services.Auth;

namespace Soundmates.Application.Users.Queries.GetSelfUserProfile;

public class GetSelfUserProfileQueryHandler(
    IAuthService _authService
) : IRequestHandler<GetSelfUserProfileQuery, Result<SelfUserProfileDto>>
{
    public async Task<Result<SelfUserProfileDto>> Handle(GetSelfUserProfileQuery request, CancellationToken cancellationToken)
    {
        var authorizedUser = await _authService.GetAuthorizedUserAsync(subClaim: request.SubClaim, checkForFirstLogin: false);

        if (authorizedUser is null)
        {
            return Result<SelfUserProfileDto>.Failure(
                errorType: ErrorType.Unauthorized,
                errorMessage: "Invalid access token.");
        }

        var userProfile = new SelfUserProfileDto
        {
            Id = authorizedUser.Id,
            Email = authorizedUser.Email,
            Name = authorizedUser.Name,
            Description = authorizedUser.Description,
            BirthYear = authorizedUser.BirthYear,
            City = authorizedUser.City,
            Country = authorizedUser.Country
        };

        return Result<SelfUserProfileDto>.Success(userProfile);
    }
}
