using FluentValidation;
using ShiftAssignerServer.Requests;

namespace ShiftAssignerServer.Services.Validation;

/// <summary>
/// Registration validation service that validates registration requests
/// and enforces business rules for registration integrity.
/// </summary>
public interface IRegistrationValidationService
{
    /// <summary>
    /// Validates a registration request for a worker, shift leader, or boss tenant.
    /// Throws ShiftAssignmentException if validation fails.
    /// </summary>
    void ValidateRegistration(RegisterRequest request, string registrationType = "Unknown");
}

public class RegistrationValidationService : ServiceValidatorBase, IRegistrationValidationService
{
    private readonly IValidator<RegisterRequest> _validator;

    public RegistrationValidationService(
        IValidator<RegisterRequest> validator,
        ILogger<RegistrationValidationService> logger) 
        : base(logger)
    {
        _validator = validator;
    }

    /// <summary>
    /// Validates a registration request using FluentValidation.
    /// </summary>
    /// <param name="request">The registration request to validate</param>
    /// <param name="registrationType">Type of registration (Worker, ShiftLeader, BossTenant) for logging</param>
    /// <exception cref="ShiftAssignmentException">Thrown when validation fails</exception>
    public void ValidateRegistration(RegisterRequest request, string registrationType = "Unknown")
    {
        Logger.LogInformation("Validating {RegistrationType} registration for ID: {ID}", registrationType, request.ID);

        // Validate against null
        if (request == null)
        {
            ValidateAndThrow("Request", "Registration request cannot be null");
            return;
        }

        // Run FluentValidation
        var validationResult = _validator.Validate(request);

        // Convert validation errors to ShiftAssignmentError format
        var errors = Dissect(validationResult);

        // Additional business rule validations
        ValidateBusinessRules(request, errors, registrationType);

        // Throw exception if any errors found
        Validate(errors);

        Logger.LogInformation("{RegistrationType} registration validation passed for ID: {ID}", registrationType, request.ID);
    }

    /// <summary>
    /// Additional business rule validations beyond basic field validation.
    /// </summary>
    private void ValidateBusinessRules(RegisterRequest request, IList<ShiftAssignmentError> errors, string registrationType)
    {
        // Business Rule: ID must not contain special characters (except hyphens and underscores)
        if (!string.IsNullOrEmpty(request.ID) && !IsValidIdFormat(request.ID))
        {
            errors.Add(new ShiftAssignmentError(
                nameof(request.ID),
                "ID must contain only alphanumeric characters, hyphens, or underscores"));
        }

        // Business Rule: Worker registration must have a ShiftLeaderId
        if (registrationType.Equals("Worker", StringComparison.OrdinalIgnoreCase))
        {
            if (string.IsNullOrWhiteSpace(request.ShiftLeaderId))
            {
                errors.Add(new ShiftAssignmentError(
                    nameof(request.ShiftLeaderId),
                    "ShiftLeaderId is required when registering a worker"));
            }
        }

        // Business Rule: ShiftLeader and BossTenant should not have a ShiftLeaderId
        if (registrationType.Equals("ShiftLeader", StringComparison.OrdinalIgnoreCase) ||
            registrationType.Equals("BossTenant", StringComparison.OrdinalIgnoreCase))
        {
            if (!string.IsNullOrWhiteSpace(request.ShiftLeaderId))
            {
                errors.Add(new ShiftAssignmentError(
                    nameof(request.ShiftLeaderId),
                    $"ShiftLeaderId should not be provided when registering a {registrationType}"));
            }
        }

        // Business Rule: Check for suspicious patterns in names
        if (ContainsSuspiciousPatterns(request.FirstName) || ContainsSuspiciousPatterns(request.LastName))
        {
            errors.Add(new ShiftAssignmentError(
                "Name",
                "Names must not contain numbers or special characters"));
        }
    }

    /// <summary>
    /// Validates that ID contains only alphanumeric characters, hyphens, or underscores.
    /// </summary>
    private bool IsValidIdFormat(string id)
    {
        return System.Text.RegularExpressions.Regex.IsMatch(id, @"^[a-zA-Z0-9_-]+$");
    }

    /// <summary>
    /// Checks if a name contains suspicious patterns (numbers, special chars).
    /// </summary>
    private bool ContainsSuspiciousPatterns(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) return false;
        
        // Names should only contain letters, spaces, hyphens, and apostrophes
        return !System.Text.RegularExpressions.Regex.IsMatch(name, @"^[a-zA-Z\s\-']+$");
    }
}
