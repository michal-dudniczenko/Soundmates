namespace Soundmates.Application.Common;

public static class PaginationValidator
{
    public static Result<T> ValidateLimitOffset<T>(int limit, int offset, int maxLimit)
    {
        if (offset < 0)
        {
            return Result<T>.Failure(
                errorType: ErrorType.BadRequest,
                errorMessage: "Offset parameter value cannot be negative.");
        }

        if (limit <= 0)
        {
            return Result<T>.Failure(
                errorType: ErrorType.BadRequest,
                errorMessage: "Limit parameter value has to be greater than zero.");
        }

        if (limit > maxLimit)
        {
            return Result<T>.Failure(
                errorType: ErrorType.BadRequest,
                errorMessage: $"Limit parameter value cannot be greater than {maxLimit}.");
        }

        return Result<T>.Success(default!); // default value since validation passed
    }
}
