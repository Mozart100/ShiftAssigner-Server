using System;
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
        public IActionResult RegisterWorker([FromBody] RegisterDto dto)
        {
            var user = new UserRecord
            {
                Id = dto.ID,
                Role = "Worker",
                Tenant = dto.Tenant,
                PasswordHash = Hash(dto.Password),
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                PhoneNumber = dto.PhoneNumber,
                DateOfBirth = dto.DateOfBirth
            };

            _store.Add(user);

            var token = _jwt.GenerateToken(user.Id, user.Role, user.Tenant);
            return Ok(new { token });
        }

        [HttpPost("register-shiftleader")]
        public IActionResult RegisterShiftLeader([FromBody] RegisterDto dto)
        {
            var user = new UserRecord
            {
                Id = dto.ID,
                Role = "ShiftLeader",
                Tenant = dto.Tenant,
                PasswordHash = Hash(dto.Password),
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                PhoneNumber = dto.PhoneNumber,
                DateOfBirth = dto.DateOfBirth
            };

            _store.Add(user);

            var token = _jwt.GenerateToken(user.Id, user.Role, user.Tenant);
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
