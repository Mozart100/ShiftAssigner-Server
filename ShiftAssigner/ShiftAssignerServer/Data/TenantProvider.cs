using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using ShiftAssignerServer.Controllers;
using ShiftAssignerServer.Requests;

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

                // Check if request is from AuthController - these endpoints don't require X-TenantId header
                var requestPath = httpContext.Request.Path.ToString();
                if (requestPath.StartsWith($"/api/v1/Auth/{AuthController.Register_Tenant}", StringComparison.OrdinalIgnoreCase))
                {
                    // Get the tenant from the request body for tenant registration endpoint
                    var tenantRequest = GetTenantFromRequestBodyAsync(httpContext).GetAwaiter().GetResult();
                    if (tenantRequest != null && !string.IsNullOrEmpty(tenantRequest.Tenant))
                    {
                        return SanitizeSchemaName(tenantRequest.Tenant);
                    }
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

        private static async Task<TenantRegisterRequest> GetTenantFromRequestBodyAsync(HttpContext httpContext)
        {
            try
            {
                // Enable buffering to allow reading the request body multiple times
                httpContext.Request.EnableBuffering();
                
                // Reset position to beginning
                httpContext.Request.Body.Position = 0;
                
                using (var reader = new StreamReader(httpContext.Request.Body, leaveOpen: true))
                {
                    var requestBody = await reader.ReadToEndAsync();
                    
                    // Reset position for the controller to read it again
                    httpContext.Request.Body.Position = 0;
                    
                    if (!string.IsNullOrEmpty(requestBody))
                    {
                        var options = new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        };
                        
                        return JsonSerializer.Deserialize<TenantRegisterRequest>(requestBody, options);
                    }
                }
            }
            catch (Exception ex)
            {
                // If we can't parse the request body, return null and use default schema
                return null;
            }
            
            return null;
        }
    }
}
