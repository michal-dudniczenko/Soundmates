namespace Soundmates.Application.Common;
using static Soundmates.Domain.Constants.AppConstants;

public static class UserMediaHelpers
{
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
