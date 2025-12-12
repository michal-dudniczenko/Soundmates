using Microsoft.AspNetCore.Mvc;
using Soundmates.Application.Common;

namespace Soundmates.Api.Extensions;

public static class ControllerExtensions
{
    public static ActionResult<T> ResultToHttpResponse<T>(
        this ControllerBase controller,
        Result<T> result)
    {
        var logger = controller.HttpContext.RequestServices
            .GetRequiredService(typeof(ILogger<>)
            .MakeGenericType(controller.GetType()))
            as ILogger;

        ActionResult<T> response;
        int statusCode;

        if (result.IsSuccess)
        {
            response = controller.Ok(result.Value);
            statusCode = 200;
        }
        else
        {
            (response, statusCode) = result.ErrorType switch
            {
                ErrorType.BadRequest => (controller.BadRequest(new { message = result.ErrorMessage }), 400),
                ErrorType.Unauthorized => (controller.Unauthorized(new { message = result.ErrorMessage }), 401),
                ErrorType.NotFound => (controller.NotFound(new { message = result.ErrorMessage }), 404),
                ErrorType.InternalServerError or _ => (controller.StatusCode(500, new { message = result.ErrorMessage }), 500)
            };
        }

        if (statusCode != 200)
        {
            logger?.LogInformation("RESPONSE: {StatusCode} for {Controller}.{Action}: {ErrorMessage}",
                statusCode,
                controller.GetType().Name,
                controller.ControllerContext.ActionDescriptor.ActionName,
                result.ErrorMessage);
        }
        else
        {
            logger?.LogInformation("RESPONSE: {StatusCode} for {Controller}.{Action}",
                statusCode,
                controller.GetType().Name,
                controller.ControllerContext.ActionDescriptor.ActionName);
        }
        return response;
    }

    public static ActionResult ResultToHttpResponse(
        this ControllerBase controller,
        Result result)
    {
        var logger = controller.HttpContext.RequestServices
            .GetRequiredService(typeof(ILogger<>)
            .MakeGenericType(controller.GetType()))
            as ILogger;

        ActionResult response;
        int statusCode;

        if (result.IsSuccess)
        {
            response = controller.Ok();
            statusCode = 200;
        }
        else
        {
            (response, statusCode) = result.ErrorType switch
            {
                ErrorType.BadRequest => (controller.BadRequest(new { message = result.ErrorMessage }), 400),
                ErrorType.Unauthorized => (controller.Unauthorized(new { message = result.ErrorMessage }), 401),
                ErrorType.NotFound => (controller.NotFound(new { message = result.ErrorMessage }), 404),
                ErrorType.InternalServerError or _ => (controller.StatusCode(500, new { message = result.ErrorMessage }), 500)
            };
        }

        if (statusCode != 200)
        {
            logger?.LogInformation("RESPONSE: {StatusCode} for {Controller}.{Action}: {ErrorMessage}",
                statusCode,
                controller.GetType().Name,
                controller.ControllerContext.ActionDescriptor.ActionName,
                result.ErrorMessage);
        }
        else
        {
            logger?.LogInformation("RESPONSE: {StatusCode} for {Controller}.{Action}",
                statusCode,
                controller.GetType().Name,
                controller.ControllerContext.ActionDescriptor.ActionName);
        }
        return response;
    }
}
