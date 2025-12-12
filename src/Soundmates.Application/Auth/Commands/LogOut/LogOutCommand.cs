using MediatR;
using Soundmates.Application.Common;

namespace Soundmates.Application.Auth.Commands.LogOut;

public record LogOutCommand(string? SubClaim) : IRequest<Result>;