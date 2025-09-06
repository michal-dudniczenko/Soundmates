namespace Soundmates.Infrastructure.Repositories.Utils;

public static class RepositoryUtils
{
    public static void ValidateLimitOffset(int limit, int offset)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(limit);
        ArgumentOutOfRangeException.ThrowIfNegative(offset);
    }
}
