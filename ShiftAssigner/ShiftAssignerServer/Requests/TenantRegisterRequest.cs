using ShiftAssignerServer.Models.Stuff;
using ShiftAssignerServer.Models.WorkerScheduling;

namespace ShiftAssignerServer.Requests;

public interface ITenantRegistrationMapper : IRegistrationMapper
{
    // string CompanyName { get; set; }

}

public class TenantRegisterRequest : RegisterRequest

{
    public string Tenant { get; set; } = string.Empty;

    public TenantShiftConfig ShiftConfig { get; set; }
}


public class TenantRegisterResponse : RegisterResponse
{
    public string Tenant { get; set; }
}

