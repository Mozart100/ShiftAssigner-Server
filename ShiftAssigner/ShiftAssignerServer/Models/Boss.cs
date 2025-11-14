using System;

namespace ShiftAssignerServer.Models
{
    /// <summary>
    /// Boss represents a higher-level person. Inherits from Person directly (bosses may not be "workers").
    /// </summary>
    public class Boss : Person
    {
        public Boss()
        {
        }

        public Boss(string firstName, string lastName, string phone, DateOnly dob, string tenant)
        {
            FirstName = firstName;
            LastName = lastName;
            PhoneNumber = phone;
            DateOfBirth = dob;
            Tenant = tenant;
        }
    }
}
