using ShiftAssignerServer.Models.Stuff;
using ShiftAssignerServer.Models.WorkerScheduling;
using static ShiftAssignerServer.Models.WorkerScheduling.TenantShiftScheduling;

namespace ShiftAssignerServer.Requests;

public interface ITenantRegistrationMapper : IRegistrationMapper
{
    // string CompanyName { get; set; }

}

public class TenantRegisterRequest : RegisterRequest,IShiftMapper

{
    public string Tenant { get; set; } = string.Empty;
    public List<ShiftInfo> Shifts { get; set; } = new List<ShiftInfo>();

}


public class TenantRegisterResponse : RegisterResponse
{
    public string Tenant { get; set; } = string.Empty;
}

