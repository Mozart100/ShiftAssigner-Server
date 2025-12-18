using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
    private const string GetScheduleRoute = "schedule/{workerId}";
    private const string UpdateAvailabilityRoute = "availability";
    private const string RequestTimeOffRoute = "time-off";
    private const string SwapShiftRoute = "swap-shift";
    private const string CreateShiftPeriodRoute = "shift-period";

    public WorkerSchedulerController(IWorkerSchedulerService workerSchedulerService, JwtService jwtService)
        : base(jwtService)
    {
        _workerSchedulerService = workerSchedulerService ?? throw new ArgumentNullException(nameof(workerSchedulerService));
    }

    // POST: api/v1/WorkerScheduler/shift-period
    [HttpPost(CreateShiftPeriodRoute)]
    public async Task<ActionResult<CreateShiftPeriodSchedulingResponse>> CreateShiftPeriod([FromBody] CreateShiftPeriodSchedulingRequest request)
    {
        // Extract shift leader info from JWT token
        if (!TryGetShiftLeaderInfo(out string? shiftLeaderId, out RoleState? role))
        {
            return Unauthorized("Valid shift leader authentication required");
        }

        // Only shift leaders can create shift periods
        if (role != RoleState.ShiftLeader)
        {
            return Forbid("Only shift leaders can create shift periods");
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
