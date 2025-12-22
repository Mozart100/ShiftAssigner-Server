using FluentValidation;
using ShiftAssignerServer.Repositories;
using ShiftAssignerServer.Requests;

namespace ShiftAssignerServer.Services.Validation;

public interface ITeamHierarchyValidationService
{
    Task ValidateReassignWorkerRequestAsync(ReassignWorkerRequest request);
}

public class TeamHierarchyValidationService : ServiceValidatorBase, ITeamHierarchyValidationService
{
    private readonly ITenantUnitOfWork _tenantUnitOfWork;
    private readonly IValidator<ReassignWorkerRequest> _reassignWorkerValidator;

    public TeamHierarchyValidationService(
        ITenantUnitOfWork tenantUnitOfWork,
        IValidator<ReassignWorkerRequest> reassignWorkerValidator,
        ILogger<TeamHierarchyValidationService> logger) : base(logger)
    {
        _tenantUnitOfWork = tenantUnitOfWork;
        _reassignWorkerValidator = reassignWorkerValidator;
    }

    public async Task ValidateReassignWorkerRequestAsync(ReassignWorkerRequest request)
    {
        var errors = new List<ShiftAssignmentError>();

        // Use FluentValidation validator for request validation
        var validationResult = await _reassignWorkerValidator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            errors.AddRange(Dissect(validationResult));
        }

        // Business validation: Check if workers exist and are active
        if (errors.IsEmpty())
        {
            foreach (var workerId in request.WorkerIds)
            {
                var worker = await _tenantUnitOfWork.WorkerRepository.FirstOrDefaultAsync(w => w.ID == workerId && w.IsActive);
                if (worker == null)
                {
                    errors.Add(new ShiftAssignmentError(nameof(request.WorkerIds), $"Worker '{workerId}' not found or is inactive"));
                    continue;
                }
            }

            // Business validation: Check if target shift leader exists and is active
            var targetShiftLeader = await _tenantUnitOfWork.ShiftLeaderRepository.FirstOrDefaultAsync(sl => 
                sl.ID == request.ReassignToShiftLeaderId && sl.IsActive);
            
            if (targetShiftLeader == null)
            {
                errors.Add(new ShiftAssignmentError(nameof(request.ReassignToShiftLeaderId), 
                    $"Target shift leader '{request.ReassignToShiftLeaderId}' not found or is inactive"));
            }

            // Business validation: Check if workers are not already assigned to the target shift leader
            if (targetShiftLeader != null)
            {
                foreach (var workerId in request.WorkerIds)
                {
                    var existingAssignment = await _tenantUnitOfWork.TeamHierarchyRepository.FirstOrDefaultAsync(th =>
                        th.WorkerId == workerId &&
                        th.ShiftLeaderId == request.ReassignToShiftLeaderId &&
                        th.IsActive &&
                        th.PeriodEnd == null);

                    if (existingAssignment != null)
                    {
                        errors.Add(new ShiftAssignmentError(nameof(request.WorkerIds), 
                            $"Worker '{workerId}' is already assigned to shift leader '{request.ReassignToShiftLeaderId}'"));
                    }
                }
            }
        }

        // Use base class validation method to throw exception if there are errors
        Validate(errors);
    }
}

public class ReassignWorkerRequestValidator : AbstractValidator<ReassignWorkerRequest>
{
    public ReassignWorkerRequestValidator()
    {
        RuleFor(x => x.WorkerIds)
            .NotNull()
            .WithMessage("Worker IDs are required")
            .Must(list => list != null && list.Any())
            .WithMessage("At least one worker ID must be provided")
            .Must(list => list != null && list.All(id => !string.IsNullOrWhiteSpace(id)))
            .WithMessage("All worker IDs must be valid non-empty strings");

        RuleFor(x => x.ReassignToShiftLeaderId)
            .NotEmpty()
            .WithMessage("Target shift leader ID is required")
            .Length(1, 50)
            .WithMessage("Target shift leader ID must be between 1 and 50 characters");

        RuleFor(x => x.Notes)
            .MaximumLength(500)
            .WithMessage("Notes cannot exceed 500 characters");
    }
}