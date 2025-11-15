using System;
using ShiftAssignerServer.Repositories;

namespace ShiftAssignerServer.Models;

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
public record Worker : PersonBase , IRegistrationMapper
{
    // No additional properties for now — keep the Worker as a typed specialization
    // in case we need worker-specific fields later (e.g. skill set, availability).


    public Worker()
    {
    }

    public Worker(string id , string firstName, string lastName, string phone, DateOnly birthDate, string tenant, RoleState roleState, string passwordHash)
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
}
