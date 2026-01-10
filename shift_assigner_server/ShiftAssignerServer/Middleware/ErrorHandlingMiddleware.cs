using System.Net;
using System.Text.Json;
using ShiftAssignerServer.Services.Validation;

namespace ShiftAssignerServer.Middleware;

/// <summary>
/// Global error handling middleware. Catches unhandled exceptions, logs them and
/// returns a stable error payload to the caller.
/// </summary>
public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlingMiddleware> _logger;
    private readonly IHostEnvironment _env;

    public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger, IHostEnvironment env)
    {
        _next = next;
        _logger = logger;
        _env = env;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (ShiftAssignmentException validationEx)
        {
            // Handle validation errors specifically
            _logger.LogWarning(validationEx, "Validation error occurred");

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;

            var errors = validationEx.ShiftAssignmentErrors
                .Select(e => new { Property = e.PropertyName, Error = e.ErrorMessage })
                .ToList();

            var response = new
            {
                Message = "Validation failed",
                Errors = errors
            };

            var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            var payload = JsonSerializer.Serialize(response, options);
            await context.Response.WriteAsync(payload);
        }
        catch (Exception ex)
        {
            // Log the error with stacktrace for diagnostics
            var errorId = Guid.NewGuid().ToString("N");
            _logger.LogError(ex, "🚨 UNHANDLED EXCEPTION (errorId={ErrorId}) - Path: {Path}, Method: {Method}", 
                errorId, context.Request.Path, context.Request.Method);

            // Add specific debugging for auth-related exceptions
            if (ex.Message.Contains("JWT") || ex.Message.Contains("token") || ex.Message.Contains("Authorization"))
            {
                _logger.LogError("🔑 AUTH-RELATED EXCEPTION: {Message}", ex.Message);
                var hasAuthHeader = context.Request.Headers.ContainsKey("Authorization");
                _logger.LogError("📋 Auth Header Present: {HasAuthHeader}", hasAuthHeader);
                if (hasAuthHeader)
                {
                    var authHeader = context.Request.Headers["Authorization"].ToString();
                    var headerStart = authHeader.Length > 20 ? authHeader.Substring(0, 20) + "..." : authHeader;
                    _logger.LogError("📋 Auth Header Value: {AuthHeader}", headerStart);
                }
            }

            // Prepare a safe response
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            var response = new
            {
                ErrorId = errorId,
                Message = "An unexpected error occurred.",
                // include details when in Development for easier debugging, otherwise omit
                Detail = _env.IsDevelopment() ? ex.ToString() : null as string
            };

            var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            var payload = JsonSerializer.Serialize(response, options);
            await context.Response.WriteAsync(payload);
        }
    }
}
