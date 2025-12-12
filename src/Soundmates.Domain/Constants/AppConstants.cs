namespace Soundmates.Domain.Constants;

public static class AppConstants
{
    public const int minPasswordLength = 8;
    public const int maxPasswordLength = 32;

    public const string SamplesDirectoryPath = "samples/";
    public const int MaxSampleSizeMb = 100;
    public static readonly int MaxSampleSize = MaxSampleSizeMb * 1024 * 1024;
    public static readonly string[] AllowedSampleContentTypes = ["audio/mpeg", "video/mp4"];
    public static readonly string[] AllowedSampleFileExtensions = [".mp3", ".mp4"];
    public const int MaxMusicSamplesCount = 5;

    public const string ImagesDirectoryPath = "images/";
    public const int MaxImageSizeMb = 5;
    public static readonly int MaxImageSize = MaxImageSizeMb * 1024 * 1024;
    public static readonly string[] AllowedImageContentTypes = ["image/jpeg", "image/jpg"];
    public static readonly string[] AllowedImageFileExtensions = [".jpeg", ".jpg"];
    public const int MaxProfilePicturesCount = 5;

    public const int MaxUserEmailLength = 100;
    public const int MaxUserNameLength = 50;
    public const int MaxUserDescriptionLength = 500;

    public static readonly DateOnly MinUserBirthDate = new(1900, 1, 1);

    public const int MaxBandMembersCount = 100;

    public const int MaxBandMemberNameLength = 50;
    public const int MinBandMemberAge = 0;
    public const int MaxBandMemberAge = 100;

    public const int MaxMessageContentLength = 4000;
}
