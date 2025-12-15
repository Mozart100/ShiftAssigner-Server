using System;

namespace ShiftAssignerServer.Models.Stuff
{
    public interface IBossTenantRegistrationMapper : IRegistrationMapper
    {

    }

    /// <summary>
    /// Boss represents a higher-level person. Inherits from Person directly (bosses may not be "workers").
    /// </summary>
    public record BossTenant : PersonBase,IBossTenantRegistrationMapper
    {
        public string Tenant { get; set; } = string.Empty;
        public ShiftConfig ShiftConfig { get; set; } = null;
        public BossTenant()
        {
            IsActive = true;
        }

        public BossTenant(string id,string firstName, string lastName, string phone, DateOnly dob, string tenant,  RoleState roleState,
            string passwordHash,ShiftConfig shiftConfig)
            : base()
        {

            ID = id;
            FirstName = firstName;
            LastName = lastName;
            PhoneNumber = phone;
            DateOfBirth = dob;
            Role = roleState;
            PasswordHash = passwordHash;
            Tenant = tenant;
            IsActive = true;
        
            ShiftConfig = shiftConfig;
        }
    }
}
