using MediatR;
using Soundmates.Application.Common;
using Soundmates.Application.Users.Common;
using Soundmates.Domain.Interfaces.Repositories;
using Soundmates.Domain.Interfaces.Services.Auth;

namespace Soundmates.Application.Users.Queries.GetPotentialMatches;

public class GetPotentialMatchesQueryHandler(
    IUserRepository _userRepository,
    IAuthService _authService
) : IRequestHandler<GetPotentialMatchesQuery, Result<List<OtherUserProfileDto>>>
{
    private const int MaxLimit = 50;

    public async Task<Result<List<OtherUserProfileDto>>> Handle(GetPotentialMatchesQuery request, CancellationToken cancellationToken)
    {
        var validationResult = PaginationValidator.ValidateLimitOffset<List<OtherUserProfileDto>>(request.Limit, request.Offset, MaxLimit);
        if (!validationResult.IsSuccess)
            return validationResult;

        var authorizedUser = await _authService.GetAuthorizedUserAsync(subClaim: request.SubClaim, checkForFirstLogin: true);

        if (authorizedUser is null)
        {
            return Result<List<OtherUserProfileDto>>.Failure(
                errorType: ErrorType.Unauthorized,
                errorMessage: "Invalid access token.");
        }

        var users = await _userRepository.GetPotentialMatchesAsync(authorizedUser.Id, request.Limit, request.Offset);

        var usersProfiles = users.Select(user => new OtherUserProfileDto
        {
            Id = user.Id,
            Name = user.Name!,
            Description = user.Description!,
            BirthYear = (int)user.BirthYear!,
            City = user.City!,
            Country = user.Country!
        }).ToList();

        return Result<List<OtherUserProfileDto>>.Success(usersProfiles);
    }
}
