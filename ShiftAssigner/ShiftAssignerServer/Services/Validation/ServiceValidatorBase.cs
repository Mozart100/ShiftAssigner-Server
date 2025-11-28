using System;
using FluentValidation.Results;

namespace ShiftAssignerServer.Services.Validation;

public abstract class ServiceValidatorBase
{
    protected ServiceValidatorBase(ILogger logger)
    {
        Logger = logger;
    }


    protected virtual IList<ShiftAssignmentError> Dissect(ValidationResult validationResult)
    {
        var errors = new List<ShiftAssignmentError>();
        if (!validationResult.IsValid)
        {
            foreach (var error in validationResult.Errors)
            {
                errors.Add(new ShiftAssignmentError(errorMessage: error.ErrorMessage, propertyName: error.PropertyName));
            }
        }

        return errors;
    }
    public ILogger Logger { get; }

    protected void Validate(IEnumerable<ShiftAssignmentError> errors)
    {
        if (errors.SafeAny())
        {
            var instance = new ShiftAssignmentException(errors.ToArray());
            ThrowException(instance);
        }
    }


    protected void Validate(string propertyName, string reason)
    {
        var error = new ShiftAssignmentError(propertyName, reason);
        var instance = new ShiftAssignmentException(error);
        ThrowException(instance);
    }

    private void ThrowException(ShiftAssignmentException shiftAssignmentException)
    {

        Logger.LogError(shiftAssignmentException, "ShiftAssignmentException {Topic} {@Erros} {@Exception}", "Validation", shiftAssignmentException.ShiftAssignmentErrors, shiftAssignmentException);
        throw shiftAssignmentException;
    }

    //   private void ThrowException(ShiftAssignmentException shiftAssignmentException)
    //   {
    //       Logger.LogError(shiftAssignmentException, "ShiftAssignmentException {Topic} {@Erros} {@Exception}", "Validation", shiftAssignmentException.ShiftAssignmentErrors, shiftAssignmentException);

    //   }

}
