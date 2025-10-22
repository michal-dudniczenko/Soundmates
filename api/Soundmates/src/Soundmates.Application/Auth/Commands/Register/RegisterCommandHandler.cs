using MediatR;
using Soundmates.Application.Common;
using Soundmates.Domain.Entities;
using Soundmates.Domain.Interfaces.Repositories;
using Soundmates.Domain.Interfaces.Services.Auth;

namespace Soundmates.Application.Auth.Commands.Register;

public class RegisterCommandHandler(
    IUserRepository _userRepository,
    IMatchPreferenceRepository _matchPreferenceRepository,
    IAuthService _authService
) : IRequestHandler<RegisterCommand, Result>
{
    public async Task<Result> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        if (await _userRepository.CheckIfEmailExistsAsync(request.Email))
        {
            return Result.Failure(
                errorType: ErrorType.BadRequest, 
                errorMessage: "User with that email already exists.");
        }

        var user = new User
        {
            Email = request.Email,
            PasswordHash = _authService.GetPasswordHash(request.Password)
        };

        await _userRepository.AddAsync(user);

        var defaultMatchPreference = new UserMatchPreference
        {
            UserId = user.Id
        };

        await _matchPreferenceRepository.AddUpdateAsync(defaultMatchPreference);

        return Result.Success();
    }
}
