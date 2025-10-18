using MediatR;
using Soundmates.Application.Common;
using Soundmates.Application.ResponseDTOs.Users;
using Soundmates.Domain.Interfaces.Repositories;
using Soundmates.Domain.Interfaces.Services.Auth;

namespace Soundmates.Application.Matching.Queries.GetPotentialMatchesArtists;

public class GetPotentialMatchesArtistsQueryHandler(
    IUserRepository _userRepository,
    IAuthService _authService
) : IRequestHandler<GetPotentialMatchesArtistsQuery, Result<List<OtherUserProfileArtistDto>>>
{
    private const int MaxLimit = 50;

    public async Task<Result<List<OtherUserProfileArtistDto>>> Handle(GetPotentialMatchesArtistsQuery request, CancellationToken cancellationToken)
    {
        var validationResult = PaginationValidator.ValidateLimitOffset<List<OtherUserProfileArtistDto>>(request.Limit, request.Offset, MaxLimit);
        if (!validationResult.IsSuccess)
            return validationResult;

        var authorizedUser = await _authService.GetAuthorizedUserAsync(subClaim: request.SubClaim, checkForFirstLogin: true);

        if (authorizedUser is null)
        {
            return Result<List<OtherUserProfileArtistDto>>.Failure(
                errorType: ErrorType.Unauthorized,
                errorMessage: "Invalid access token.");
        }

        var users = await _userRepository.GetPotentialMatchesAsync(authorizedUser.Id, request.Limit, request.Offset);

        var usersProfiles = users.Select(user => new OtherUserProfileArtistDto
        {
            Id = user.Id,
            Name = user.Name!,
            Description = user.Description!,
            BirthYear = (int)user.BirthYear!,
            City = user.City!,
            Country = user.Country!
        }).ToList();

        return Result<List<OtherUserProfileArtistDto>>.Success(usersProfiles);
    }
}
