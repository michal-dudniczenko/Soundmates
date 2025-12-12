using MediatR;
using Soundmates.Application.Common;

namespace Soundmates.Application.Auth.Commands.Register;

public record RegisterCommand(string Email, string Password) : IRequest<Result>;
