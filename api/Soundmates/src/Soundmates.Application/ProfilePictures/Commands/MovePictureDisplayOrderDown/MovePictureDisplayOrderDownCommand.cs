using MediatR;
using Soundmates.Application.Common;

namespace Soundmates.Application.ProfilePictures.Commands.MovePictureDisplayOrderDown;

public record MovePictureDisplayOrderDownCommand(int ProfilePictureId, string? SubClaim) : IRequest<Result>;
