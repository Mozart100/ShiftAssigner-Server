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
        TryGetPersonInfo(out string? currentShiftLeaderId, out RoleState? _);

        // Perform reassignment
        bool success = await _service.ReassignAsync(request);

        var response = new ReassignWorkerResponse
        {
            ShiftLeaderId = request.ReassignToShiftLeaderId,
            Notes = $"Successfully reassigned {request.WorkerIds.Count} worker(s) to shift leader {request.ReassignToShiftLeaderId}. {request.Notes}".Trim()
        };
        
        return Ok(response);
    }

    // GET: api/v1/StuffBookings/shiftleader/{shiftLeaderId}/workers
    [OnlyRole(RoleState.ShiftLeader)]
    [HttpGet(ShiftLeaderWorkersRoute)]
    public async Task<ActionResult<GetWorkerPerShiftLeaderResponse>> GetShiftLeaderWithWorkers(string shiftLeaderId)
    {
        // Extract shift leader info from JWT token to verify authorization
        TryGetPersonInfo(out string? currentShiftLeaderId, out RoleState? role);

        var shiftLeaderWithWorkers = await _service.GetShiftLeaderWithWorkersAsync(shiftLeaderId);
        
        return Ok(shiftLeaderWithWorkers);
    }


}
