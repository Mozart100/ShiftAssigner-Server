using System;
using ShiftAssignerServer.Repositories;

namespace ShiftAssignerServer.Models.Stuff;

public interface IRegistrationMapper : IAutoMapperEntities
{
    string ID { get; set; }
    string FirstName { get; set; }
    string LastName { get; set; }
    string PhoneNumber { get; set; }
    DateOnly DateOfBirth { get; set; }
    string Tenant { get; set; }
    string PasswordHash { get; set; }
}

/// <summary>
/// Represents a regular worker.
/// </summary>
public record Worker : PersonBase, IRegistrationMapper
{
    // No additional properties for now — keep the Worker as a typed specialization
    // in case we need worker-specific fields later (e.g. skill set, availability).


    public Worker()
    {
    }

    public Worker(string id, string firstName, string lastName, string phone, DateOnly birthDate, string tenant, RoleState roleState, string passwordHash)
    {
        ID = id;
        FirstName = firstName;
        LastName = lastName;
        PhoneNumber = phone;
        DateOfBirth = birthDate;
        Tenant = tenant;
        this.Role = roleState;
        PasswordHash = passwordHash;
    }


    public interface IPubWorker : IAutoMapperEntities
    {
        string ID { get; set; }
        string FirstName { get; set; }
        string LastName { get; set; }
        string PhoneNumber { get; set; }
        DateOnly DateOfBirth { get; set; }
        string Tenant { get; set; }
    }

    public class PubWorker : IPubWorker
    {
        public string ID { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public DateOnly DateOfBirth { get; set; }
        public string Tenant { get; set; } = string.Empty;
    }
}
