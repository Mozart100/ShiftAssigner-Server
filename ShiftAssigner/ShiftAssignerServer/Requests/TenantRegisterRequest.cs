using ShiftAssignerServer.Models.Stuff;

namespace ShiftAssignerServer.Requests;

public interface ITenantRegistrationMapper : IRegistrationMapper
{
    string CompanyName { get; set; }

}

public class TenantRegisterRequest : IRegistrationMapper
{
    public string ID { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public DateOnly DateOfBirth { get; set; }
    public string Tenant { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;


    public string CompanyName { get; set; }
}


public class TenantRegisterResponse:RegisterResponse 
{
}

