using FluentValidation;
using ShiftAssignerServer.Requests;

namespace ShiftAssignerServer.Services.Validation;

/// <summary>
/// FluentValidation validator for RegisterRequest.
/// Validates registration data integrity for workers, shift leaders, and boss tenants.
/// </summary>
public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidator()
    {
        RuleFor(x => x.ID)
            .NotEmpty().WithMessage("ID is required")
            .MinimumLength(3).WithMessage("ID must be at least 3 characters")
            .MaximumLength(100).WithMessage("ID must not exceed 100 characters");

        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("FirstName is required")
            .MaximumLength(50).WithMessage("FirstName must not exceed 50 characters");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("LastName is required")
            .MaximumLength(50).WithMessage("LastName must not exceed 50 characters");

        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage("PhoneNumber is required")
            .Matches(@"^\+?[\d\s\-\(\)]+$").WithMessage("PhoneNumber must contain only digits, spaces, hyphens, parentheses, or a leading plus sign")
            .MaximumLength(20).WithMessage("PhoneNumber must not exceed 20 characters");

        RuleFor(x => x.DateOfBirth)
            .NotEmpty().WithMessage("DateOfBirth is required")
            .Must(BeValidAge).WithMessage("Person must be at least 16 years old and not older than 100 years");

        RuleFor(x => x.PasswordHash)
            .NotEmpty().WithMessage("Password is required")
            .MinimumLength(6).WithMessage("Password must be at least 6 characters");

        // ShiftLeaderId is optional - it's only required when registering a worker
        RuleFor(x => x.ShiftLeaderId)
            .MaximumLength(100).WithMessage("ShiftLeaderId must not exceed 100 characters");
    }

    private bool BeValidAge(DateOnly dateOfBirth)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var age = today.Year - dateOfBirth.Year;
        if (dateOfBirth > today.AddYears(-age)) age--;

        return age >= 16 && age <= 100;
    }
}
