using Microsoft.AspNetCore.Diagnostics;

namespace Soundmates.Api.Handlers;

public class GlobalExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
        httpContext.Response.ContentType = "application/json";

        var response = new { message = "Something went wrong." };
        await httpContext.Response.WriteAsJsonAsync(response, cancellationToken);

        return true;
    }
}
