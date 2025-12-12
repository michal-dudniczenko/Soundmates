using MediatR;
using Soundmates.Application.Common;

namespace Soundmates.Application.ProfilePictures.Commands.UploadProfilePicture;

public record UploadProfilePictureCommand(Stream FileStream, string FileName, long FileLength, string ContentType, string? SubClaim) : IRequest<Result>;
