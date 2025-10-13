using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ProjForum.Identity.Domain.Exceptions;

namespace ProjForum.Identity.Api.ExceptionHandlers;

public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var problemDetails = new ProblemDetails
        {
            Instance = httpContext.Request.Path
        };

        httpContext.Response.ContentType = "application/json";

        switch (exception)
        {
            case FluentValidation.ValidationException  fluentException:
                problemDetails.Title = "one or more validation errors occurred.";
                problemDetails.Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1";
                httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
                var validationErrors = fluentException.Errors.Select(error => error.ErrorMessage).ToList();
                problemDetails.Extensions.Add("errors", validationErrors);
                logger.LogError("Validation errors occurred: {ValidationErrors}", string.Join(", ", validationErrors));
                break;

            case NotFoundException:
                problemDetails.Title = exception.Message;
                httpContext.Response.StatusCode = StatusCodes.Status404NotFound;
                break;

            case UnauthorizedException:
                problemDetails.Title = exception.Message;
                httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
                break;

            case ConflictException:
                problemDetails.Title = exception.Message;
                httpContext.Response.StatusCode = StatusCodes.Status409Conflict;
                break;

            default:
                problemDetails.Title = "An unexpected error occurred.";
                httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
                break;
        }

        problemDetails.Status = httpContext.Response.StatusCode;

        logger.LogError("{ProblemDetailsTitle}", problemDetails.Title);
        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken).ConfigureAwait(false);
        return true;
    }
}