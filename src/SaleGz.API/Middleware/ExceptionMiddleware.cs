using SaleGz.Application.Common.Exceptions;
using System.Net;
using System.Text.Json;

namespace SaleGz.API.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        _next   = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error no controlado: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var (statusCode, response) = exception switch
        {
            NotFoundException ex => (
                HttpStatusCode.NotFound,
                new ApiErrorResponse(404, ex.Message)
            ),
            BusinessException ex => (
                HttpStatusCode.BadRequest,
                new ApiErrorResponse(400, ex.Message)
            ),
            Application.Common.Exceptions.ValidationException ex => (
                HttpStatusCode.UnprocessableEntity,
                new ApiErrorResponse(422, "Error de validación", ex.Errors)
            ),
            ForbiddenException ex => (
                HttpStatusCode.Forbidden,
                new ApiErrorResponse(403, ex.Message)
            ),
            _ => (
                HttpStatusCode.InternalServerError,
                new ApiErrorResponse(500, exception.Message + " | " + exception.InnerException?.Message)
            )
        };

        context.Response.StatusCode = (int)statusCode;

        var json = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        return context.Response.WriteAsync(json);
    }
}

public record ApiErrorResponse(
    int StatusCode,
    string Message,
    IDictionary<string, string[]>? Errors = null
);
