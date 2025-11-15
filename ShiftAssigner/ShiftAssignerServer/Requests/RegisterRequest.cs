using System;
using ShiftAssignerServer.Models;
using ShiftAssignerServer.Models.Stuff;
using ShiftAssignerServer.Repositories;

namespace ShiftAssignerServer.Requests;




public class RegisterRequest : IRegistrationMapper
{
    public string ID { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public DateOnly DateOfBirth { get; set; }
    public string Tenant { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
}


public class RegisterResponse
{
    /// JWT token string
    /// </summary>
    public string Token { get; set; } = string.Empty;
}
