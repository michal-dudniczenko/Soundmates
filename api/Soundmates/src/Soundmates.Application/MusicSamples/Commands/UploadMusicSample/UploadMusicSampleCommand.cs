using MediatR;
using Soundmates.Application.Common;

namespace Soundmates.Application.MusicSamples.Commands.UploadMusicSample;

public record UploadMusicSampleCommand(Stream FileStream, string FileName, long FileLength, string ContentType, string? SubClaim) : IRequest<Result>;
