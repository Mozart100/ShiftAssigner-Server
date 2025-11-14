using System;

namespace ShiftAssignerServer.Models
{
    /// <summary>
    /// Boss represents a higher-level person. Inherits from Person directly (bosses may not be "workers").
    /// </summary>
    public record Boss : ShiftLeader
    {
        public Boss()
        {
        }

        public Boss(string id,string firstName, string lastName, string phone, DateOnly dob, string tenant,  RoleState roleState,
            string passwordHash)
            : base(id,firstName, lastName, phone, dob, tenant, roleState, passwordHash)
        
        {
        }
    }
}
