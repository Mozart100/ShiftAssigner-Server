using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShiftAssignerServer.Models;
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
    
    public WorkerSchedulerController(IWorkerSchedulerService workerSchedulerService, JwtService jwtService)
        : base(jwtService)
    {
        _workerSchedulerService = workerSchedulerService ?? throw new ArgumentNullException(nameof(workerSchedulerService));
    }

    // GET: api/v1/WorkerScheduler/schedule/{workerId}
    [HttpGet(GetScheduleRoute)]
    public async Task<ActionResult<WorkerScheduleResponse>> GetWorkerSchedule(string workerId)
    {
        // Extract worker/shift leader info from JWT token
        if (!TryGetUserInfo(out string? userId, out RoleState? role))
        {
            return Unauthorized("Valid authentication required");
        }

        // Allow workers to view their own schedule or shift leaders to view their team's schedules
        if (role == RoleState.Worker && userId != workerId)
        {
            return Forbid("Workers can only view their own schedule");
        }

        // TODO: Implement get worker schedule logic
        var response = new WorkerScheduleResponse
        {
            WorkerId = workerId,
            Message = "Worker schedule retrieval placeholder - implementation pending"
        };

        return Ok(response);
    }

    // POST: api/v1/WorkerScheduler/availability
    [HttpPost(UpdateAvailabilityRoute)]
    public async Task<ActionResult<UpdateAvailabilityResponse>> UpdateAvailability([FromBody] UpdateAvailabilityRequest request)
    {
        // Extract worker info from JWT token
        if (!TryGetUserInfo(out string? workerId, out RoleState? role))
        {
            return Unauthorized("Valid worker authentication required");
        }

        // Only workers can update their own availability
        if (role != RoleState.Worker)
        {
            return Forbid("Only workers can update availability");
        }

        // Validate request
        if (request == null)
        {
            return BadRequest("Update availability request is required");
        }

        // TODO: Implement availability update logic
        var response = new UpdateAvailabilityResponse
        {
            WorkerId = workerId,
            Message = "Availability update placeholder - implementation pending"
        };

        return Ok(response);
    }

    // POST: api/v1/WorkerScheduler/time-off
    [HttpPost(RequestTimeOffRoute)]
    public async Task<ActionResult<TimeOffRequestResponse>> RequestTimeOff([FromBody] TimeOffRequest request)
    {
        // Extract worker info from JWT token
        if (!TryGetUserInfo(out string? workerId, out RoleState? role))
        {
            return Unauthorized("Valid worker authentication required");
        }

        // Only workers can request time off
        if (role != RoleState.Worker)
        {
            return Forbid("Only workers can request time off");
        }

        // Validate request
        if (request?.StartDate == default || request?.EndDate == default)
        {
            return BadRequest("Valid start and end dates are required");
        }

        if (request.StartDate > request.EndDate)
        {
            return BadRequest("Start date cannot be after end date");
        }

        // TODO: Implement time off request logic
        var response = new TimeOffRequestResponse
        {
            RequestId = Guid.NewGuid().ToString(),
            WorkerId = workerId,
            Status = "Pending",
            Message = "Time off request submitted successfully"
        };

        return Ok(response);
    }

    // POST: api/v1/WorkerScheduler/swap-shift
    [HttpPost(SwapShiftRoute)]
    public async Task<ActionResult<ShiftSwapResponse>> RequestShiftSwap([FromBody] ShiftSwapRequest request)
    {
        // Extract worker info from JWT token
        if (!TryGetUserInfo(out string? workerId, out RoleState? role))
        {
            return Unauthorized("Valid worker authentication required");
        }

        // Only workers can request shift swaps
        if (role != RoleState.Worker)
        {
            return Forbid("Only workers can request shift swaps");
        }

        // Validate request
        if (string.IsNullOrWhiteSpace(request?.TargetWorkerId))
        {
            return BadRequest("Target worker ID is required");
        }

        if (request.WorkerId == request.TargetWorkerId)
        {
            return BadRequest("Cannot swap shift with yourself");
        }

        // TODO: Implement shift swap request logic
        var response = new ShiftSwapResponse
        {
            SwapRequestId = Guid.NewGuid().ToString(),
            FromWorkerId = workerId,
            ToWorkerId = request.TargetWorkerId,
            Status = "Pending",
            Message = "Shift swap request submitted successfully"
        };

        return Ok(response);
    }

    private bool TryGetUserInfo(out string? userId, out RoleState? role)
    {
        userId = null;
        role = null;

        // Try worker first
        // if (TryGetWorkerInfo(out string? workerId, out RoleState? workerRole))
        // {
        //     userId = workerId;
        //     role = workerRole;
        //     return true;
        // }

        // Try shift leader
        if (TryGetShiftLeaderInfo(out string? shiftLeaderId, out RoleState? leaderRole))
        {
            userId = shiftLeaderId;
            role = leaderRole;
            return true;
        }

        return false;
    }
}

// Request/Response DTOs for WorkerScheduler
public class WorkerScheduleResponse
{
    public string WorkerId { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    // TODO: Add schedule data properties
}

public class UpdateAvailabilityRequest
{
    public List<AvailabilitySlot> AvailableSlots { get; set; } = new();
    public string Notes { get; set; } = string.Empty;
}

public class AvailabilitySlot
{
    public DayOfWeek DayOfWeek { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public bool IsAvailable { get; set; } = true;
}

public class UpdateAvailabilityResponse
{
    public string WorkerId { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}

public class TimeOffRequest
{
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public string Reason { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
}

public class TimeOffRequestResponse
{
    public string RequestId { get; set; } = string.Empty;
    public string WorkerId { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}

public class ShiftSwapRequest
{
    public string WorkerId { get; set; } = string.Empty;
    public string TargetWorkerId { get; set; } = string.Empty;
    public DateOnly ShiftDate { get; set; }
    public string ShiftName { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
}

public class ShiftSwapResponse
{
    public string SwapRequestId { get; set; } = string.Empty;
    public string FromWorkerId { get; set; } = string.Empty;
    public string ToWorkerId { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}