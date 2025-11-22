using System.Net;
using System.Text.Json;

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
        catch (Exception ex)
        {
            // Log the error with stacktrace for diagnostics
            var errorId = Guid.NewGuid().ToString("N");
            _logger.LogError(ex, "Unhandled exception (errorId={ErrorId})", errorId);

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
