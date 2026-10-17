using Microsoft.EntityFrameworkCore;
using Npgsql;
using UniversityCourseManagementApi.Common;

namespace UniversityCourseManagementApi.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(
        RequestDelegate next,
        ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (AppException ex)
        {
            _logger.LogWarning(
                "Business error {Code}: {Message}",
                ex.ErrorCode, ex.Message);

            await WriteErrorAsync(
                context, ex.StatusCode,
                ex.ErrorCode, ex.Message);
        }
        catch (DbUpdateException ex)
            when (ex.InnerException is PostgresException
            { SqlState: PostgresErrorCodes.UniqueViolation })
        {
            _logger.LogWarning(ex, "Database unique conflict");

            await WriteErrorAsync(
                context, 409, "DUPLICATE_VALUE",
                "A record with these values already exists");
        }
        catch (DbUpdateException ex)
            when (ex.InnerException is PostgresException
            { SqlState: PostgresErrorCodes.ForeignKeyViolation })
        {
            _logger.LogWarning(ex, "Database relation conflict");

            await WriteErrorAsync(
                context, 409, "RELATION_CONFLICT",
                "Operation conflicts with related records");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected application error");

            await WriteErrorAsync(
                context, 500, "INTERNAL_SERVER_ERROR",
                "An unexpected error occurred");
        }
    }

    private static async Task WriteErrorAsync(
        HttpContext context,
        int statusCode,
        string errorCode,
        string message)
    {
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";

        var result = ReturnResult<object>.Failure(
            statusCode,
            errorCode,
            message,
            context.TraceIdentifier);

        await context.Response.WriteAsJsonAsync(result);
    }
}