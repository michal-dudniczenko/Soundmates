using MediatR;
using Soundmates.Application.Common;

namespace Soundmates.Application.ProfilePictures.Commands.MovePictureDisplayOrderUp;

public record MovePictureDisplayOrderUpCommand(int ProfilePictureId, string? SubClaim) : IRequest<Result>;
