using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShiftAssignerServer.Requests;
using ShiftAssignerServer.Services;
using ShiftAssignerServer.Models.Stuff;
using ShiftAssignerServer.Models;

namespace ShiftAssignerServer.Controllers;

[Authorize]
[ApiController]
[Route("api/v1/[controller]")]
public class StuffBookingsController : BaseController
{
    private readonly IStuffBookingService _service;

    public StuffBookingsController(IStuffBookingService service, JwtService jwtService)
        : base(jwtService)
    {
        _service = service;
    }


    // POST: api/v1/StuffBookings/reassign
    [HttpPost("reassign")]
    public async Task<ActionResult<ReassignWorkerResponse>> Reassign([FromBody] ReassignWorkerRequest request)
    {
        // Validate request
        if (request?.WorkerIds == null || !request.WorkerIds.Any())
        {
            return BadRequest("WorkerIds list is required and must contain at least one worker ID");
        }

        if (string.IsNullOrWhiteSpace(request.ReassignToShiftLeaderId))
        {
            return BadRequest("ReassignToShiftLeaderId is required");
        }

        // Extract shift leader info from JWT token
        if (!TryGetShiftLeaderInfo(out string? currentShiftLeaderId, out RoleState? role))
        {
            return Unauthorized("Valid shift leader authentication required");
        }

        // Verify that the requesting user is a shift leader
        if (role != RoleState.ShiftLeader)
        {
            return Forbid("Only shift leaders can reassign workers");
        }

        // Perform reassignment
        bool success = await _service.ReassignAsync(request);
        
        if (!success)
        {
            return BadRequest("Failed to reassign workers. Please check that all worker IDs are valid and currently assigned.");
        }

        var response = new ReassignWorkerResponse
        {
            ShiftLeaderId = request.ReassignToShiftLeaderId,
            Notes = $"Successfully reassigned {request.WorkerIds.Count} worker(s) to shift leader {request.ReassignToShiftLeaderId}. {request.Notes}".Trim()
        };
        
        return Ok(response);
    }


}
