using System;

namespace ShiftAssignerServer.Requests;

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


public class RegisterResponse
{
    /// JWT token string
    /// </summary>
    public string Token { get; set; } = string.Empty;
}
