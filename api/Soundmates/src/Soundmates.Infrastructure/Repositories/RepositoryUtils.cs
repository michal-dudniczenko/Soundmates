using Soundmates.Domain.Entities;

namespace Soundmates.Infrastructure.Repositories;

public static class RepositoryUtils
{
    public const int MaxLimit = 100;

    public static void ValidateLimitOffset(int limit, int offset, int maxLimit = MaxLimit)
    {
        if (limit <= 0)
            throw new ArgumentOutOfRangeException(nameof(limit), "Limit must be greater than zero.");

        if (limit > maxLimit)
            throw new ArgumentOutOfRangeException(nameof(limit), $"Limit cannot exceed {maxLimit}.");

        if (offset < 0)
            throw new ArgumentOutOfRangeException(nameof(offset), "Offset cannot be negative.");
    }

    public static string GetKeyNotFoundMessage<T>(int entityId) => $"Entity of type {typeof(T).Name} with id {entityId} not found.";
}
