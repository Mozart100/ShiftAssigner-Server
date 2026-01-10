using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using ShiftAssignerServer.Controllers;
using ShiftAssignerServer.Requests;
using ShiftAssignerServer.Services;

namespace ShiftAssignerServer.Middleware;

/// <summary>
/// Middleware responsible for resolving tenant context from various sources:
/// - JWT token for authenticated requests
/// - Request body for tenant registration
/// - X-TenantId header as fallback
/// </summary>
public class TenantResolutionMiddleware
{
    public const string TenantContextKey = "TenantId";
    // private const string TenantIdHeaderName = "X-TenantId";
    private readonly JwtService _jwtService;

    private readonly RequestDelegate _next;

    public TenantResolutionMiddleware(RequestDelegate next, JwtService jwtService)
    {
        _next = next;
        _jwtService = jwtService;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var tenantId = await ResolveTenantIdAsync(context);
        context.Items[TenantContextKey] = tenantId;
        await _next(context);
    }

    private async Task<string> ResolveTenantIdAsync(HttpContext context)
    {
        var requestPath = context.Request.Path.ToString();
        Console.WriteLine($"ResolveTenantIdAsync: Processing path: {requestPath}");

        // Skip tenant resolution for Swagger endpoints
        if (requestPath.StartsWith("/swagger", StringComparison.OrdinalIgnoreCase) ||
            requestPath.StartsWith("/_framework", StringComparison.OrdinalIgnoreCase) ||
            requestPath.StartsWith("/_vs", StringComparison.OrdinalIgnoreCase) ||
            requestPath.StartsWith("/.well-known", StringComparison.OrdinalIgnoreCase))
        {
            Console.WriteLine($"ResolveTenantIdAsync: Skipping tenant resolution for system endpoint");
            return null; // No tenant context needed for these endpoints
        }

        // 1. Handle tenant registration endpoint - extract tenant from request body
        if (requestPath.StartsWith($"/api/v1/Auth/{AuthController.Register_Tenant}", StringComparison.OrdinalIgnoreCase))
        {
            Console.WriteLine($"ResolveTenantIdAsync: Handling tenant registration endpoint");
            var tenantId = await GetTenantFromRequestBodyAsync(context);
            return tenantId;
        }

        if (requestPath.StartsWith($"/api/v1/ShiftLeaders/{ShiftLeadersController.Login_EndPoint}", StringComparison.OrdinalIgnoreCase))
        {
            Console.WriteLine($"ResolveTenantIdAsync: Handling shift leader login endpoint");
            var tenantId = await GetTenantFromBodyAsync(context);
            return tenantId;
        }

        // 2. For authenticated endpoints, try to extract tenant from JWT token
        if (context.Request.Headers.ContainsKey("Authorization"))
        {
            Console.WriteLine($"ResolveTenantIdAsync: Found Authorization header, extracting from JWT");
            var tenantFromJwt = GetTenantFromJwtToken(context);
            if (!string.IsNullOrEmpty(tenantFromJwt))
            {
                Console.WriteLine($"ResolveTenantIdAsync: Successfully extracted tenant from JWT: {tenantFromJwt}");
                return tenantFromJwt;
            }
        }

        Console.WriteLine($"ResolveTenantIdAsync: Unable to resolve tenant context for path: {requestPath}");
        throw new BadHttpRequestException("Unable to resolve tenant context for the request.");
    }

    private async Task<string> GetTenantFromBodyAsync(HttpContext context)
    {
        try
        {
            var requestBody = await ReadRequestBodySafelyAsync(context);

            if (!string.IsNullOrEmpty(requestBody))
            {
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var loginRequest = JsonSerializer.Deserialize<LoginShiftLeaderRequest>(requestBody, options);

                if (loginRequest != null && !string.IsNullOrEmpty(loginRequest.TenantName))
                {
                    return loginRequest.TenantName;
                }
            }
        }
        catch (JsonException)
        {
            throw new BadHttpRequestException("Invalid JSON format in shift leader login request.");
        }
        catch (Exception ex) when (ex is not BadHttpRequestException)
        {
            throw new BadHttpRequestException("Error reading shift leader login request body.");
        }

        // If we reach here, tenant was not found or was empty
        throw new BadHttpRequestException("Shift leader login request must contain a valid 'TenantName' field in the request body.");
    }

    private string GetTenantFromJwtToken(HttpContext context)
    {
        try
        {
            var authHeader = context.Request.Headers["Authorization"].ToString();
            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
            {
                throw new UnauthorizedAccessException("Authorization header with Bearer token is required for authenticated endpoints.");
            }

            var token = authHeader.Substring("Bearer ".Length).Trim();
            
            // Use JwtService to parse the token and extract tenant
            var claims = _jwtService.ParseToken(token);
            return claims.Tenant;
        }
        catch
        {
            // If JWT parsing fails, return null to try other methods
            return null;
        }
    }

    private async Task<string> GetTenantFromRequestBodyAsync(HttpContext context)
    {
        try
        {
            var requestBody = await ReadRequestBodySafelyAsync(context);

            if (!string.IsNullOrEmpty(requestBody))
            {
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var tenantRequest = JsonSerializer.Deserialize<TenantRegisterRequest>(requestBody, options);

                if (tenantRequest != null && !string.IsNullOrEmpty(tenantRequest.Tenant))
                {
                    return tenantRequest.Tenant;
                }
            }
        }
        catch (JsonException)
        {
            throw new BadHttpRequestException("Invalid JSON format in tenant registration request.");
        }
        catch (Exception ex) when (ex is not BadHttpRequestException)
        {
            throw new BadHttpRequestException("Error reading tenant registration request body.");
        }

        // If we reach here, tenant was not found or was empty
        throw new BadHttpRequestException("Tenant registration request must contain a valid 'Tenant' field in the request body.");
    }

    /// <summary>
    /// Safely reads the request body while handling buffering and positioning correctly
    /// </summary>
    private async Task<string> ReadRequestBodySafelyAsync(HttpContext context)
    {
        // Enable buffering to allow reading the request body multiple times
        context.Request.EnableBuffering();

        // Only reset position if the stream is seekable
        if (context.Request.Body.CanSeek)
        {
            context.Request.Body.Position = 0;
        }

        // Use a StreamReader to read the request body
        using var reader = new StreamReader(context.Request.Body, System.Text.Encoding.UTF8, leaveOpen: true);
        var requestBody = await reader.ReadToEndAsync();
        
        // Reset position for subsequent reads (like controller model binding) if possible
        if (context.Request.Body.CanSeek)
        {
            context.Request.Body.Position = 0;
        }
        
        return requestBody;
    }
}