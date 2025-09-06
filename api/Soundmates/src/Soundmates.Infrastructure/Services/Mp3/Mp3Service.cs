using Soundmates.Domain.Interfaces.Services.Mp3;

namespace Soundmates.Infrastructure.Services.Mp3;

public class Mp3Service : IMp3Service
{
    public TimeSpan GetMp3FileDuration(Stream fileStream)
    {
        ArgumentNullException.ThrowIfNull(fileStream);

        // Wrap the stream in TagLib# abstraction
        var fileAbstraction = new StreamFileAbstraction(fileStream);

        using var tagFile = TagLib.File.Create(fileAbstraction);

        return tagFile.Properties.Duration;
    }

    // Helper class for TagLib# to read from a generic stream
    private sealed class StreamFileAbstraction : TagLib.File.IFileAbstraction
    {
        private readonly Stream _stream;

        public StreamFileAbstraction(Stream stream)
        {
            _stream = stream;
        }

        public string Name { get; } = "uploaded.mp3";

        public Stream ReadStream => _stream;

        public Stream WriteStream => throw new NotSupportedException();

        public void CloseStream(Stream stream)
        {
            // Don't dispose the stream here; let the caller handle it
        }
    }
}
