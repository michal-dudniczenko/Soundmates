using MediatR;
using Soundmates.Application.Common;

namespace Soundmates.Application.MusicSamples.Commands.MoveSampleDisplayOrderUp;

public record MoveSampleDisplayOrderUpCommand(int MusicSampleId, string? SubClaim) : IRequest<Result>;
