using FluentValidation;
using ShiftAssignerServer.Repositories;
using ShiftAssignerServer.Requests;

namespace ShiftAssignerServer.Services.Validation;

public interface IWorkerSchedulerValidationService
{
    Task ValidateCreateShiftPeriodRequestAsync(CreateShiftPeriodSchedulingRequest request, string shiftLeaderId);
    Task ValidateWorkerAssigningToPeriodRequestAsync(WorkerAssigningToPeriodRequest request, string workerId);
}

public class WorkerSchedulerValidationService : ServiceValidatorBase, IWorkerSchedulerValidationService
{
    private readonly ITenantUnitOfWork _tenantUnitOfWork;
    private readonly IValidator<CreateShiftPeriodSchedulingRequest> _validator;
    private readonly IValidator<WorkerAssigningToPeriodRequest> _workerAssigningValidator;

    public WorkerSchedulerValidationService(
        ITenantUnitOfWork tenantUnitOfWork, 
        IValidator<CreateShiftPeriodSchedulingRequest> validator,
        IValidator<WorkerAssigningToPeriodRequest> workerAssigningValidator,
        ILogger<WorkerSchedulerValidationService> logger)
        : base(logger)
    {
        _tenantUnitOfWork = tenantUnitOfWork;
        _validator = validator;
        _workerAssigningValidator = workerAssigningValidator;
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

    public async Task ValidateWorkerAssigningToPeriodRequestAsync(WorkerAssigningToPeriodRequest request, string workerId)
    {
        var errors = new List<ShiftAssignmentError>();

        // Validate worker ID
        if (string.IsNullOrWhiteSpace(workerId))
        {
            errors.Add(new ShiftAssignmentError("workerId", "Worker ID is required"));
        }

        // Use FluentValidation validator for request validation
        if (request != null)
        {
            var validationResult = await _workerAssigningValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                errors.AddRange(Dissect(validationResult));
            }

            // Business validation: Check if the shifts exist and are available for assignment
            if (request.Period != null && !errors.Any())
            {
                foreach (var day in request.Period)
                {
                    foreach (var requestedShift in day.Shifts)
                    {
                        // Find the shift period that contains this date and shift
                        var shiftPeriods = await _tenantUnitOfWork.ShiftPeriodSchedulingRepository
                            .GetAllAsync(x => x.IsActive && 
                                x.Period.Any(p => p.DateOnly == day.Date && 
                                    p.Shifts.Any(s => s.ShiftName.Equals(requestedShift.ShiftName, StringComparison.OrdinalIgnoreCase))));

                        if (!shiftPeriods.Any())
                        {
                            errors.Add(new ShiftAssignmentError(
                                $"{nameof(request.Period)}.Shifts.ShiftName",
                                $"No available shift '{requestedShift.ShiftName}' found for date {day.Date}"));
                            continue;
                        }

                        // Check if the shift has capacity and worker is not already assigned
                        foreach (var period in shiftPeriods)
                        {
                            var targetDay = period.Period.FirstOrDefault(p => p.DateOnly == day.Date);
                            var targetShift = targetDay?.Shifts.FirstOrDefault(s => 
                                s.ShiftName.Equals(requestedShift.ShiftName, StringComparison.OrdinalIgnoreCase));

                            if (targetShift != null)
                            {
                                // Check if worker is already assigned to this shift
                                if (targetShift.WorkerIds.Contains(workerId))
                                {
                                    errors.Add(new ShiftAssignmentError(
                                        $"{nameof(request.Period)}.Shifts",
                                        $"Worker is already assigned to shift '{requestedShift.ShiftName}' on {day.Date}"));
                                }
                                // Check if shift has capacity
                                else if (targetShift.WorkerIds.Count >= targetShift.AmountOfWorkers)
                                {
                                    errors.Add(new ShiftAssignmentError(
                                        $"{nameof(request.Period)}.Shifts",
                                        $"Shift '{requestedShift.ShiftName}' on {day.Date} is at full capacity ({targetShift.AmountOfWorkers} workers)"));
                                }
                            }
                        }
                    }
                }
            }
        }
        else
        {
            errors.Add(new ShiftAssignmentError("request", "Worker assigning to period request is required"));
        }

        // Use base class validation method to throw exception if there are errors
        Validate(errors);
    }
}