namespace Soundmates.Api.Middleware;

public class LogRequestInfoMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<LogRequestInfoMiddleware> _logger;

    public LogRequestInfoMiddleware(RequestDelegate next, ILogger<LogRequestInfoMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        _logger.LogInformation("REQUEST: {Method} {Url}", 
            context.Request.Method, 
            context.Request.Path + context.Request.QueryString);

        await _next(context);
    }
}
