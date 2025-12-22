using System;
using ShiftAssignerServer.Common;
using ShiftAssignerServer.Repositories;

namespace ShiftAssignerServer.Models.Stuff;

public interface IRegistrationMapper : IAutoMapperEntities
{
    string ID { get; set; }
    string FirstName { get; set; }
    string LastName { get; set; }
    string PhoneNumber { get; set; }
    DateOnly DateOfBirth { get; set; }
    string PasswordHash { get; set; }
}

/// <summary>
/// Represents a regular worker.
/// </summary>
public record Worker : PersonBase, IRegistrationMapper
{
    // No additional properties for now — keep the Worker as a typed specialization
    // in case we need worker-specific fields later (e.g. skill set, availability).

    public bool IsPasswordRequired { get; set; }

    public Worker()
    {
        IsActive = true;
        IsPasswordRequired = true; // Workers must set password on first login
    }

    public Worker(string id, string firstName, string lastName, string phone, DateOnly birthDate, RoleState roleState, string passwordHash)
    {
        ID = id;
        FirstName = firstName;
        LastName = lastName;
        PhoneNumber = phone;
        DateOfBirth = birthDate;
        this.Role = roleState;
        PasswordHash = passwordHash;
        IsActive = true;
        IsPasswordRequired = true; // Workers must set password on first login
    }


    public interface IPubWorker : IAutoMapperEntities
    {
        string ID { get; set; }
        string FirstName { get; set; }
        string LastName { get; set; }
        string PhoneNumber { get; set; }
        DateOnly DateOfBirth { get; set; }
        string ShiftLeaderId { get; set; }
    }

    public class PubWorker : IPubWorker
    {
        public string ID { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public DateOnly DateOfBirth { get; set; }
        public string ShiftLeaderId { get; set; } = string.Empty;
    }
}
