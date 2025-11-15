using System;

namespace ShiftAssignerServer.Models
{
    public class RegisterRequest
    {
        public string ID { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public DateOnly DateOfBirth { get; set; }
        public string Tenant { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
