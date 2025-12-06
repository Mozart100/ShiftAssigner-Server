using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShiftAssignerServer.Requests;
using ShiftAssignerServer.Services;
using ShiftAssignerServer.Models.Stuff;

namespace ShiftAssignerServer.Controllers;

[Authorize]
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
    public async Task<IActionResult> Assign([FromBody] AssignStuffRequest request)
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
            PeriodStart = periodStart,
            PeriodEnd = periodEnd,
            Notes = request.Notes
        };

        await _service.AssignAsync(booking);
        return Ok();
    }

    // POST: api/v1/StuffBookings/reassign
    [HttpPost("reassign")]
    public async Task<ActionResult<ReassignWorkerResponse>> Reassign([FromBody] ReassignWorkerRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.WorkerId) || string.IsNullOrWhiteSpace(request.ShiftLeaderId) || string.IsNullOrWhiteSpace(request.PeriodStart) || string.IsNullOrWhiteSpace(request.Tenant))
        {
            return BadRequest("workerId, shiftLeaderId (target), tenant and periodStart are required (ISO yyyy-MM-dd)");
        }

        if (!DateOnly.TryParse(request.PeriodStart, out var periodStart))
        {
            return BadRequest("periodStart must be an ISO date: yyyy-MM-dd");
        }

        // await _service.ReassignAsync(request.Tenant, request.WorkerId, request.ShiftLeaderId, periodStart, null, request.Notes);
        
        var response = new ReassignWorkerResponse
        {
            ShiftLeaderId = request.ShiftLeaderId,
            PeriodStart = request.PeriodStart,
            Notes = request.Notes
        };
        
        return Ok(response);
    }


    // GET: api/v1/StuffBookings/leader/{leaderId}?period=yyyy-MM-dd
    // [HttpGet("leader/{leaderId}")]
    // public async Task<IActionResult> GetWorkersForLeader(string leaderId, [FromQuery] string period, [FromQuery] string tenant, [FromQuery] string periodEnd = "")
    // {
    //     if (string.IsNullOrWhiteSpace(tenant)) return BadRequest("tenant query parameter is required");
    //
    //     if (!DateOnly.TryParse(period, out var periodStart))
    //     {
    //         return BadRequest("period must be an ISO date: yyyy-MM-dd");
    //     }
    //
    //     DateOnly? pe = null;
    //     if (!string.IsNullOrWhiteSpace(periodEnd))
    //     {
    //         if (!DateOnly.TryParse(periodEnd, out var parsed)) return BadRequest("periodEnd must be an ISO date: yyyy-MM-dd");
    //         pe = parsed;
    //     }
    //
    //     var workers = await _service.GetWorkersForLeaderPeriodAsync(tenant, leaderId, periodStart, pe);
    //     return Ok(new GetWorkerPerTenantResponse { Workers = workers });
    // }

    
    // POST: api/v1/StuffBookings/reassign-bulk
    // [HttpPost("reassign-bulk")]
    // public async Task<IActionResult> ReassignBulk([FromBody] ReassignStuffRequest request)
    // {
    //     if (request is null) return BadRequest("request body is required");
    //     if (request.WorkerIds is null || !request.WorkerIds.Any()) return BadRequest("workerIds is required and must contain at least one id");
    //     if (string.IsNullOrWhiteSpace(request.ShiftLeaderId) || string.IsNullOrWhiteSpace(request.Tenant) || string.IsNullOrWhiteSpace(request.PeriodStart))
    //     {
    //         return BadRequest("shiftLeaderId (target), tenant and periodStart are required (ISO yyyy-MM-dd)");
    //     }

    //     if (!DateOnly.TryParse(request.PeriodStart, out var periodStart))
    //     {
    //         return BadRequest("periodStart must be an ISO date: yyyy-MM-dd");
    //     }

    //     DateOnly? periodEnd = null;
    //     if (!string.IsNullOrWhiteSpace(request.PeriodEnd))
    //     {
    //         if (!DateOnly.TryParse(request.PeriodEnd, out var pe))
    //         {
    //             return BadRequest("periodEnd must be an ISO date: yyyy-MM-dd");
    //         }
    //         periodEnd = pe;
    //     }

    //     // Perform reassignment for each worker id
    //     foreach (var workerId in request.WorkerIds)
    //     {
    //         if (string.IsNullOrWhiteSpace(workerId)) continue;
    //         await _service.ReassignAsync(request.Tenant, workerId, request.ShiftLeaderId, periodStart, periodEnd, request.Notes);
    //     }

    //     return Ok();
    // }
}
