using ShiftAssignerServer.Models.Stuff;

namespace ShiftAssignerServer.Requests;

public class GetShiftLeaderPerTenantResponse
{
    public IEnumerable<PubShiftLeader> ShifLeaders { get; set; } 
}
