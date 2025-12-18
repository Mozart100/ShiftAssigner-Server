using FluentValidation;
using ShiftAssignerServer.Repositories;
using ShiftAssignerServer.Requests;

namespace ShiftAssignerServer.Services.Validation;

public interface IWorkerSchedulerValidationService
{
    Task ValidateCreateShiftPeriodRequestAsync(CreateShiftPeriodSchedulingRequest request, string shiftLeaderId);
}

public class WorkerSchedulerValidationService : ServiceValidatorBase, IWorkerSchedulerValidationService
{
    private readonly ITenantUnitOfWork _tenantUnitOfWork;
    private readonly IValidator<CreateShiftPeriodSchedulingRequest> _validator;

    public WorkerSchedulerValidationService(
        ITenantUnitOfWork tenantUnitOfWork,
        IValidator<CreateShiftPeriodSchedulingRequest> validator,
        ILogger<WorkerSchedulerValidationService> logger)
        : base(logger)
    {
        _tenantUnitOfWork = tenantUnitOfWork;
        _validator = validator;
    }

    public async Task ValidateCreateShiftPeriodRequestAsync(CreateShiftPeriodSchedulingRequest request, string shiftLeaderId)
    {
        var errors = new List<ShiftAssignmentError>();

        // Get active tenant shift configuration to validate against min/max worker limits
        var activeConfig = await _tenantUnitOfWork.TenantShiftSchedulingRepository.FirstOrDefaultAsync(x => x.IsActive);

        if (request is null)
        {
            errors.Add(new ShiftAssignmentError("request", "Create shift period request is required"));
            Validate(errors);
        }

        // Check if tenant has active shift configuration
        if (activeConfig == null)
        {
            errors.Add(new ShiftAssignmentError("tenantConfiguration", "No active shift configuration found for this tenant"));
            Validate(errors);
        }

        // Validate shift leader ID
        if (shiftLeaderId.IsEmpty())
        {
            errors.Add(new ShiftAssignmentError("shiftLeaderId", "Shift leader ID is required"));
            Validate(errors);
        }

        // Use FluentValidation validator for request validation
        var validationResult = await _validator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            errors.AddRange(Dissect(validationResult));
            Validate(errors);
        }

        if (request.NextPeriod.IsEmpty())
        {
            errors.Add(new ShiftAssignmentError($"{nameof(CreateShiftPeriodSchedulingRequest.NextPeriod)}", $"No active shift configuration found for {nameof(CreateShiftPeriodSchedulingRequest)}"));
            Validate(errors);
        }


        // Validate worker counts against tenant shift configuration
        foreach (var day in request.NextPeriod)
        {
            foreach (var requestShift in day.Shifts)
            {
                // Find matching shift configuration by name
                var shiftConfig = activeConfig.Shifts.FirstOrDefault(s =>
                    s.ShiftName.Equals(requestShift.ShiftName, StringComparison.OrdinalIgnoreCase));

                if (shiftConfig is not null)
                {
                    // Check minimum worker requirement
                    if (requestShift.AmountOfWorkers < shiftConfig.MinimumAmountOfWorkers)
                    {
                        errors.Add(new ShiftAssignmentError(
                            $"{nameof(request.NextPeriod)}.Shifts.AmountOfWorkers",
                            $"Shift '{requestShift.ShiftName}' on {day.Date} requires at least {shiftConfig.MinimumAmountOfWorkers} workers, but only {requestShift.AmountOfWorkers} were assigned"));
                    }

                    // Check maximum worker limit
                    if (requestShift.AmountOfWorkers > shiftConfig.MaximumAmountOfWorkers)
                    {
                        errors.Add(new ShiftAssignmentError(
                            $"{nameof(request.NextPeriod)}.Shifts.AmountOfWorkers",
                            $"Shift '{requestShift.ShiftName}' on {day.Date} cannot exceed {shiftConfig.MaximumAmountOfWorkers} workers, but {requestShift.AmountOfWorkers} were assigned"));
                    }
                }
                else
                {
                    // Shift name not found in configuration
                    errors.Add(new ShiftAssignmentError(
                        $"{nameof(request.NextPeriod)}.Shifts.ShiftName",
                        $"Shift '{requestShift.ShiftName}' is not configured for this tenant. Available shifts: {string.Join(", ", activeConfig.Shifts.Select(s => s.ShiftName))}"));
                }
            }
        }

        if (errors.IsEmpty())
        {
            var existingPeriods = await _tenantUnitOfWork.ShiftPeriodSchedulingRepository.GetAllAsync(x => x.IsActive && x.ShiftLeaderId.IsEqual(shiftLeaderId));

            foreach (var existingPeriod in existingPeriods)
            {
                if (request.StartFrom <= existingPeriod.LastDay )
                {
                    errors.Add(new ShiftAssignmentError(nameof(request.StartFrom), $"Shift period overlaps with existing period from {existingPeriod.StartFrom} to {existingPeriod.LastDay}"));
                    break;
                }
            }
        }

        // Use base class validation method to throw exception if there are errors
        Validate(errors);
    }
}