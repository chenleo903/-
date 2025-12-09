using System.Text.Json;
using CrmSystem.Api.DTOs;
using CrmSystem.Api.Exceptions;

namespace CrmSystem.Api.Middleware;

/// <summary>
/// 全局异常处理中间件
/// 捕获所有异常并转换为统一的 ApiResponse 格式
/// </summary>
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;
    private readonly JsonSerializerOptions _jsonOptions;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var response = context.Response;
        response.ContentType = "application/json";

        var apiResponse = new ApiResponse<object>
        {
            Success = false,
            Data = null,
            Errors = new List<ErrorDetail>()
        };

        switch (exception)
        {
            case ValidationException validationEx:
                response.StatusCode = StatusCodes.Status400BadRequest;
                foreach (var error in validationEx.Errors)
                {
                    apiResponse.Errors.Add(new ErrorDetail
                    {
                        Field = error.Key,
                        Message = error.Value
                    });
                }
                _logger.LogWarning("Validation error: {Errors}", validationEx.Errors);
                break;

            case NotFoundException notFoundEx:
                response.StatusCode = StatusCodes.Status404NotFound;
                apiResponse.Errors.Add(new ErrorDetail
                {
                    Message = notFoundEx.Message
                });
                _logger.LogWarning("Resource not found: {Message}", notFoundEx.Message);
                break;

            case ConflictException conflictEx:
                response.StatusCode = StatusCodes.Status409Conflict;
                apiResponse.Errors.Add(new ErrorDetail
                {
                    Message = conflictEx.Message
                });
                _logger.LogWarning("Conflict error: {Message}", conflictEx.Message);
                break;

            case ConcurrencyException concurrencyEx:
                response.StatusCode = StatusCodes.Status409Conflict;
                apiResponse.Errors.Add(new ErrorDetail
                {
                    Message = concurrencyEx.Message,
                    CurrentValue = concurrencyEx.CurrentVersion.ToString("yyyy-MM-ddTHH:mm:ss.fffZ")
                });
                _logger.LogWarning(
                    "Concurrency conflict: {Message}, CurrentVersion: {CurrentVersion}",
                    concurrencyEx.Message,
                    concurrencyEx.CurrentVersion);
                break;

            default:
                response.StatusCode = StatusCodes.Status500InternalServerError;
                apiResponse.Errors.Add(new ErrorDetail
                {
                    Message = "An internal server error occurred"
                });
                // Log full exception details for server errors
                _logger.LogError(
                    exception,
                    "Unhandled exception: {ExceptionType} - {Message}",
                    exception.GetType().Name,
                    exception.Message);
                break;
        }

        var result = JsonSerializer.Serialize(apiResponse, _jsonOptions);
        await response.WriteAsync(result);
    }
}

/// <summary>
/// Extension method to register the exception handling middleware
/// </summary>
public static class ExceptionHandlingMiddlewareExtensions
{
    public static IApplicationBuilder UseExceptionHandling(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<ExceptionHandlingMiddleware>();
    }
}
