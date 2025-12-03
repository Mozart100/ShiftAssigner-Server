using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using ShiftAssignerServer.Middleware;

namespace ShiftAssignerServer.Data;

public interface ITenantProvider
{
    /// <summary>
    /// Raw tenant id as resolved by the TenantResolutionMiddleware.
    /// </summary>
    string TenantId { get; }

    /// <summary>
    /// Sanitized tenant id that is safe to use as a PostgreSQL schema name.
    /// </summary>
    string TenantSchema { get; }
}

public sealed class TenantProvider : ITenantProvider
{
    private const string DefaultSchema = "default";
    private readonly IHttpContextAccessor _httpContextAccessor;

    public TenantProvider(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string TenantId
    {
        get
        {
            var httpContext = _httpContextAccessor.HttpContext;

            // When running migrations / design-time there is no HttpContext
            if (httpContext is null)
            {
                return DefaultSchema;
            }

            // Get tenant ID resolved by TenantResolutionMiddleware
            if (httpContext.Items.TryGetValue(TenantResolutionMiddleware.TenantContextKey, out var tenantId))
            {
                return tenantId?.ToString() ?? DefaultSchema;
            }

            // Fallback to default if middleware hasn't run or didn't resolve tenant
            return DefaultSchema;
        }
    }

    /// <summary>
    /// Schema name derived from TenantId, lowercased and cleaned for PostgreSQL.
    /// </summary>
    public string TenantSchema => SanitizeSchemaName(TenantId);

    public static string SanitizeSchemaName(string value)
    {
        var cleaned = value
            .ToLowerInvariant()
            .Replace(" ", "_")
            .Replace("-", "_")
            .Replace(".", "_")
            .Where(c => char.IsLetterOrDigit(c) || c == '_')
            .Aggregate("", (current, c) => current + c);

        // PostgreSQL identifiers must not start with a digit
        if (string.IsNullOrEmpty(cleaned) || (!char.IsLetter(cleaned[0]) && cleaned[0] != '_'))
        {
            cleaned = "_" + cleaned;
        }

        return cleaned;
    }
}


public sealed class StaticTenantProvider : ITenantProvider
{
    public StaticTenantProvider(string schema)
    {
        TenantSchema = schema;
    }

    public string TenantSchema { get; }

    public string TenantId => TenantSchema;
}
