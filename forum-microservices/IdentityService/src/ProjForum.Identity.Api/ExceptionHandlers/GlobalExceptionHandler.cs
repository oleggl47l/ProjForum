using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ProjForum.BuildingBlocks.Domain;
using ProjForum.Identity.Domain.Exceptions;
using ValidationException = FluentValidation.ValidationException;

namespace ProjForum.Identity.Api.ExceptionHandlers;

public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception,
        CancellationToken cancellationToken)
    {
        var (statusCode, title) = exception switch
        {
            ValidationException => (StatusCodes.Status400BadRequest, "Validation failed"),
            ArgumentException => (StatusCodes.Status400BadRequest, "Invalid input"),
            FormatException => (StatusCodes.Status400BadRequest, "Invalid format"),
            InvalidDataException => (StatusCodes.Status400BadRequest, "Invalid data"),
            NotSupportedException => (StatusCodes.Status400BadRequest, "Operation not supported"),
            InvalidOperationException => (StatusCodes.Status400BadRequest, "Operation is not valid"),
            DomainException => (StatusCodes.Status400BadRequest, "Domain rule violated"),
            UnauthorizedAccessException => (StatusCodes.Status401Unauthorized, "Unauthorized"),
            System.Security.Authentication.AuthenticationException => (StatusCodes.Status401Unauthorized,
                "Authentication failed"),
            AccessViolationException => (StatusCodes.Status403Forbidden, "Forbidden"),
            KeyNotFoundException => (StatusCodes.Status404NotFound, "Resource not found"),
            TimeoutException or TaskCanceledException => (StatusCodes.Status408RequestTimeout, "Request timed out"),
            ConflictException => (StatusCodes.Status409Conflict, "Conflict detected"),
            IOException => (StatusCodes.Status500InternalServerError, "I/O error occurred"),
            _ => (StatusCodes.Status500InternalServerError, "An unexpected error occurred")
        };

        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = exception is ValidationException ? null : exception.Message,
            Instance = httpContext.Request.Path
        };

        if (exception is ValidationException validationException)
        {
            problemDetails.Extensions["errors"] = validationException.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(e => e.ErrorMessage).ToArray()
                );
        }

        logger.Log(LogLevelFromStatus(statusCode), exception, "Unhandled exception");

        httpContext.Response.StatusCode = statusCode;
        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);
        return true;
    }

    private static LogLevel LogLevelFromStatus(int status) =>
        status >= 500 ? LogLevel.Error : LogLevel.Warning;
}