using System;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShiftAssignerServer.Middleware;
using ShiftAssignerServer.Models;
using ShiftAssignerServer.Models.Stuff;
using ShiftAssignerServer.Requests;
using ShiftAssignerServer.Services;
using ShiftAssignerServer.Services.Validation;
using ShiftAssignerServer.Repositories;

namespace ShiftAssignerServer.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class AuthController : TenantControllerBase
    {
        public const string Register_Tenant = "register-boss-tenant";

        private readonly JwtService _jwt;
        private readonly IMapper _mapper;
        private readonly ISchemamanagerService _tenantService;
        private readonly IMainSchemaService _mainSchemaService;
        private readonly IShiftLeaderService _shiftLeaderService;
        private readonly IWorkerService _workerService;
        private readonly ITeamHierarchyService _shiftAssignmentService;
        private readonly IShiftLeaderRepository _shiftLeaderRepository;
        private readonly IRegistrationValidationService _validationService;

        public AuthController(JwtService jwt,
            IMapper mapper,
            ISchemamanagerService tenantService,
            IMainSchemaService mainSchemaService,
            IShiftLeaderService shiftLeaderService,
            IWorkerService workerService,
            ITeamHierarchyService shiftAssignmentService,
            IShiftLeaderRepository shiftLeaderRepository,
            IRegistrationValidationService validationService)
            : base(jwt)
        {
            _jwt = jwt;
            _mapper = mapper;
            _tenantService = tenantService;
            _mainSchemaService = mainSchemaService;
            _shiftLeaderService = shiftLeaderService;
            _workerService = workerService;
            _shiftAssignmentService = shiftAssignmentService;
            _shiftLeaderRepository = shiftLeaderRepository;
            _validationService = validationService;
        }

        [Authorize]
        [HttpPost("register-worker")]
        public async Task<ActionResult<RegisterResponse>> RegisterWorker([FromBody] RegisterRequest dto)
        {
            // Validate the registration request
            _validationService.ValidateRegistration(dto, "Worker");

            // Create typed Worker instance (constructor expects role and passwordHash)

            var worker = _mapper.Map<Worker>(dto);
            // var worker = new Worker(dto.ID, dto.FirstName, dto.LastName, dto.PhoneNumber, dto.DateOfBirth, dto.Tenant, RoleState.Worker, pwHash);
            // _store.Add(worker, pwHash);
            bool flag = await _workerService.AddWorkerAsync(worker);
            var role = worker.Role.ToString(); // "Worker"

            // Determine tenant for the token. Workers no longer carry tenant; if the DTO included a ShiftLeaderId
            // we derive the tenant from that leader and create an initial assignment.
            string tenantForToken = string.Empty;
            if (!string.IsNullOrWhiteSpace(dto.ShiftLeaderId))
            {
                // try to find the leader's tenant and set it on the assignment and token
                var leader = _shiftLeaderRepository.FirstOrDefault(x => x.ID.Equals(dto.ShiftLeaderId, StringComparison.InvariantCultureIgnoreCase));
                tenantForToken = GetTenantOrEmpty();

                var assignment = new TeamHierarchy
                {
                    WorkerId = worker.ID,
                    ShiftLeaderId = dto.ShiftLeaderId,
                    PeriodStart = DateOnly.FromDateTime(DateTime.UtcNow),
                    PeriodEnd = null,
                    Notes = "Assigned on registration"
                };

                await _shiftAssignmentService.AssignAsync(assignment);
            }

            var token = _jwt.GenerateToken(worker.ID, role, tenantForToken);
            return Ok(new RegisterResponse { Token = token });
        }


        [AllowAnonymous]
        [HttpPost(Register_Tenant)]
        public async Task<ActionResult<TenantRegisterResponse>> RegisterBossTenant([FromBody] TenantRegisterRequest request)
        {
            // Debugger.Break();
            var tenant = _mapper.Map<BossTenant>(request);

            // Create tenant schema in the database
            await _tenantService.CreateIfNoxExistedTenantSchemaAsync(tenant.Tenant);
            
            // Register the boss tenant in the tenant-specific schema
            bool flag = await _tenantService.AddBossTenantAsync(request);
            
            // Add schema entry to the main schema registry
            await _mainSchemaService.AddTenantSchemaAsync(tenant.Tenant);

            var role = tenant.Role.ToString(); // "ShiftLeader"
            var token = _jwt.GenerateToken(tenant.ID, role, tenant.Tenant);
            return Ok(new TenantRegisterResponse { Token = token, Tenant = tenant.Tenant });
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
