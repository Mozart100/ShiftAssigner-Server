using FluentValidation;
using ShiftAssignerServer.Requests;

namespace ShiftAssignerServer.Services.Validation;

public interface IWorkersServiceValidation
{
    Task Registering(WorkerRegisteringRequest request);
}

public class WorkersServiceValidation : ServiceValidatorBase, IWorkersServiceValidation
{
    private readonly IValidator<WorkerRegisteringRequest> _validator;

    public WorkersServiceValidation(ILogger<WorkersServiceValidation> logger, IValidator<WorkerRegisteringRequest> validator) : base(logger)
    {
        _validator = validator;
    }

    public async Task Registering(WorkerRegisteringRequest request)
    {
        Logger.LogInformation("Worker Validation");

        // Validate against null
        if (request == null)
        {
            ValidateAndThrow("Request", "Registration request cannot be null");
            return;
        }


        var validationResult = _validator.Validate(request);
        var errors = Dissect(validationResult);

        // Throw exception if any errors found
        Validate(errors);
    }
}
