using Microsoft.AspNetCore.Builder;
using ShiftAssignerServer.Middleware;

namespace ShiftAssignerServer.Extensions;

public static class TenantResolutionMiddlewareExtensions
{
    /// <summary>
    /// Adds the tenant resolution middleware to the application pipeline.
    /// This middleware should be added early in the pipeline, before authentication.
    /// </summary>
    public static IApplicationBuilder UseTenantResolution(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<TenantResolutionMiddleware>();
    }
}