using MediatR;
using Soundmates.Application.Common;

namespace Soundmates.Application.ProfilePictures.Commands.MovePictureDisplayOrderDown;

public record MovePictureDisplayOrderDownCommand(Guid ProfilePictureId, string? SubClaim) : IRequest<Result>;
