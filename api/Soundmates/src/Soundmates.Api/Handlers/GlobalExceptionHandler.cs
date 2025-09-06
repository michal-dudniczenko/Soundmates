using Microsoft.AspNetCore.Diagnostics;

namespace Soundmates.Api.Handlers;

public class GlobalExceptionHandler(
    ILogger<GlobalExceptionHandler> _logger
) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        _logger.LogError(exception, "Unhandled exception occurred while processing the request.");

        httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
        httpContext.Response.ContentType = "application/json";

        var response = new { message = "Something went wrong." };
        await httpContext.Response.WriteAsJsonAsync(response, cancellationToken);

        return true;
    }
}
