using System;
using ShiftAssignerServer.Repositories;

namespace ShiftAssignerServer.Models.Stuff;

/// <summary>
/// Represents an assignment of a Worker to a ShiftLeader for a specific period (week/day).
/// This allows the same worker to be supervised by different leaders across different periods.
/// </summary>
public partial class ShiftAssignment : IAutoMapperEntities
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

/// <summary>
/// Additional computed members for <see cref="ShiftAssignment"/>.
/// </summary>
public partial class ShiftAssignment
{
    /// <summary>
    /// Returns true when the assignment is currently active.
    /// Interpretation: active when the current UTC date is on or after <see cref="PeriodStart"/> and
    /// (if <see cref="PeriodEnd"/> is set) on or before <see cref="PeriodEnd"/>. If <see cref="PeriodEnd"/>
    /// is null the assignment remains active from <see cref="PeriodStart"/> onward.
    /// </summary>
    public bool Active
    {
        get
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            if (today < PeriodStart)
            {
                return false;
            }
            if (PeriodEnd is null)
            {
                return true;
            }
            
            return today <= PeriodEnd.Value;
        }
    }
}
