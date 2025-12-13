using FluentValidation;
using ShiftAssignerServer.Requests;

namespace ShiftAssignerServer.Services.Validation;

/// <summary>
/// FluentValidation validator for WorkerRegisteringRequest.
/// Validates worker registration data integrity including personal information.
/// </summary>
public class WorkerRegisteringRequestValidator : AbstractValidator<WorkerRegisteringRequest>
{
    public WorkerRegisteringRequestValidator()
    {
        RuleFor(x => x.ID)
            .NotEmpty().WithMessage("Worker ID is required")
            .MinimumLength(3).WithMessage("Worker ID must be at least 3 characters")
            .MaximumLength(100).WithMessage("Worker ID must not exceed 100 characters")
            .Matches(@"^[a-zA-Z0-9_-]+$").WithMessage("Worker ID can only contain letters, numbers, hyphens, and underscores");

        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("FirstName is required")
            .MaximumLength(50).WithMessage("FirstName must not exceed 50 characters")
            .Matches(@"^[a-zA-Z\s\-'\.]+$").WithMessage("FirstName can only contain letters, spaces, hyphens, apostrophes, and periods");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("LastName is required")
            .MaximumLength(50).WithMessage("LastName must not exceed 50 characters")
            .Matches(@"^[a-zA-Z\s\-'\.]+$").WithMessage("LastName can only contain letters, spaces, hyphens, apostrophes, and periods");

        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage("PhoneNumber is required")
            .Matches(@"^\+?[\d\s\-\(\)]+$").WithMessage("PhoneNumber must contain only digits, spaces, hyphens, parentheses, or a leading plus sign")
            .MinimumLength(10).WithMessage("PhoneNumber must be at least 10 characters")
            .MaximumLength(20).WithMessage("PhoneNumber must not exceed 20 characters");

        RuleFor(x => x.DateOfBirth)
            .NotEmpty().WithMessage("DateOfBirth is required")
            .Must(BeValidWorkerAge).WithMessage("Worker must be at least 16 years old and not older than 70 years");
    }

    private bool BeValidWorkerAge(DateOnly dateOfBirth)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var age = today.Year - dateOfBirth.Year;
        if (dateOfBirth > today.AddYears(-age)) age--;

        // Workers typically have a different age range than general users
        return age >= 16 && age <= 70;
    }
}