using System;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ShiftAssignerServer.Models;
using ShiftAssignerServer.Models.Stuff;
using ShiftAssignerServer.Requests;
using ShiftAssignerServer.Services;

namespace ShiftAssignerServer.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class AuthController : ControllerBase
    {
        public const string Register_Tenant = "register-boss-tenant";

        private readonly JwtService _jwt;
        private readonly IMapper _mapper;
        private readonly ITenantService _tenantService;
        private readonly IShiftLeaderService _shiftLeaderService;
        private readonly IWorkerService _workerService;
        private readonly IShiftAssignmentService _shiftAssignmentService;

        public AuthController(JwtService jwt,
            IMapper mapper,
         ITenantService tenantService,
         IShiftLeaderService shiftLeaderService,
         IWorkerService workerService,
         IShiftAssignmentService shiftAssignmentService
         )
        {
            _jwt = jwt;
            this._mapper = mapper;
            _tenantService = tenantService;
            _shiftLeaderService = shiftLeaderService;
            _workerService = workerService;
            _shiftAssignmentService = shiftAssignmentService;
        }

        [HttpPost("register-worker")]
        public async Task<ActionResult<RegisterResponse>> RegisterWorker([FromBody] RegisterRequest dto)
        {
            // Create typed Worker instance (constructor expects role and passwordHash)
            var pwHash = Hash(dto.PasswordHash);

            var worker = _mapper.Map<Worker>(dto);
            // var worker = new Worker(dto.ID, dto.FirstName, dto.LastName, dto.PhoneNumber, dto.DateOfBirth, dto.Tenant, RoleState.Worker, pwHash);
            // _store.Add(worker, pwHash);
            bool flag = await _workerService.AddWorker(worker);
            var role = worker.Role.ToString(); // "Worker"
            var token = _jwt.GenerateToken(worker.ID, role, worker.Tenant);

            // If the registration included a supervising shift leader, create an initial assignment
            if (!string.IsNullOrWhiteSpace(dto.ShiftLeaderId))
            {
                var assignment = new ShiftAssignment
                {
                    WorkerId = worker.ID,
                    ShiftLeaderId = dto.ShiftLeaderId,
                    PeriodStart = DateOnly.FromDateTime(DateTime.UtcNow),
                    PeriodEnd = null,
                    Notes = "Assigned on registration"
                };

                await _shiftAssignmentService.AssignAsync(assignment);
            }

            return Ok(new RegisterResponse { Token = token });
        }

        [HttpPost("register-shift-leader")]
        public async Task<ActionResult<RegisterResponse>> RegisterShiftLeader([FromBody] RegisterRequest dto)
        {
            // Debugger.Break();
            var pwHash = Hash(dto.PasswordHash);
            var leader = _mapper.Map<ShiftLeader>(dto);
            leader.Role = RoleState.ShiftLeader;
            // _store.Add(leader, pwHash);


            bool flag = await _shiftLeaderService.AddTenantAsync(leader);

            var role = leader.Role.ToString(); // "ShiftLeader"
            var token = _jwt.GenerateToken(leader.ID, role, leader.Tenant);
            return Ok(new RegisterResponse { Token = token });
        }

        [HttpPost(Register_Tenant)]
        public async Task<ActionResult<TenantRegisterResponse>> RegisterBossTenant([FromBody] TenantRegisterRequest dto)
        {
            // Debugger.Break();
            var pwHash = Hash(dto.PasswordHash);
            var tenant = _mapper.Map<BossTenant>(dto);

            await _tenantService.AddTenantAsync(tenant.Tenant);

            var role = tenant.Role.ToString(); // "ShiftLeader"
            var token = _jwt.GenerateToken(tenant.ID, role, tenant.Tenant);
            return Ok(new TenantRegisterResponse { Token = token });
        }

        // ---------------------------------------------------------------------------------------------------------------
        // ---------------------------------------------------------------------------------------------------------------
        // ---------------------------------------------------------------------------------------------------------------

        private static string Hash(string input)
        {
            // Simple SHA256 hash for demo purposes (not salted). Do NOT use in production.
            using var sha = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(input ?? string.Empty);
            var hash = sha.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }
    }
}
