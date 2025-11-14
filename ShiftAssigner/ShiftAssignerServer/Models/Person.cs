using System;

namespace ShiftAssignerServer.Models
{
    /// <summary>
    /// Base class for people in the system.
    /// </summary>
    public abstract class Person
    {
        /// <summary>
        /// Unique identifier for the person. Use Guid so these models are DB-agnostic.
        /// </summary>
        public Guid Id { get; set; } = Guid.NewGuid();

        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public string PhoneNumber { get; set; } = string.Empty;

        // DateOnly is available on net6+ / net8
        public DateOnly DateOfBirth { get; set; }

        /// <summary>
        /// Represents the tenant / company name this person belongs to.
        /// </summary>
        public string Tenant { get; set; } = string.Empty;
    }
}
