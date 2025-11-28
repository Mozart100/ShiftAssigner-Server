using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ShiftAssignerServer.Models.Stuff;
using ShiftAssignerServer.Requests;
using ShiftAssignerServer.Services;
using ShiftAssignerServer.Repositories;

namespace ShiftAssignerServer.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class WorkersController : ControllerBase
{
    private readonly IWorkerService _service;
    private readonly IStuffBookingService _assignmentService;
    private readonly IShiftLeaderRepository _shiftLeaderRepository;

    public WorkersController(IWorkerService service, IStuffBookingService assignmentService, IShiftLeaderRepository shiftLeaderRepository)
    {
        _service = service;
        _assignmentService = assignmentService;
        _shiftLeaderRepository = shiftLeaderRepository;
    }

    // GET: api/v1/Workers/{tenant}
    [HttpGet("{tenant}")]
    public async Task<ActionResult<GetWorkerPerTenantResponse>> GetAllPerTenant(string tenant)
    {
        var workers = await _service.GetAllActiveWorkersPerShiftLeaderAsync(tenant);
        return Ok(new GetWorkerPerTenantResponse{ Workers = workers});
    }

    // GET: api/v1/Workers/leader/{shiftLeaderId}
    // If a period query parameter is supplied (yyyy-MM-dd) it is used; otherwise the current UTC date is used.
    [HttpGet("leader/{shiftLeaderId}")]
    public async Task<ActionResult<GetWorkerPerTenantResponse>> GetAllPerLeader(string shiftLeaderId, [FromQuery] string period = "")
    {
        // Find leader to determine tenant
        var leader = _shiftLeaderRepository.FirstOrDefault(x => x.ID.Equals(shiftLeaderId, StringComparison.InvariantCultureIgnoreCase));
        if (leader is null) return NotFound();

        DateOnly periodStart;
        if (string.IsNullOrWhiteSpace(period)) periodStart = DateOnly.FromDateTime(DateTime.UtcNow);
        else if (!DateOnly.TryParse(period, out periodStart)) return BadRequest("period must be yyyy-MM-dd");

        var workers = await _assignmentService.GetWorkersForLeaderPeriodAsync(leader.Tenant, shiftLeaderId, periodStart);
        return Ok(new GetWorkerPerTenantResponse { Workers = workers });
    }

    // POST: api/v1/Workers/retire
    [HttpPost("retire")]
    public async Task<IActionResult> RetireWorker([FromBody] RetireWorkerRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.WorkerId) || string.IsNullOrWhiteSpace(request.Tenant))
        {
            return BadRequest("workerId and tenant are required");
        }

        var result = await _service.RetireWorkerAsync(request.Tenant, request.WorkerId);
        
        if (result)
        {
            return Ok(new { Message = "Worker retired successfully", WorkerId = request.WorkerId });
        }
        
        return BadRequest("Failed to retire worker");
    }
}
