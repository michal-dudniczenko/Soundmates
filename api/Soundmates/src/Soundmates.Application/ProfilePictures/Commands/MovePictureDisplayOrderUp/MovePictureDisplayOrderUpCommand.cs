using MediatR;
using Soundmates.Application.Common;

namespace Soundmates.Application.ProfilePictures.Commands.MovePictureDisplayOrderUp;

public record MovePictureDisplayOrderUpCommand(Guid ProfilePictureId, string? SubClaim) : IRequest<Result>;
