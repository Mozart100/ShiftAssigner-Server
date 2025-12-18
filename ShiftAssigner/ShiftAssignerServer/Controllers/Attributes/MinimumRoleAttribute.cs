using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using ShiftAssignerServer.Models;

namespace ShiftAssignerServer.Controllers.Attributes;

/// <summary>
/// Action filter attribute that validates minimum role requirements for accessing endpoints.
/// Automatically extracts role information from JWT token and validates access.
/// </summary>
public class MinimumRoleAttribute : ActionFilterAttribute
{
    private readonly RoleState _minimumRole;

    /// <summary>
    /// Initializes a new instance of the MinimumRoleAttribute.
    /// </summary>
    /// <param name="minimumRole">The minimum role required to access the endpoint</param>
    public MinimumRoleAttribute(RoleState minimumRole)
    {
        _minimumRole = minimumRole;
    }

    public override void OnActionExecuting(ActionExecutingContext context)
    {
        // Get the controller instance to access TryGetShiftLeaderInfo method
        if (context.Controller is TenantControllerBase controller)
        {
            // Extract role information from JWT token
            if (!controller.TryGetShiftLeaderInfo(out string? shiftLeaderId, out RoleState? role))
            {
                context.Result = new UnauthorizedObjectResult(new
                {
                    Success = false,
                    Message = "Valid authentication required"
                });
                return;
            }

            // Check if user has the minimum required role
            if (!HasMinimumRole(role, _minimumRole))
            {
                context.Result = new ObjectResult(new
                {
                    Success = false,
                    Message = $"Access denied. Minimum role required: {_minimumRole}, Current role: {role}"
                })
                {
                    StatusCode = 403 // Forbidden
                };
                return;
            }

            // Store role information in HttpContext for use in the action
            context.HttpContext.Items["ShiftLeaderId"] = shiftLeaderId;
            context.HttpContext.Items["UserRole"] = role;
        }
        else
        {
            // Controller doesn't inherit from TenantControllerBase
            context.Result = new UnauthorizedObjectResult(new
            {
                Success = false,
                Message = "Authentication not supported for this controller"
            });
            return;
        }

        base.OnActionExecuting(context);
    }

    /// <summary>
    /// Determines if the current role meets the minimum role requirement.
    /// Role hierarchy: Worker < ShiftLeader < BossTenant
    /// </summary>
    /// <param name="currentRole">The user's current role</param>
    /// <param name="minimumRole">The minimum required role</param>
    /// <returns>True if the user has sufficient privileges</returns>
    private static bool HasMinimumRole(RoleState? currentRole, RoleState minimumRole)
    {
        if (currentRole == null) return false;

        // Define role hierarchy (higher number = higher privilege)
        var roleHierarchy = new Dictionary<RoleState, int>
        {
            { RoleState.Worker, 1 },
            { RoleState.ShiftLeader, 2 },
            { RoleState.Boss, 3 }
        };

        return roleHierarchy.TryGetValue(currentRole.Value, out int currentLevel) &&
               roleHierarchy.TryGetValue(minimumRole, out int requiredLevel) &&
               currentLevel >= requiredLevel;
    }
}

/// <summary>
/// Extension methods for accessing role information stored by MinimumRoleAttribute
/// </summary>
public static class HttpContextRoleExtensions
{
    /// <summary>
    /// Gets the shift leader ID from the current HTTP context
    /// </summary>
    public static string? GetShiftLeaderId(this HttpContext context)
    {
        return context.Items["ShiftLeaderId"] as string;
    }

    /// <summary>
    /// Gets the user role from the current HTTP context
    /// </summary>
    public static RoleState? GetUserRole(this HttpContext context)
    {
        return context.Items["UserRole"] as RoleState?;
    }
}