using System;

namespace ShiftAssignerServer.Models.Stuff
{
    public interface IBossTenantRegistrationMapper : IRegistrationMapper
    {

    }

    /// <summary>
    /// Boss represents a higher-level person. Inherits from Person directly (bosses may not be "workers").
    /// </summary>
    public record BossTenant : ShiftLeader,IBossTenantRegistrationMapper
    {
        public BossTenant()
        {
        }

        public BossTenant(string id,string firstName, string lastName, string phone, DateOnly dob, string tenant,  RoleState roleState,
            string passwordHash)
            : base(id,firstName, lastName, phone, dob, tenant, roleState, passwordHash)
        
        {
        }
    }
}
