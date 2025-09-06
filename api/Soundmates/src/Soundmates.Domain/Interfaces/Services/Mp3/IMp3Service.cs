namespace Soundmates.Domain.Interfaces.Services.Mp3;

public interface IMp3Service
{
    TimeSpan GetMp3FileDuration(Stream fileStream);
}
