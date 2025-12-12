using MediatR;
using Soundmates.Application.Common;
using Soundmates.Application.ResponseDTOs.Auth;

namespace Soundmates.Application.Auth.Commands.Refresh;

public record RefreshCommand(string RefreshToken) : IRequest<Result<AccessTokenDto>>;