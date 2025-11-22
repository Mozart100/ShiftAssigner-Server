using static ShiftAssignerServer.Models.Stuff.Worker;

namespace ShiftAssignerServer.Requests;

public class GetWorkerPerTenantResponse
{
    public IEnumerable<PubWorker> Workers { get; set; } 
}
