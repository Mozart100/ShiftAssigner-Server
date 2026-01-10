using System.Security.Cryptography;
using System.Text;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShiftAssignerServer.Models;
using ShiftAssignerServer.Models.Stuff;
using ShiftAssignerServer.Requests;
using ShiftAssignerServer.Services;
using ShiftAssignerServer.Services.Validation;
using ShiftAssignerServer.Repositories;
using ShiftAssignerServer.Models.WorkerScheduling;

namespace ShiftAssignerServer.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class AuthController : TenantControllerBase
    {
        public const string Register_Tenant = "register-boss-tenant";

        private readonly JwtService _jwt;
        private readonly IMapper _mapper;
        private readonly ISchemaManagerService _tenantService;
        private readonly IMainSchemaService _mainSchemaService;
        private readonly IShiftLeaderService _shiftLeaderService;
        private readonly IWorkerService _workerService;
        private readonly ITeamHierarchyService _shiftAssignmentService;
        private readonly IShiftLeaderRepository _shiftLeaderRepository;
        private readonly IRegistrationValidationService _validationService;

        public AuthController(JwtService jwt,
            IMapper mapper,
            ISchemaManagerService tenantService,
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



        [AllowAnonymous]
        [HttpPost(Register_Tenant)]
        public async Task<ActionResult<TenantRegisterResponse>> RegisterBossTenant([FromBody] TenantRegisterRequest request)
        {
            // 🔍 DEBUG: Log the received request
            Console.WriteLine($"🔍 CONTROLLER DEBUG: Received request for tenant: {request?.Tenant ?? "NULL"}");
            Console.WriteLine($"🔍 CONTROLLER DEBUG: Request ID: {request?.ID ?? "NULL"}");
            Console.WriteLine($"🔍 CONTROLLER DEBUG: Request FirstName: {request?.FirstName ?? "NULL"}");
            Console.WriteLine($"🔍 CONTROLLER DEBUG: Shifts count: {request?.Shifts?.Count ?? 0}");
            
            // Check ModelState for validation errors
            if (!ModelState.IsValid)
            {
                Console.WriteLine("❌ CONTROLLER DEBUG: ModelState is INVALID");
                foreach (var error in ModelState)
                {
                    Console.WriteLine($"❌ Field: {error.Key}, Errors: {string.Join(", ", error.Value.Errors.Select(e => e.ErrorMessage))}");
                }
                return BadRequest(ModelState);
            }
            
            Console.WriteLine("✅ CONTROLLER DEBUG: ModelState is VALID, proceeding...");
            
            // Debugger.Break();
            var tenant = _mapper.Map<BossTenant>(request);
            var tenantShiftScheduling = _mapper.Map<TenantShiftScheduling>(request);

            tenant.Role = RoleState.Boss;

            // Create tenant schema in the database
            await _tenantService.CreateIfNoxExistedTenantSchemaAsync(tenant.Tenant);
            
            // Register the boss tenant in the tenant-specific schema
            bool flag = await _tenantService.AddBossTenantWithSchedulingAsync(tenant,tenantShiftScheduling);
            
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
