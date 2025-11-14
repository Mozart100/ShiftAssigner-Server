using System;

namespace ShiftAssignerServer.Models
{
    /// <summary>
    /// Represents a regular worker.
    /// </summary>
    public record Worker : PersonBase
    {
        // No additional properties for now — keep the Worker as a typed specialization
        // in case we need worker-specific fields later (e.g. skill set, availability).


        public Worker()
        {
        }

        public Worker(string id , string firstName, string lastName, string phone, DateOnly birthDate, string tenant, RoleState roleState, string passwordHash)
        {
            Id = id;
            FirstName = firstName;
            LastName = lastName;
            PhoneNumber = phone;
            DateOfBirth = birthDate;
            Tenant = tenant;
            this.Role = roleState;
            PasswordHash = passwordHash;
            // passwordHash will be stored in the in-memory credential store; do not keep duplicates here.
        }

    }
}
