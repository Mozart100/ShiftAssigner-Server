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
        private readonly JwtService _jwt;
        private readonly InMemoryUserStore _store;
        private readonly IMapper _mapper;
        private readonly ITenantService _tenantService;

        public AuthController(JwtService jwt,
         InMemoryUserStore store
         , IMapper mapper,
         ITenantService tenantService
         )
        {
            _jwt = jwt;
            _store = store;
            this._mapper = mapper;
            _tenantService = tenantService;
        }

        [HttpPost("register-worker")]
        public ActionResult<RegisterResponse> RegisterWorker([FromBody] RegisterRequest dto)
        {
            // Create typed Worker instance (constructor expects role and passwordHash)
            var pwHash = Hash(dto.PasswordHash);

            var worker = _mapper.Map<Worker>(dto);
            // var worker = new Worker(dto.ID, dto.FirstName, dto.LastName, dto.PhoneNumber, dto.DateOfBirth, dto.Tenant, RoleState.Worker, pwHash);
            // _store.Add(worker, pwHash);

            var role = worker.Role.ToString(); // "Worker"
            var token = _jwt.GenerateToken(worker.ID, role, worker.Tenant);
            return Ok(new RegisterResponse { Token = token });
        }

        [HttpPost("register-shift-leader")]
        public ActionResult<RegisterResponse> RegisterShiftLeader([FromBody] RegisterRequest dto)
        {
            // Debugger.Break();
            var pwHash = Hash(dto.PasswordHash);
            var leader = _mapper.Map<ShiftLeader>(dto);
            // _store.Add(leader, pwHash);

            var role = leader.Role.ToString(); // "ShiftLeader"
            var token = _jwt.GenerateToken(leader.ID, role, leader.Tenant);
            return Ok(new RegisterResponse { Token = token });
        }

        [HttpPost("register-boss-tenant")]
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
