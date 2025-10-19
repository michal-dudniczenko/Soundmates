namespace Soundmates.Application.Common;

public static class UserMediaHelpers
{
    private const string SamplesDirectoryPath = "samples/";
    private const string ImagesDirectoryPath = "images/";

    public static string GetMusicSampleUrl(string fileName)
    {
        ArgumentNullException.ThrowIfNullOrWhiteSpace(fileName);

        return SamplesDirectoryPath + fileName;
    }

    public static string GetProfilePictureUrl(string fileName)
    {
        ArgumentNullException.ThrowIfNullOrWhiteSpace(fileName);

        return ImagesDirectoryPath + fileName;
    }
}
