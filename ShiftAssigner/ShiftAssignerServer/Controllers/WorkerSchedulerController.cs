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
    private const string CreateShiftPeriodRoute = "shift-period";
    private const string GetWorkerScheduleRoute = "active-period/worker";

    private readonly IWorkerSchedulerService _workerSchedulerService;
    public WorkerSchedulerController(IWorkerSchedulerService workerSchedulerService , JwtService jwtService)
        : base(jwtService )
    {
        _workerSchedulerService = workerSchedulerService ;
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
    [HttpGet(GetWorkerScheduleRoute)]
    public async Task<ActionResult<WorkerShiftPeriodSchedulingResponse>> GetWorkerShiftScheduling()
    {
        TryGetPersonInfo(out string? workerId, out RoleState? _);

        var response = await _workerSchedulerService.GetWorkerShiftPeriodCurrentAndNextScheduling(workerId);
        return Ok(response);
    }


    [HttpPut]
    public async Task<ActionResult<WorkerAssigningToPeriodResponse>> WorkerAssignTo(WorkerAssigningToPeriodRequest request)
    {
        TryGetPersonInfo(out string? workerId, out RoleState? _);

        WorkerAssigningToPeriodResponse response = await _workerSchedulerService.WorkerAssigningToPeriod(workerId, request);
        return Ok(response);
    }

}
