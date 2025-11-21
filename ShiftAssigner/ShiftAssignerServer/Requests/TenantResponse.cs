using ShiftAssignerServer.Models.Stuff;

namespace ShiftAssignerServer.Requests;

public class TenantResponse
{
    public List<string> Tenants { get; set; } = new List<string>();
}


public class GetShiftLeaderPerTenantResponse
{
    public List<PubShiftLeader> Tenants { get; set; } = new List<PubShiftLeader>();
}

