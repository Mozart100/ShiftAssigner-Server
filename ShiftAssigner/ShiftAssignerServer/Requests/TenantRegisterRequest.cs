using ShiftAssignerServer.Models.Stuff;

namespace ShiftAssignerServer.Requests;

public interface ITenantRegistrationMapper : IRegistrationMapper
{
    // string CompanyName { get; set; }

}

public class TenantRegisterRequest : RegisterRequest

{
    public string Tenant { get; set; } = string.Empty;

    public ShiftConfig ShiftConfig { get; set; }
}


public class TenantRegisterResponse : RegisterResponse
{
    public string Tenant { get; set; }
}

