using System;

namespace ShiftAssignerServer.Models
{
    /// <summary>
    /// Represents a regular worker.
    /// </summary>
    public class Worker : Person
    {
        // No additional properties for now — keep the Worker as a typed specialization
        // in case we need worker-specific fields later (e.g. skill set, availability).

        public Worker()
        {
        }

        public Worker(string firstName, string lastName, string phone, DateOnly dob, string tenant)
        {
            FirstName = firstName;
            LastName = lastName;
            PhoneNumber = phone;
            DateOfBirth = dob;
            Tenant = tenant;
        }
    }
}
