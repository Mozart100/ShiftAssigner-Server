using ShiftAssignerServer.Repositories;

namespace ShiftAssignerServer.Middleware;

/// <summary>
/// Middleware that automatically saves any pending database changes at the end of each request
/// </summary>
public class AutoSaveMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<AutoSaveMiddleware> _logger;

    public AutoSaveMiddleware(RequestDelegate next, ILogger<AutoSaveMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            // Process the request
            await _next(context);

            // Auto-save any pending changes after successful request processing
            var unitOfWork = context.RequestServices.GetService<ITenantUnitOfWork>();
            if (unitOfWork != null)
            {
                var changesSaved = await unitOfWork.AutoSaveIfChangesAsync();
                if (changesSaved)
                {
                    _logger.LogInformation("Auto-saved database changes for request {RequestPath}", context.Request.Path);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during request processing for {RequestPath}", context.Request.Path);
            throw;
        }
    }
}

/// <summary>
/// Extension method to register the AutoSave middleware
/// </summary>
public static class AutoSaveMiddlewareExtensions
{
    public static IApplicationBuilder UseAutoSave(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<AutoSaveMiddleware>();
    }
}