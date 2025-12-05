using System;
using ShiftAssignerServer.Common;
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
    }


    /// <summary>
    /// Shift leader is a specialized Worker responsible for leading a shift.
    /// Inherits from Worker so it keeps all worker properties and can gain leader-specific fields later.
    /// </summary>
    public record ShiftLeader : PersonBase, IShiftLeaderRegistrationMapper
    {
        // EF Core requires a parameterless constructor
        public ShiftLeader()
        {
            IsActive = true;
            IsPasswordRequired = true;
            PasswordHash = string.Empty; // No password set initially
        }

        public bool IsPasswordRequired { get; set; }

        public ShiftLeader(
            string id,
            string firstName,
            string lastName,
            string phone,
            DateOnly dob,
            string tenant,
            RoleState roleState,
            string passwordHash = "" // Default to empty password
        ) : base()
        {
            ID = id;
            FirstName = firstName;
            LastName = lastName;
            PhoneNumber = phone;
            DateOfBirth = dob;
            Role = roleState;
            PasswordHash = string.Empty; // Initially no password
            IsActive = true;
            IsPasswordRequired = true;   // Must set password on first login
        }
    }




    public class PubShiftLeader : IPubShiftLeaderMapper
    {
        public string ID { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public DateOnly DateOfBirth { get; set; }
    }


}
