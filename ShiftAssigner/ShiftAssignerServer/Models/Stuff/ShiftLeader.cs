using System;
using ShiftAssignerServer.Repositories;

namespace ShiftAssignerServer.Models.Stuff
{

    public interface IShiftLeaderRegistrationMapper : IRegistrationMapper
    {

    }

      public interface IPubShiftLeaderMapper : IAutoMapperEntities
    {
        string ID { get; set; }
        string FirstName { get; set; }
        string LastName { get; set; }
        string PhoneNumber { get; set; }
        DateOnly DateOfBirth { get; set; }
        string Tenant { get; set; }
    }


    /// <summary>
    /// Shift leader is a specialized Worker responsible for leading a shift.
    /// Inherits from Worker so it keeps all worker properties and can gain leader-specific fields later.
    /// </summary>
    public record ShiftLeader : Worker, IShiftLeaderRegistrationMapper
    {
        // EF Core requires a parameterless constructor
        public ShiftLeader()
        {
        }

        public ShiftLeader(
            string id,
            string firstName,
            string lastName,
            string phone,
            DateOnly dob,
            string tenant,
            RoleState roleState,
            string passwordHash
        )
            : base(id, firstName, lastName, phone, dob, tenant, roleState, passwordHash)
        {
        }
    }


  

    public record PubShiftLeader : IPubShiftLeaderMapper
    {
        public string ID { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public DateOnly DateOfBirth { get; set; }
        public string Tenant { get; set; } = string.Empty;
    }


}
