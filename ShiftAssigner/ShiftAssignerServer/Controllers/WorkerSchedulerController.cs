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

        // Validate request
        if (request == null)
        {
            return BadRequest("Create shift period request is required");
        }

        if (request.StartFrom == default)
        {
            return BadRequest("Start date is required");
        }

        if (request.NextPeriod == null || !request.NextPeriod.Any())
        {
            return BadRequest("NextPeriod with at least one day is required");
        }

        // Validate each day has shifts
        foreach (var day in request.NextPeriod)
        {
            if (day.Shifts == null || !day.Shifts.Any())
            {
                return BadRequest($"Day {day.Date} must have at least one shift");
            }

            foreach (var shift in day.Shifts)
            {
                if (string.IsNullOrWhiteSpace(shift.ShiftName))
                {
                    return BadRequest("Shift name is required");
                }

                if (shift.AmountOfWorkers <= 0)
                {
                    return BadRequest("Amount of workers must be greater than 0");
                }
            }
        }

        try
        {
            var success = await _workerSchedulerService.CreateNewWorkerRegisteringRequest(request, shiftLeaderId);

            if (success)
            {
                var response = new CreateShiftPeriodSchedulingResponse
                {
                    ShiftLeaderId = shiftLeaderId,
                    StartFrom = request.StartFrom,
                    Success = true,
                    Message = "Shift period created successfully"
                };
                return Ok(response);
            }
            else
            {
                return BadRequest(new CreateShiftPeriodSchedulingResponse
                {
                    Success = false,
                    Message = "Failed to create shift period"
                });
            }
        }
        catch (Exception ex)
        {
            return BadRequest(new CreateShiftPeriodSchedulingResponse
            {
                Success = false,
                Message = $"Failed to create shift period: {ex.Message}"
            });
        }
    }

}
