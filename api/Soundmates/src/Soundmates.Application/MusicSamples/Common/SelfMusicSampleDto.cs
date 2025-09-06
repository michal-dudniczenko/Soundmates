namespace Soundmates.Application.MusicSamples.Common;

public class SelfMusicSampleDto
{
    public required int Id { get; init; }
    public required string FileUrl { get; init; }
    public required int DisplayOrder { get; init; }
}
