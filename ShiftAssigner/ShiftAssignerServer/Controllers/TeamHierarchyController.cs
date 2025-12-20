using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShiftAssignerServer.Requests;
using ShiftAssignerServer.Services;
using ShiftAssignerServer.Models;
using ShiftAssignerServer.Controllers.Attributes;

namespace ShiftAssignerServer.Controllers;

[Authorize]
[ApiController]
[Route("api/v1/[controller]")]
public class TeamHierarchyController : TenantControllerBase
{
    private readonly ITeamHierarchyService _service;
    
    private const string ReassignRoute = "reassign";
    private const string ShiftLeaderWorkersRoute = "shiftleader/{shiftLeaderId}/workers";

    public TeamHierarchyController(ITeamHierarchyService service, JwtService jwtService)
        : base(jwtService)
    {
        _service = service;
    }


    // POST: api/v1/StuffBookings/reassign
    [OnlyRole(RoleState.ShiftLeader)]
    [HttpPost(ReassignRoute)]
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
        if (!TryGetPersonInfo(out string? currentShiftLeaderId, out RoleState? role))
        {
            return Unauthorized("Valid shift leader authentication required");
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

    // GET: api/v1/StuffBookings/shiftleader/{shiftLeaderId}/workers
    [HttpGet(ShiftLeaderWorkersRoute)]
    public async Task<ActionResult<GetWorkerPerShiftLeaderResponse>> GetShiftLeaderWithWorkers(string shiftLeaderId)
    {
        // Extract shift leader info from JWT token to verify authorization
        if (!TryGetPersonInfo(out string? currentShiftLeaderId, out RoleState? role))
        {
            return Unauthorized("Valid shift leader authentication required");
        }

        // Verify that the requesting user is a shift leader
        if (role != RoleState.ShiftLeader)
        {
            return Forbid("Only shift leaders can access this information");
        }

        // Optional: Allow shift leaders to only view their own workers
        // Uncomment the following lines if you want to restrict access
        // if (currentShiftLeaderId != shiftLeaderId)
        // {
        //     return Forbid("You can only view your own workers");
        // }

        var shiftLeaderWithWorkers = await _service.GetShiftLeaderWithWorkersAsync(shiftLeaderId);
        
        if (shiftLeaderWithWorkers == null)
        {
            return NotFound($"Shift leader with ID '{shiftLeaderId}' not found");
        }

        return Ok(shiftLeaderWithWorkers);
    }


}
