using MediatR;
using Soundmates.Application.Common;
using Soundmates.Domain.Entities;
using Soundmates.Domain.Enums;
using Soundmates.Domain.Interfaces.Repositories;
using Soundmates.Domain.Interfaces.Services.Auth;

namespace Soundmates.Application.Auth.Commands.Register;

public class RegisterCommandHandler(
    IUserRepository _userRepository,
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

        UserBase user = request.UserType switch
        {
            UserType.Artist => new Artist
            {
                Email = request.Email,
                PasswordHash = _authService.GetPasswordHash(request.Password)
            },
            UserType.Band => new Band
            {
                Email = request.Email,
                PasswordHash = _authService.GetPasswordHash(request.Password)
            },
            _ => throw new InvalidOperationException()
        };

        await _userRepository.AddAsync(user);

        return Result.Success();
    }
}
