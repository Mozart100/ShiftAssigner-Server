using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using ShiftAssignerServer.Services.Validation;

namespace ShiftAssignerServer.Controllers.Attributes;

/// <summary>
/// Action filter attribute that validates model state and returns standardized error responses.
/// Integrates with the existing ShiftAssignmentError pattern used throughout the application.
/// </summary>
public class ValidateModelAttribute : ActionFilterAttribute
{
    private readonly bool _returnDetailedErrors;

    /// <summary>
    /// Initializes a new instance of the ValidateModelAttribute.
    /// </summary>
    /// <param name="returnDetailedErrors">Whether to return detailed validation errors or a simple message</param>
    public ValidateModelAttribute(bool returnDetailedErrors = true)
    {
        _returnDetailedErrors = returnDetailedErrors;
    }

    public override void OnActionExecuting(ActionExecutingContext context)
    {
        // Check if model state is valid
        if (!context.ModelState.IsValid)
        {
            if (_returnDetailedErrors)
            {
                // Convert ModelState errors to ShiftAssignmentError format for consistency
                var errors = context.ModelState
                    .Where(x => x.Value?.Errors.Count > 0)
                    .SelectMany(x => x.Value.Errors.Select(e => 
                        new ShiftAssignmentError(x.Key, e.ErrorMessage)))
                    .ToList();

                // Create detailed error response matching the project's error pattern
                var errorResponse = new
                {
                    Success = false,
                    Message = string.Join("; ", errors.Select(e => e.ErrorMessage)),
                    ValidationErrors = errors.Select(e => new 
                    {
                        Field = e.PropertyName,
                        Error = e.ErrorMessage
                    }).ToList()
                };

                context.Result = new BadRequestObjectResult(errorResponse);
            }
            else
            {
                // Simple error response
                var errorResponse = new
                {
                    Success = false,
                    Message = "Invalid request data"
                };

                context.Result = new BadRequestObjectResult(errorResponse);
            }
        }

        base.OnActionExecuting(context);
    }
}