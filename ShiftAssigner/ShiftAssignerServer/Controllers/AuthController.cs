using System;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using ShiftAssignerServer.Models;
using ShiftAssignerServer.Services;

namespace ShiftAssignerServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly JwtService _jwt;
        private readonly InMemoryUserStore _store;

        public AuthController(JwtService jwt, InMemoryUserStore store)
        {
            _jwt = jwt;
            _store = store;
        }

        [HttpPost("register-worker")]
        public ActionResult<string> RegisterWorker([FromBody] RegisterRequest dto)
        {
            Debugger.Break();
            // Create typed Worker instance (constructor expects role and passwordHash)
            var pwHash = Hash(dto.Password);
            var worker = new Worker(dto.ID, dto.FirstName, dto.LastName, dto.PhoneNumber, dto.DateOfBirth, dto.Tenant, RoleState.Worker, pwHash);
            _store.Add(worker, pwHash);

            var role = worker.Role.ToString(); // "Worker"
            var token = _jwt.GenerateToken(worker.Id, role, worker.Tenant);
            return Ok(new { token });
        }

        [HttpPost("register-shiftleader")]
        public ActionResult<string> RegisterShiftLeader([FromBody] RegisterRequest dto)
        {
            Debugger.Break();
            var pwHash = Hash(dto.Password);
            var leader = new ShiftLeader(dto.ID,dto.FirstName, dto.LastName, dto.PhoneNumber, dto.DateOfBirth, dto.Tenant, RoleState.ShiftLeader, pwHash);
            _store.Add(leader, pwHash);

            var role = leader.Role.ToString(); // "ShiftLeader"
            var token = _jwt.GenerateToken(leader.Id, role, leader.Tenant);
            return Ok(new { token });
        }

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
