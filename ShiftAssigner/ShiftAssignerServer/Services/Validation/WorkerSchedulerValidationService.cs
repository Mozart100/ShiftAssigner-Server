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

        // Validate shift leader ID
        if (string.IsNullOrWhiteSpace(shiftLeaderId))
        {
            errors.Add(new ShiftAssignmentError("shiftLeaderId", "Shift leader ID is required"));
        }

        // Use FluentValidation validator for request validation
        if (request != null)
        {
            var validationResult = await _validator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                errors.AddRange(Dissect(validationResult));
            }
        }
        else
        {
            errors.Add(new ShiftAssignmentError("request", "Create shift period request is required"));
        }

        // Check for overlapping periods for the same shift leader if no errors so far
        if (!errors.Any() && !string.IsNullOrWhiteSpace(shiftLeaderId) && request != null && request.NextPeriod?.Any() == true)
        {
            var existingPeriods = await _tenantUnitOfWork.ShiftPeriodSchedulingRepository
                .GetAllAsync(x => x.IsActive && x.ShiftLeaderId == shiftLeaderId);

            var requestEndDate = request.NextPeriod.Max(d => d.Date);

            foreach (var existingPeriod in existingPeriods)
            {
                if (existingPeriod.Period?.Any() == true)
                {
                    var existingStartDate = existingPeriod.StartFrom;
                    var existingEndDate = existingPeriod.Period.Max(d => d.DateOnly);

                    // Check for overlap
                    if (request.StartFrom <= existingEndDate && requestEndDate >= existingStartDate)
                    {
                        errors.Add(new ShiftAssignmentError(nameof(request.StartFrom), $"Shift period overlaps with existing period from {existingStartDate} to {existingEndDate}"));
                        break;
                    }
                }
            }
        }

        // Use base class validation method to throw exception if there are errors
        Validate(errors);
    }
}