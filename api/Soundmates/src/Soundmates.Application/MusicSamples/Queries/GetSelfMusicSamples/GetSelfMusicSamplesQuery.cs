using MediatR;
using Soundmates.Application.Common;
using Soundmates.Application.MusicSamples.Common;

namespace Soundmates.Application.MusicSamples.Queries.GetSelfMusicSamples;

public record GetSelfMusicSamplesQuery(int Limit, int Offset, string? SubClaim) : IRequest<Result<List<SelfMusicSampleDto>>>;
