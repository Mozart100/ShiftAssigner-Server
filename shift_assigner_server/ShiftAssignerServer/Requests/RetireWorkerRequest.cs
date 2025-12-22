namespace ShiftAssignerServer.Requests;

/// <summary>
/// Request to retire (soft delete) a worker by setting IsActive to false.
/// </summary>
public class RetireWorkerRequest
{
    /// <summary>
    /// The ID of the worker to retire.
    /// </summary>
    public string WorkerId { get; set; } = string.Empty;

    /// <summary>
    /// The tenant (company) this worker belongs to.
    /// </summary>
    public string Tenant { get; set; } = string.Empty;

    /// <summary>
    /// Optional reason for retirement.
    /// </summary>
    public string Reason { get; set; } = string.Empty;
}
