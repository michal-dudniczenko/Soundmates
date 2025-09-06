using MediatR;
using Soundmates.Application.Common;

namespace Soundmates.Application.ProfilePictures.Commands.DeleteProfilePicture;

public record DeleteProfilePictureCommand(Guid ProfilePictureId, string? SubClaim) : IRequest<Result>;
