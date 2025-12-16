using System;
using ShiftAssignerServer.Common;
using ShiftAssignerServer.Models;
using ShiftAssignerServer.Repositories;

namespace ShiftAssignerServer.Models.Stuff;

/// <summary>
/// Represents a booking of a Worker to a ShiftLeader for a specific period (week/day).
/// This allows the same worker to be supervised by different leaders across different periods.
/// Renamed from ShiftAssignment to StuffBooking.
/// </summary>
public partial class TeamHierarchy : IAutoMapperEntities, IActiveEntity
{
    public string ID { get; set; } = Guid.NewGuid().ToString("N");

    // Worker who is booked
    public string WorkerId { get; set; } = string.Empty;

    // Supervising shift leader for the given period
    public string ShiftLeaderId { get; set; } = string.Empty;

    // Period start date (use DateOnly to represent a day/period boundary). Interpret as week-start or period identifier.
    public DateOnly PeriodStart { get; set; }

    // Optional end/top date for the period. If set, the booking is valid for the inclusive range [PeriodStart..PeriodEnd].
    public DateOnly? PeriodEnd { get; set; }

    // Optional notes
    public string Notes { get; set; } = string.Empty;

    // Scheduled reassignment date - when null, no reassignment is scheduled
    public DateOnly? ReassignmentScheduledDate { get; set; }

    /// <summary>
    /// Soft delete flag. When false, the entity is considered logically deleted.
    /// </summary>
    public bool IsActive { get; set; } = true;
}

/// <summary>
/// Additional computed members for <see cref="TeamHierarchy"/>.
/// </summary>
public partial class TeamHierarchy
{
    /// <summary>
    /// Returns true when the booking is currently active.
    /// Interpretation: active when the current UTC date is on or after <see cref="PeriodStart"/> and
    /// (if <see cref="PeriodEnd"/> is set) on or before <see cref="PeriodEnd"/>. If <see cref="PeriodEnd"/>
    /// is null the booking remains active from <see cref="PeriodStart"/> onward.
    /// </summary>
    public bool IsOnDuty
    {
        get
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);

            if (today > PeriodStart && PeriodEnd is null)
            {
                return true;
            }

            return false;
        }
    }
}
