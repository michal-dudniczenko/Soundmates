using MediatR;
using Soundmates.Application.Common;
using Soundmates.Application.ProfilePictures.Common;

namespace Soundmates.Application.ProfilePictures.Queries.GetSelfProfilePictures;

public record GetSelfProfilePicturesQuery(int Limit, int Offset, string? SubClaim) : IRequest<Result<List<SelfProfilePictureDto>>>;
