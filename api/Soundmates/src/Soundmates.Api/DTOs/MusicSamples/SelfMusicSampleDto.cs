namespace Soundmates.Api.DTOs.MusicSamples;

public class SelfMusicSampleDto
{
    public required Guid Id { get; set; }
    public required string FileUrl { get; set; }
    public required int DisplayOrder { get; set; }
}
