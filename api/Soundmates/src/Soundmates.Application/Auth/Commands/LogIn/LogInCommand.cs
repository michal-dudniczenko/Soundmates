using MediatR;
using Soundmates.Application.Common;
using Soundmates.Application.ResponseDTOs.Auth;

namespace Soundmates.Application.Auth.Commands.LogIn;

public record LogInCommand(string Email, string Password) : IRequest<Result<AccessRefreshTokensDto>>;