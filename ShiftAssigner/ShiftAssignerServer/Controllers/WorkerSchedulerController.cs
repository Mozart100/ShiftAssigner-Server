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

    public WorkerSchedulerController(IWorkerSchedulerService workerSchedulerService , JwtService jwtService)
        : base(jwtService )
    {
        _workerSchedulerService = workerSchedulerService ?? throw new ArgumentNullException(nameof(workerSchedulerService));
    }

    // POST: api/v1/WorkerScheduler/shift-period
    [MinimumRole(RoleState.ShiftLeader)]
    [HttpPost(CreateShiftPeriodRoute)]
    public async Task<ActionResult<CreateShiftPeriodSchedulingResponse>> CreateShiftPeriod([FromBody] CreateShiftPeriodSchedulingRequest request)
    {
        TryGetPersonInfo(out string? shiftLeaderId, out RoleState? _);

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

    [OnlyRole(RoleState.Worker)]
    [HttpGet]
    public async Task<ActionResult<WorkerShiftPeriodSchedulingResponse>> GetShiftScheduling()
    {
        TryGetPersonInfo(out string? workerId, out RoleState? _);

        WorkerShiftPeriodSchedulingResponse response = await _workerSchedulerService.GetWorkerShiftPeriodCurrentAndNextScheduling(workerId);


        return Ok(response);
    }

}
