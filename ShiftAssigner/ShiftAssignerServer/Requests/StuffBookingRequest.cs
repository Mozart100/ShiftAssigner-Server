using System;

namespace ShiftAssignerServer.Requests;

public class StuffBookingRequest
{
    public string WorkerId { get; set; } = string.Empty;
    public string ShiftLeaderId { get; set; } = string.Empty;
    // ISO date yyyy-MM-dd for period start
    public string PeriodStart { get; set; } = string.Empty;
    // Optional ISO date yyyy-MM-dd for the period end (top period). If empty, booking is for a single-day/periodStart.
    public string PeriodEnd { get; set; } = string.Empty;
    // Tenant (company) this booking belongs to
    public string Tenant { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
}
