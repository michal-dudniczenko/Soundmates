namespace Soundmates.Application.MusicSamples.Common;

public class SelfMusicSampleDto
{
    public required Guid Id { get; init; }
    public required string FileUrl { get; init; }
    public required int DisplayOrder { get; init; }
}
