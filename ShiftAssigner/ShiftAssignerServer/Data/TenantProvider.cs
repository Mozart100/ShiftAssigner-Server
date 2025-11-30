using Microsoft.AspNetCore.Http;
using System;
using System.Linq;

namespace ShiftAssignerServer.Data
{
    public interface ITenantProvider
    {
        /// <summary>
        /// Raw tenant id as it comes from the X-TenantId header.
        /// </summary>
        string TenantId { get; }

        /// <summary>
        /// Sanitized tenant id that is safe to use as a PostgreSQL schema name.
        /// </summary>
        string TenantSchema { get; }
    }

    public sealed class TenantProvider : ITenantProvider
    {
        private const string TenantIdHeaderName = "X-TenantId";
        private const string Default_Schema = "default";
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
                    // Safe default for design-time – adjust as needed
                    return Default_Schema;
                }

                if (!httpContext.Request.Headers.TryGetValue(TenantIdHeaderName, out var values))
                {
                    throw new UnauthorizedAccessException($"Required header '{TenantIdHeaderName}' is missing.");
                }

                var tenantId = values.ToString().Trim();
                if (string.IsNullOrWhiteSpace(tenantId))
                {
                    throw new ArgumentException($"Header '{TenantIdHeaderName}' cannot be empty or whitespace.");
                }

                return tenantId;
            }
        }

        /// <summary>
        /// Schema name derived from TenantId, lowercased and cleaned for PostgreSQL.
        /// </summary>
        public string TenantSchema => SanitizeSchemaName(TenantId);

        private static string SanitizeSchemaName(string value)
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
}
