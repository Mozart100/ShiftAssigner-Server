using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShiftAssignerServer.Middleware;
using ShiftAssignerServer.Models.Stuff;
using ShiftAssignerServer.Requests;
using ShiftAssignerServer.Services;
using ShiftAssignerServer.Repositories;
using AutoMapper;
using ShiftAssignerServer.Models;

namespace ShiftAssignerServer.Controllers;

[Authorize]
[ApiController]
[Route("api/v1/[controller]")]
public class WorkersController : BaseController
{
    public const string Register_EndPoint = "register";
    public const string Login_EndPoint = "login";

    private readonly IWorkerService _workerService;
    private readonly IStuffBookingService _assignmentService;
    private readonly IMapper _mapper;

    public WorkersController(IWorkerService service, IStuffBookingService assignmentService,IMapper mapper, JwtService jwtService)
        : base(jwtService)
    {
        _workerService = service;
        _assignmentService = assignmentService;
        _mapper = mapper;
    }


    [Authorize]
    [HttpPost(Register_EndPoint)]
    public async Task<ActionResult<RegisteringWorkerResponse>> Registering([FromBody] RegisteringWorkerRequest request)
    {
        // Debugger.Break();
        var worker = _mapper.Map<Worker>(request);
        worker.Role = RoleState.Worker;

        // Get tenant from TenantResolutionMiddleware via base controller
        var tenant = GetTenant();

        // Tenant is now handled by the tenant-specific database schema, not as a property

        bool flag = await _workerService.AddWorkerAsync(worker);

        var role = worker.Role.ToString(); // "Worker"
        var token = _jwtService.GenerateToken(worker.ID, role, GetTenantOrEmpty());
        return Ok(new RegisteringWorkerResponse { Token = token });
    }

    [Authorize]
    [HttpPost(Login_EndPoint)]
    public async Task<ActionResult<LoginWorkerResponse>> LoginWorker([FromBody] LoginWorkerRequest request)
    {
        // Get tenant from TenantResolutionMiddleware via base controller
        var tenant = GetTenant();

        bool success = await _workerService.LoginAsync(request);

        if (!success)
        {
            return NotFound("Worker not found");
        }

        var role = "Worker";
        var token = _jwtService.GenerateToken(request.ID, role, GetTenantOrEmpty());
        return Ok(new LoginWorkerResponse { Token = token });
    }


    // GET: api/v1/Workers/{tenant}
    // [HttpGet("{tenant}")]
    // public async Task<ActionResult<GetWorkerPerTenantResponse>> GetAllPerTenant(string tenant)
    // {
    //     var workers = await _service.GetAllActiveWorkersPerShiftLeaderAsync(tenant);
    //     return Ok(new GetWorkerPerTenantResponse { Workers = workers });
    // }

    // // GET: api/v1/Workers/leader/{shiftLeaderId}
    // // If a period query parameter is supplied (yyyy-MM-dd) it is used; otherwise the current UTC date is used.
    // [HttpGet("leader/{shiftLeaderId}")]
    // public async Task<ActionResult<GetWorkerPerTenantResponse>> GetWorkersForLeaderAndPeriod(string shiftLeaderId, string period)
    // {
    //     // Find leader to determine tenant
    //     var leader = _shiftLeaderRepository.FirstOrDefault(x => x.ID.Equals(shiftLeaderId, StringComparison.InvariantCultureIgnoreCase));
    //     if (leader is null) return NotFound();

    //     DateOnly periodStart;
    //     if (string.IsNullOrWhiteSpace(period)) periodStart = DateOnly.FromDateTime(DateTime.UtcNow);
    //     else if (!DateOnly.TryParse(period, out periodStart)) return BadRequest("period must be yyyy-MM-dd");

    //     // Get tenant from middleware since ShiftLeader no longer has Tenant property
    //     var tenant = HttpContext.Items[TenantResolutionMiddleware.TenantContextKey]?.ToString() ?? string.Empty;
    //     var workers = await _assignmentService.GetWorkersForLeaderPeriodAsync(tenant, shiftLeaderId, periodStart);
    //     return Ok(new GetWorkerPerTenantResponse { Workers = workers });
    // }

    // // POST: api/v1/Workers/retire
    // [HttpPost("retire")]
    // public async Task<IActionResult> RetireWorker([FromBody] RetireWorkerRequest request)
    // {
    //     if (string.IsNullOrWhiteSpace(request.WorkerId) || string.IsNullOrWhiteSpace(request.Tenant))
    //     {
    //         return BadRequest("workerId and tenant are required");
    //     }

    //     var result = await _service.RetireWorkerAsync(request.Tenant, request.WorkerId);

    //     if (result)
    //     {
    //         return Ok(new { Message = "Worker retired successfully", WorkerId = request.WorkerId });
    //     }

    //     return BadRequest("Failed to retire worker");
    // }
}
