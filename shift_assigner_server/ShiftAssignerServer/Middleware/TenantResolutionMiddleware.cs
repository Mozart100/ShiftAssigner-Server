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

        // 1. Handle tenant registration endpoint - extract tenant from request body
        if (requestPath.StartsWith($"/api/v1/Auth/{AuthController.Register_Tenant}", StringComparison.OrdinalIgnoreCase))
        {
            var tenantId = await GetTenantFromRequestBodyAsync(context);
            return tenantId;
        }

        if (requestPath.StartsWith($"/api/v1/ShiftLeaders/{ShiftLeadersController.Login_EndPoint}", StringComparison.OrdinalIgnoreCase))
        {
            var tenantId = await GetTenantFromBodyAsync(context);
            return tenantId;
        }

        // 2. For authenticated endpoints, try to extract tenant from JWT token
        if (context.Request.Headers.ContainsKey("Authorization"))
        {
            var tenantFromJwt = GetTenantFromJwtToken(context);
            if (!string.IsNullOrEmpty(tenantFromJwt))
            {
                return tenantFromJwt;
            }
        }

        throw new BadHttpRequestException("Unable to resolve tenant context for the request.");
    }

    private async Task<string> GetTenantFromBodyAsync(HttpContext context)
    {
        try
        {
            // Enable buffering to allow reading the request body multiple times
            context.Request.EnableBuffering();

            // Reset position to beginning
            context.Request.Body.Position = 0;

            using (var reader = new StreamReader(context.Request.Body, leaveOpen: true))
            {
                var requestBody = await reader.ReadToEndAsync();

                // Reset position for the controller to read it again
                context.Request.Body.Position = 0;

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
        }
        catch (JsonException)
        {
            throw new BadHttpRequestException("Invalid JSON format in shift leader login request.");
        }
        catch (Exception)
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
            // Enable buffering to allow reading the request body multiple times
            context.Request.EnableBuffering();

            // Reset position to beginning
            context.Request.Body.Position = 0;

            using (var reader = new StreamReader(context.Request.Body, leaveOpen: true))
            {
                var requestBody = await reader.ReadToEndAsync();

                // Reset position for the controller to read it again
                context.Request.Body.Position = 0;

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
        }
        catch (JsonException)
        {
            throw new BadHttpRequestException("Invalid JSON format in tenant registration request.");
        }
        catch (Exception)
        {
            throw new BadHttpRequestException("Error reading tenant registration request body.");
        }

        // If we reach here, tenant was not found or was empty
        throw new BadHttpRequestException("Tenant registration request must contain a valid 'Tenant' field in the request body.");
    }
}