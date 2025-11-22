using System;
using ShiftAssignerServer.Repositories;

namespace ShiftAssignerServer.Models.Stuff;

/// <summary>
/// Represents an assignment of a Worker to a ShiftLeader for a specific period (week/day).
/// This allows the same worker to be supervised by different leaders across different periods.
/// </summary>
public class ShiftAssignment : IAutoMapperEntities
{
    public string ID { get; set; } = Guid.NewGuid().ToString("N");

    // Worker who is assigned
    public string WorkerId { get; set; } = string.Empty;

    // Supervising shift leader for the given period
    public string ShiftLeaderId { get; set; } = string.Empty;

    // Tenant (company) this assignment belongs to - enforces tenant isolation
    public string Tenant { get; set; } = string.Empty;

    // Period start date (use DateOnly to represent a day/period boundary). Interpret as week-start or period identifier.
    public DateOnly PeriodStart { get; set; }

    // Optional end/top date for the period. If set, the assignment is valid for the inclusive range [PeriodStart..PeriodEnd].
    public DateOnly? PeriodEnd { get; set; }

    // Optional notes
    public string Notes { get; set; } = string.Empty;
}
