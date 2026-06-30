using FluentValidation;

namespace NaarNoor.API.Middleware;

/// <summary>
/// Exception handling middleware. Maps known exception types to the appropriate
/// HTTP status code so clients receive consistent, actionable error responses
/// instead of a generic 500 for every failure.
/// </summary>
public static class ExceptionHandlingMiddleware
{
    public static void UseExceptionHandlingMiddleware(this WebApplication app)
    {
        app.UseExceptionHandler(errorApp =>
        {
            errorApp.Run(async context =>
            {
                var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
                var exceptionFeature = context.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerPathFeature>();

                if (exceptionFeature?.Error is not Exception exception)
                {
                    return;
                }

                context.Response.ContentType = "application/json";

                switch (exception)
                {
                    case ValidationException validationException:
                        logger.LogWarning(validationException, "Validation failure");
                        context.Response.StatusCode = StatusCodes.Status400BadRequest;

                        var errors = validationException.Errors
                            .GroupBy(e => e.PropertyName)
                            .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());

                        await context.Response.WriteAsJsonAsync(new
                        {
                            error = "Validation failed",
                            statusCode = StatusCodes.Status400BadRequest,
                            errors
                        });
                        break;

                    case KeyNotFoundException:
                        logger.LogWarning(exception, "Resource not found");
                        context.Response.StatusCode = StatusCodes.Status404NotFound;
                        await context.Response.WriteAsJsonAsync(new
                        {
                            error = exception.Message,
                            statusCode = StatusCodes.Status404NotFound
                        });
                        break;

                    case UnauthorizedAccessException:
                        logger.LogWarning(exception, "Unauthorized access");
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        await context.Response.WriteAsJsonAsync(new
                        {
                            error = "Unauthorized",
                            statusCode = StatusCodes.Status401Unauthorized
                        });
                        break;

                    case ArgumentException or FormatException:
                        logger.LogWarning(exception, "Bad request");
                        context.Response.StatusCode = StatusCodes.Status400BadRequest;
                        await context.Response.WriteAsJsonAsync(new
                        {
                            error = "Invalid request",
                            statusCode = StatusCodes.Status400BadRequest
                        });
                        break;

                    default:
                        logger.LogError(exception, "Unhandled exception");
                        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                        await context.Response.WriteAsJsonAsync(new
                        {
                            error = "An unexpected error occurred",
                            statusCode = StatusCodes.Status500InternalServerError
                        });
                        break;
                }
            });
        });
    }
}
