using MediatR;
using Soundmates.Application.Common;

namespace Soundmates.Application.Auth.Commands.LogIn;

public record LogInCommand(string Email, string Password) : IRequest<Result<AuthTokens>>;