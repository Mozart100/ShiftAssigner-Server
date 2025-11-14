using System;

namespace ShiftAssignerServer.Models
{
    /// <summary>
    /// Represents a regular worker.
    /// </summary>
    public record Worker : Person
    {
        // No additional properties for now — keep the Worker as a typed specialization
        // in case we need worker-specific fields later (e.g. skill set, availability).


        public Worker()
        {
        }

        public Worker(string firstName, string lastName, string phone, DateOnly birthDate, string tenant, RoleState roleState, string passwordHash)
        {
            FirstName = firstName;
            LastName = lastName;
            PhoneNumber = phone;
            DateOfBirth = birthDate;
            Tenant = tenant;
            this.Role = roleState;
            // passwordHash will be stored in the in-memory credential store; do not keep duplicates here.
        }

    }
}
