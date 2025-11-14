using System;

namespace ShiftAssignerServer.Models
{
    /// <summary>
    /// Shift leader is a specialized Worker responsible for leading a shift.
    /// Inherits from Worker so it keeps all worker properties and can gain leader-specific fields later.
    /// </summary>
    public record ShiftLeader : Worker
    {
        // EF Core requires a parameterless constructor
        public ShiftLeader()
        {
        }

        public ShiftLeader(
            string firstName,
            string lastName,
            string phone,
            DateOnly dob,
            string tenant,
            RoleState roleState,
            string passwordHash
        )
            : base(firstName, lastName, phone, dob, tenant, roleState, passwordHash)
        {
        }
    }

}
