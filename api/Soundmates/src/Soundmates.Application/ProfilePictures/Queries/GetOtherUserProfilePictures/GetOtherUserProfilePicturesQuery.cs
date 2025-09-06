using MediatR;
using Soundmates.Application.Common;
using Soundmates.Application.ProfilePictures.Common;

namespace Soundmates.Application.ProfilePictures.Queries.GetOtherUserProfilePictures;

public record GetOtherUserProfilePicturesQuery(Guid OtherUserId, int Limit, int Offset, string? SubClaim) : IRequest<Result<List<OtherUserProfilePictureDto>>>;
