namespace ShiftAssignerServer.Requests;

public class ReassignWorkerRequest
{
    public List<string> WorkerIds { get; set; } = new List<string>();
    public string WorkerId { get; set; } = string.Empty;
    public string ShiftLeaderId { get; set; } = string.Empty;
    // ISO date yyyy-MM-dd for period start
    public string PeriodStart { get; set; } = string.Empty;

    // Tenant (company) this booking belongs to
    public string Tenant { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
}

public class ReassignWorkerResponse
{
    public string ShiftLeaderId { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
    public string PeriodStart { get; set; } = string.Empty;
    public string Tenant { get; set; } = string.Empty;
    
}
