using System;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
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

        public AuthController(JwtService jwt,
         InMemoryUserStore store 
         ,        IMapper mapper
         )
        {
            _jwt = jwt;
            _store = store;
            this._mapper = mapper;
        }

        [HttpPost("register-worker")]
        public ActionResult<RegisterResponse> RegisterWorker([FromBody] RegisterRequest dto)
        {
            // Create typed Worker instance (constructor expects role and passwordHash)
            var pwHash = Hash(dto.PasswordHash);

            var worker =_mapper.Map<Worker>(dto);
            // var worker = new Worker(dto.ID, dto.FirstName, dto.LastName, dto.PhoneNumber, dto.DateOfBirth, dto.Tenant, RoleState.Worker, pwHash);
            // _store.Add(worker, pwHash);

            var role = worker.Role.ToString(); // "Worker"
            var token = _jwt.GenerateToken(worker.ID, role, worker.Tenant);
            return Ok(new RegisterResponse { Token = token });
        }

        [HttpPost("register-shiftleader")]
        public ActionResult<RegisterResponse> RegisterShiftLeader([FromBody] RegisterRequest dto)
        {
            // Debugger.Break();
            var pwHash = Hash(dto.PasswordHash);
            var leader =_mapper.Map<ShiftLeader>(dto);
            // _store.Add(leader, pwHash);

            var role = leader.Role.ToString(); // "ShiftLeader"
            var token = _jwt.GenerateToken(leader.ID, role, leader.Tenant);
            return Ok(new RegisterResponse { Token = token });
        }

         [HttpPost("register-boss-tenant")]
        public ActionResult<RegisterResponse> RegisterBossTenant([FromBody] RegisterRequest dto)
        {
            // Debugger.Break();
            var pwHash = Hash(dto.PasswordHash);
            var leader =_mapper.Map<BossTenant>(dto);
            // _store.Add(leader, pwHash);

            var role = leader.Role.ToString(); // "ShiftLeader"
            var token = _jwt.GenerateToken(leader.ID, role, leader.Tenant);
            return Ok(new RegisterResponse { Token = token });
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
