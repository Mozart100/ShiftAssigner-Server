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
using ShiftAssignerServer.Services.Validation;
using ShiftAssignerServer.Controllers.Attributes;

namespace ShiftAssignerServer.Controllers;

[Authorize]
[ApiController]
[Route("api/v1/[controller]")]
public class WorkersController : TenantControllerBase
{
    public const string Register_EndPoint = "register";
    public const string Login_EndPoint = "login";

    private readonly IWorkerService _workerService;
    private readonly IWorkersServiceValidation _workersServiceValidation;
    private readonly ITeamHierarchyService _assignmentService;
    private readonly IMapper _mapper;

    public WorkersController(IWorkerService service, 
    IWorkersServiceValidation workersServiceValidation,
    ITeamHierarchyService assignmentService,IMapper mapper, JwtService jwtService)
        : base(jwtService)
    {
        _workerService = service;
        _workersServiceValidation = workersServiceValidation;
        _assignmentService = assignmentService;
        _mapper = mapper;
    }


    [Authorize]
    [OnlyRole(RoleState.ShiftLeader)]
    [HttpPost(Register_EndPoint)]
    public async Task<ActionResult<RegisteringWorkerResponse>> Registering([FromBody] WorkerRegisteringRequest request)
    {
        // Debugger.Break();
        var worker = _mapper.Map<Worker>(request);
        worker.Role = RoleState.Worker;

        // Get tenant from TenantResolutionMiddleware via base controller
        var tenant = GetTenant();

        TryGetPersonInfo(out string? shiftLeaderId, out RoleState? _);


        // Register the worker
        bool workerAdded = await _workerService.AddWorkerAsync(worker);
        if (!workerAdded)
        {
            return BadRequest("Failed to register worker");
        }

        // Create a booking to assign the worker to the shift leader
        var booking = new TeamHierarchy
        {
            WorkerId = worker.ID,
            ShiftLeaderId = shiftLeaderId,
            PeriodStart = DateOnly.FromDateTime(DateTime.UtcNow),
            PeriodEnd = null, // Open-ended assignment
            Notes = "Initial assignment during worker registration",
            IsActive = true
        };

        bool bookingAdded = await _assignmentService.AssignAsync(booking);
        if (!bookingAdded)
        {
            return BadRequest("Worker registered but failed to assign to shift leader");
        }

        var workerRole = worker.Role.ToString(); // "Worker"
        var token = _jwtService.GenerateToken(worker.ID, workerRole, GetTenantOrEmpty());
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
}
