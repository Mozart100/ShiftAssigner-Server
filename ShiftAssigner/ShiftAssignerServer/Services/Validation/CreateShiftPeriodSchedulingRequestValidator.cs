using FluentValidation;
using ShiftAssignerServer.Requests;

namespace ShiftAssignerServer.Services.Validation;

/// <summary>
/// FluentValidation validator for CreateShiftPeriodSchedulingRequest.
/// Validates shift period scheduling data integrity including dates, shifts, and worker counts.
/// </summary>
public class CreateShiftPeriodSchedulingRequestValidator : AbstractValidator<CreateShiftPeriodSchedulingRequest>
{
    public CreateShiftPeriodSchedulingRequestValidator()
    {
        RuleFor(x => x.StartFrom)
            .NotEmpty().WithMessage("Start date is required")
            .Must(BeValidStartDate).WithMessage("Start date cannot be in the past");

        RuleFor(x => x.NextPeriod)
            .NotNull().WithMessage("Period is required")
            .NotEmpty().WithMessage("Period with at least one day is required");

        RuleForEach(x => x.NextPeriod)
            .SetValidator(new CreateDayScheduleValidator());

        RuleFor(x => x.NextPeriod)
            .Must(HaveUniqueDates).WithMessage("Duplicate dates found in period")
            .When(x => x.NextPeriod != null && x.NextPeriod.Any());

        RuleFor(x => x)
            .Must(HaveValidDateOrder).WithMessage("All days must be on or after the start date")
            .When(x => x.NextPeriod != null && x.NextPeriod.Any());
    }

    private bool BeValidStartDate(DateOnly startDate)
    {
        return true;
        // return startDate >= DateOnly.FromDateTime(DateTime.Now.Date);
    }

    private bool HaveUniqueDates(List<CreateShiftPeriodSchedulingRequest.CreateDaySchedule> period)
    {
        if (period == null) return true;
        
        var dates = period.Select(d => d.Date).ToList();
        return dates.Count == dates.Distinct().Count();
    }

    private bool HaveValidDateOrder(CreateShiftPeriodSchedulingRequest request)
    {
        if (request.NextPeriod == null) return true;
        
        return request.NextPeriod.All(day => day.Date >= request.StartFrom);
    }
}

public class CreateDayScheduleValidator : AbstractValidator<CreateShiftPeriodSchedulingRequest.CreateDaySchedule>
{
    public CreateDayScheduleValidator()
    {
        RuleFor(x => x.Date)
            .NotEmpty().WithMessage("Date is required");

        RuleFor(x => x.Shifts)
            .NotNull().WithMessage("Shifts are required")
            .NotEmpty().WithMessage("Each day must have at least one shift");

        RuleForEach(x => x.Shifts)
            .SetValidator(new CreateShiftInfoValidator());

        RuleFor(x => x.Shifts)
            .Must(HaveUniqueShiftNames).WithMessage("Duplicate shift names found on the same day")
            .When(x => x.Shifts != null && x.Shifts.Any());
    }

    private bool HaveUniqueShiftNames(List<CreateShiftPeriodSchedulingRequest.CreateShiftInfo> shifts)
    {
        if (shifts == null) return true;
        
        var shiftNames = shifts.Select(s => s.ShiftName?.ToLowerInvariant() ?? "").ToList();
        return shiftNames.Count == shiftNames.Distinct().Count();
    }
}

public class CreateShiftInfoValidator : AbstractValidator<CreateShiftPeriodSchedulingRequest.CreateShiftInfo>
{
    public CreateShiftInfoValidator()
    {
        RuleFor(x => x.ShiftName)
            .NotEmpty().WithMessage("Shift name is required")
            .MaximumLength(50).WithMessage("Shift name must not exceed 50 characters")
            .Matches(@"^[a-zA-Z0-9\s\-_]+$").WithMessage("Shift name can only contain letters, numbers, spaces, hyphens, and underscores");

        RuleFor(x => x.AmountOfWorkers)
            .GreaterThan(0).WithMessage("Amount of workers must be greater than 0")
            .LessThanOrEqualTo(50).WithMessage("Amount of workers cannot exceed 50 per shift");
    }
}