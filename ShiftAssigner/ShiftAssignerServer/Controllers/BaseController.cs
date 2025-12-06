using System;
using Microsoft.AspNetCore.Mvc;
using ShiftAssignerServer.Middleware;
using ShiftAssignerServer.Models;
using ShiftAssignerServer.Services;

namespace ShiftAssignerServer.Controllers;

/// <summary>
/// Base controller providing common functionality for all API controllers.
/// Handles tenant resolution and other cross-cutting concerns.
/// </summary>
public abstract class BaseController : ControllerBase
{
    protected readonly JwtService _jwtService;
    
    protected BaseController(JwtService jwtService)
    {
        _jwtService = jwtService;
    }
    /// <summary>
    /// Gets the current tenant from the TenantResolutionMiddleware.
    /// Returns null if the request is in the main schema context.
    /// Returns the tenant name for tenant-specific schema contexts.
    /// </summary>
    /// <returns>The tenant name or null for main schema</returns>
    protected string? GetTenant()
    {
        return HttpContext.Items[TenantResolutionMiddleware.TenantContextKey]?.ToString();
    }

    /// <summary>
    /// Gets the current tenant with a fallback to empty string.
    /// Use this when you need a non-null value for string operations.
    /// </summary>
    /// <returns>The tenant name or empty string</returns>
    protected string GetTenantOrEmpty()
    {
        return GetTenant() ?? string.Empty;
    }

    /// <summary>
    /// Tries to get the current tenant from the TenantResolutionMiddleware.
    /// </summary>
    /// <param name="tenant">When this method returns, contains the tenant name if found; otherwise, null.</param>
    /// <returns>True if a tenant context exists, false if in main schema</returns>
    protected bool TryGetTenant(out string? tenant)
    {
        tenant = GetTenant();
        return !string.IsNullOrEmpty(tenant);
    }

    /// <summary>
    /// Tries to get the current shift leader ID and role from the JWT token.
    /// </summary>
    /// <param name="shiftLeaderId">When this method returns, contains the shift leader ID if found; otherwise, null.</param>
    /// <param name="role">When this method returns, contains the user role enum if found; otherwise, null.</param>
    /// <returns>True if both shift leader ID and role exist in the token</returns>
    protected bool TryGetShiftLeaderInfo(out string? shiftLeaderId, out RoleState? role)
    {
        shiftLeaderId = null;
        role = null;
        
        try
        {
            var authHeader = HttpContext.Request.Headers["Authorization"].ToString();
            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
            {
                return false;
            }

            var token = authHeader.Substring("Bearer ".Length).Trim();
            var claims = _jwtService.ParseToken(token);
            
            shiftLeaderId = claims.UserId;
            
            if (!string.IsNullOrEmpty(claims.Role) && Enum.TryParse<RoleState>(claims.Role, out var parsedRole))
            {
                role = parsedRole;
            }
            
            return !string.IsNullOrEmpty(shiftLeaderId) && role.HasValue;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Checks if the current request is operating in a tenant-specific context.
    /// </summary>
    /// <returns>True if in tenant context, false if in main schema</returns>
    protected bool IsInTenantContext()
    {
        return !string.IsNullOrEmpty(GetTenant());
    }
}