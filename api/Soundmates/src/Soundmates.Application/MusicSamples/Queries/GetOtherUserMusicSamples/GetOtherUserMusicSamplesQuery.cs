using MediatR;
using Soundmates.Application.Common;
using Soundmates.Application.MusicSamples.Common;

namespace Soundmates.Application.MusicSamples.Queries.GetOtherUserMusicSamples;

public record GetOtherUserMusicSamplesQuery(int OtherUserId, int Limit, int Offset, string? SubClaim) : IRequest<Result<List<OtherUserMusicSampleDto>>>;
