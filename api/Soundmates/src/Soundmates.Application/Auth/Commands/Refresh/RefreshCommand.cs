using MediatR;
using Soundmates.Application.Common;

namespace Soundmates.Application.Auth.Commands.Refresh;

public record RefreshCommand(string RefreshToken) : IRequest<Result<AuthAccessToken>>;