using MediatR;
using Soundmates.Application.Common;

namespace Soundmates.Application.ProfilePictures.Commands.DeleteProfilePicture;

public record DeleteProfilePictureCommand(int ProfilePictureId, string? SubClaim) : IRequest<Result>;
