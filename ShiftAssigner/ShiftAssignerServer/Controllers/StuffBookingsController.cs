using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ShiftAssignerServer.Requests;
using ShiftAssignerServer.Services;
using ShiftAssignerServer.Models.Stuff;

namespace ShiftAssignerServer.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class StuffBookingsController : ControllerBase
{
    private readonly IStuffBookingService _service;

    public StuffBookingsController(IStuffBookingService service)
    {
        _service = service;
    }

    // POST: api/v1/StuffBookings/assign
    [HttpPost("assign")]
    public async Task<IActionResult> Assign([FromBody] StuffBookingRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.WorkerId) || string.IsNullOrWhiteSpace(request.ShiftLeaderId) || string.IsNullOrWhiteSpace(request.PeriodStart) || string.IsNullOrWhiteSpace(request.Tenant))
        {
            return BadRequest("workerId, shiftLeaderId, tenant and periodStart are required (ISO yyyy-MM-dd)");
        }

        if (!DateOnly.TryParse(request.PeriodStart, out var periodStart))
        {
            return BadRequest("periodStart must be an ISO date: yyyy-MM-dd");
        }

        DateOnly? periodEnd = null;
        if (!string.IsNullOrWhiteSpace(request.PeriodEnd))
        {
            if (!DateOnly.TryParse(request.PeriodEnd, out var pe))
            {
                return BadRequest("periodEnd must be an ISO date: yyyy-MM-dd");
            }
            periodEnd = pe;
        }

        var booking = new StuffBooking
        {
            WorkerId = request.WorkerId,
            ShiftLeaderId = request.ShiftLeaderId,
            Tenant = request.Tenant,
            PeriodStart = periodStart,
            PeriodEnd = periodEnd,
            Notes = request.Notes
        };

        await _service.AssignAsync(booking);
        return Ok();
    }

    // GET: api/v1/StuffBookings/leader/{leaderId}?period=yyyy-MM-dd
    [HttpGet("leader/{leaderId}")]
    public async Task<IActionResult> GetWorkersForLeader(string leaderId, [FromQuery] string period, [FromQuery] string tenant, [FromQuery] string periodEnd = "")
    {
        if (string.IsNullOrWhiteSpace(tenant)) return BadRequest("tenant query parameter is required");

        if (!DateOnly.TryParse(period, out var periodStart))
        {
            return BadRequest("period must be an ISO date: yyyy-MM-dd");
        }

        DateOnly? pe = null;
        if (!string.IsNullOrWhiteSpace(periodEnd))
        {
            if (!DateOnly.TryParse(periodEnd, out var parsed)) return BadRequest("periodEnd must be an ISO date: yyyy-MM-dd");
            pe = parsed;
        }

        var workers = await _service.GetWorkersForLeaderPeriodAsync(tenant, leaderId, periodStart, pe);
        return Ok(new GetWorkerPerTenantResponse { Workers = workers });
    }
}
