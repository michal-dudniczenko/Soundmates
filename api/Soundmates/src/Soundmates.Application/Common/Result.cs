namespace Soundmates.Application.Common;

public enum ErrorType
{
    BadRequest,
    Unauthorized,
    NotFound,
    InternalServerError
}

public class Result<T>
{
    public bool IsSuccess { get; init; }
    public T? Value { get; init; }
    public ErrorType? ErrorType { get; init; }
    public string? ErrorMessage { get; init; }

    public static Result<T> Success(T value) =>
        new() { IsSuccess = true, Value = value };

    public static Result<T> Failure(ErrorType errorType, string errorMessage) =>
        new() { IsSuccess = false, ErrorType = errorType, ErrorMessage = errorMessage };
}

public class Result
{
    public bool IsSuccess { get; init; }
    public ErrorType? ErrorType { get; init; }
    public string? ErrorMessage { get; init; }

    public static Result Success() => new() { IsSuccess = true };
    public static Result Failure(ErrorType errorType, string errorMessage) =>
        new() { IsSuccess = false, ErrorType = errorType, ErrorMessage = errorMessage };
}
