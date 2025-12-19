using FluentValidation;
using ShiftAssignerServer.Requests;

namespace ShiftAssignerServer.Services.Validation;

/// <summary>
/// FluentValidation validator for WorkerAssigningToPeriodRequest.
/// Validates worker assignment to shift periods including dates and shift information.
/// </summary>
public class WorkerAssigningToPeriodRequestValidator : AbstractValidator<WorkerAssigningToPeriodRequest>
{
    public WorkerAssigningToPeriodRequestValidator()
    {
        RuleFor(x => x.Period)
            .NotNull().WithMessage("Period is required")
            .NotEmpty().WithMessage("Period must contain at least one day");

        RuleForEach(x => x.Period)
            .SetValidator(new WorkerAssigningDayScheduleValidator());

        RuleFor(x => x.Period)
            .Must(HaveUniqueDates).WithMessage("Duplicate dates found in period")
            .When(x => x.Period != null && x.Period.Any());
    }

    private bool HaveUniqueDates(List<WorkerAssigningToPeriodRequest.CreateDaySchedule> period)
    {
        if (period == null) return true;
        
        var dates = period.Select(d => d.Date).ToList();
        return dates.Count == dates.Distinct().Count();
    }
}

public class WorkerAssigningDayScheduleValidator : AbstractValidator<WorkerAssigningToPeriodRequest.CreateDaySchedule>
{
    public WorkerAssigningDayScheduleValidator()
    {
        RuleFor(x => x.Date)
            .NotEmpty().WithMessage("Date is required")
            .Must(BeValidDate).WithMessage("Date cannot be in the past");

        RuleFor(x => x.Shifts)
            .NotNull().WithMessage("Shifts are required")
            .NotEmpty().WithMessage("Each day must have at least one shift");

        RuleForEach(x => x.Shifts)
            .SetValidator(new WorkerAssigningShiftInfoValidator());

        RuleFor(x => x.Shifts)
            .Must(HaveUniqueShiftNames).WithMessage("Duplicate shift names found on the same day")
            .When(x => x.Shifts != null && x.Shifts.Any());
    }

    private bool BeValidDate(DateOnly date)
    {
        return true;
        // return date >= DateOnly.FromDateTime(DateTime.Now.Date);
    }

    private bool HaveUniqueShiftNames(List<WorkerAssigningToPeriodRequest.CreateShiftInfo> shifts)
    {
        if (shifts == null) return true;
        
        var shiftNames = shifts.Select(s => s.ShiftName?.ToLowerInvariant() ?? "").Where(s => !string.IsNullOrEmpty(s)).ToList();
        return shiftNames.Count == shiftNames.Distinct().Count();
    }
}

public class WorkerAssigningShiftInfoValidator : AbstractValidator<WorkerAssigningToPeriodRequest.CreateShiftInfo>
{
    public WorkerAssigningShiftInfoValidator()
    {
        RuleFor(x => x.ShiftName)
            .NotEmpty().WithMessage("Shift name is required")
            .MaximumLength(50).WithMessage("Shift name must not exceed 50 characters")
            .Matches(@"^[a-zA-Z0-9\s\-_]+$").WithMessage("Shift name can only contain letters, numbers, spaces, hyphens, and underscores");
    }
}