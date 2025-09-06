using MediatR;
using Soundmates.Application.Common;

namespace Soundmates.Application.MusicSamples.Commands.MoveSampleDisplayOrderDown;

public record MoveSampleDisplayOrderDownCommand(int MusicSampleId, string? SubClaim) : IRequest<Result>;
