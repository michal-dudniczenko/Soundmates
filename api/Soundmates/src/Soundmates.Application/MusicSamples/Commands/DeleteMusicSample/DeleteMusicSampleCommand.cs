using MediatR;
using Soundmates.Application.Common;

namespace Soundmates.Application.MusicSamples.Commands.DeleteMusicSample;

public record DeleteMusicSampleCommand(Guid MusicSampleId, string? SubClaim) : IRequest<Result>;
