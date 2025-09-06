using MediatR;
using Soundmates.Application.Common;
using Soundmates.Domain.Entities;
using Soundmates.Domain.Interfaces.Repositories;
using Soundmates.Domain.Interfaces.Services.Auth;

namespace Soundmates.Application.Matching.Commands.CreateDislike;

public class CreateDislikeCommandHandler(
    IUserRepository _userRepository,
    IDislikeRepository _dislikeRepository,
    IAuthService _authService
) : IRequestHandler<CreateDislikeCommand, Result>
{
    public async Task<Result> Handle(CreateDislikeCommand request, CancellationToken cancellationToken)
    {
        var authorizedUser = await _authService.GetAuthorizedUserAsync(subClaim: request.SubClaim, checkForFirstLogin: true);

        if (authorizedUser is null)
        {
            return Result.Failure(
                errorType: ErrorType.Unauthorized,
                errorMessage: "Invalid access token.");
        }

        var receiverExists = await _userRepository.CheckIfExistsActiveAsync(request.ReceiverId);
        if (!receiverExists)
        {
            return Result.Failure(
                errorType: ErrorType.NotFound,
                errorMessage: $"No user with ID: {request.ReceiverId}");
        }

        var dislike = new Dislike
        {
            GiverId = authorizedUser.Id,
            ReceiverId = request.ReceiverId
        };

        await _dislikeRepository.AddAsync(dislike);

        return Result.Success();
    }
}
