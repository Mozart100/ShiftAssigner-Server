using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShiftAssignerServer.Controllers.Attributes;
using ShiftAssignerServer.Models;
using ShiftAssignerServer.Requests;
using ShiftAssignerServer.Services;

namespace ShiftAssignerServer.Controllers;

[Authorize]
[ApiController]
[Route("api/v1/[controller]")]
public class WorkerSchedulerController : TenantControllerBase
{
    private readonly IWorkerSchedulerService _workerSchedulerService;
    private const string CreateShiftPeriodRoute = "shift-period";

    public WorkerSchedulerController(IWorkerSchedulerService workerSchedulerService, JwtService jwtService)
        : base(jwtService)
    {
        _workerSchedulerService = workerSchedulerService ?? throw new ArgumentNullException(nameof(workerSchedulerService));
    }

    // POST: api/v1/WorkerScheduler/shift-period
    [MinimumRole(RoleState.ShiftLeader)]
    [HttpPost(CreateShiftPeriodRoute)]
    // [ValidateModel]
    public async Task<ActionResult<CreateShiftPeriodSchedulingResponse>> CreateShiftPeriod([FromBody] CreateShiftPeriodSchedulingRequest request)
    {
        // Role validation is handled by [MinimumRole] attribute
        // Both ShiftLeader and BossTenant can access this endpoint
        var shiftLeaderId = HttpContext.GetShiftLeaderId();
        
        if (string.IsNullOrEmpty(shiftLeaderId))
        {
            return Unauthorized("Valid shift leader authentication required");
        }

        var record = await _workerSchedulerService.CreateNewWorkerRegisteringRequest(request, shiftLeaderId);

        var response = new CreateShiftPeriodSchedulingResponse
        {
            ShiftLeaderId = shiftLeaderId,
            StartFrom = request.StartFrom,
            LastDate = record.LastDay,
            Success = true,
            Message = "Shift period created successfully"
        };
        return Ok(response);
    }

}
