using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using ShiftAssignerServer.Models;

namespace ShiftAssignerServer.Controllers.Attributes;

/// <summary>
/// Authorization attribute that ensures the authenticated user has exactly the specified role.
/// Unlike MinimumRole, this attribute only allows the exact role specified (no role hierarchy).
/// </summary>
public class OnlyRoleAttribute : ActionFilterAttribute
{
    private readonly RoleState _requiredRole;

    /// <summary>
    /// Initializes a new instance of the OnlyRoleAttribute.
    /// </summary>
    /// <param name="requiredRole">The exact role required to access the endpoint</param>
    public OnlyRoleAttribute(RoleState requiredRole)
    {
        _requiredRole = requiredRole;
    }

    public override void OnActionExecuting(ActionExecutingContext context)
    {
        // Get the controller (must inherit from TenantControllerBase)
        if (context.Controller is not TenantControllerBase controller)
        {
            context.Result = new StatusCodeResult(500); // Internal server error
            return;
        }

        // Try to get the current user's role information
        if (!controller.TryGetPersonInfo(out string? userId, out RoleState? userRole) || 
            userRole == null)
        {
            context.Result = new UnauthorizedObjectResult(new
            {
                Success = false,
                Message = "Valid authentication required"
            });
            return;
        }

        // Check if user's role exactly matches the required role
        if (userRole.Value != _requiredRole)
        {
            var errorMessage = $"Access denied. Required role: {_requiredRole}, but user has: {userRole}";
            context.Result = new ObjectResult(new
            {
                Success = false,
                Message = errorMessage
            })
            {
                StatusCode = 403 // Forbidden
            };
            return;
        }

        // Store role information in HttpContext for use in the action
        context.HttpContext.Items["UserId"] = userId;
        context.HttpContext.Items["UserRole"] = userRole;

        base.OnActionExecuting(context);
    }
}