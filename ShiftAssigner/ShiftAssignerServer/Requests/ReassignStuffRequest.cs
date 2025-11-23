using System.Collections.Generic;

namespace ShiftAssignerServer.Requests;

/// <summary>
/// Request to reassign one or more workers to a different ShiftLeader within a tenant for a given period.
/// </summary>
public class ReassignStuffRequest
{
    // Worker IDs to reassign
    public List<string> WorkerIds { get; set; } = new List<string>();

    // Target ShiftLeader ID (the leader to whom the workers will be assigned)
    public string ShiftLeaderId { get; set; } = string.Empty;

    // Tenant (company) this reassignment applies to
    public string Tenant { get; set; } = string.Empty;

    // ISO date yyyy-MM-dd for period start
    public string PeriodStart { get; set; } = string.Empty;

    // Optional ISO date yyyy-MM-dd for the period end (inclusive)
    public string PeriodEnd { get; set; } = string.Empty;

    // Optional notes to attach to the new booking(s)
    public string Notes { get; set; } = string.Empty;
}
