using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShiftAssignerServer.Middleware;
using ShiftAssignerServer.Models;
using ShiftAssignerServer.Models.Stuff;
using ShiftAssignerServer.Requests;
using ShiftAssignerServer.Services;

namespace ShiftAssignerServer.Controllers;

[Authorize]
[ApiController]
[Route("api/v1/[controller]")]
public class ShiftLeadersController : ControllerBase
{
    private readonly IShiftLeaderService _shiftLeaderService;
    private readonly IMapper _mapper;
    private readonly JwtService _jwtService;

    public ShiftLeadersController(IShiftLeaderService service, IMapper mapper, JwtService jwtService)
    {
        _shiftLeaderService = service;
        _mapper = mapper;
        _jwtService = jwtService;
    }

    // GET: api/v1/ShiftLeaders/{tenant}
    [HttpGet("{tenant}")]
    public async Task<ActionResult<GetShiftLeaderPerTenantResponse>> GetAllPerTenant(string tenant)
    {
        var leaders = await _shiftLeaderService.GetAllShiftLeaderAsync(tenant);
        return Ok(new GetShiftLeaderPerTenantResponse { ShifLeaders = leaders });
    }



    [Authorize]
    [HttpPost("adding-shift-leader")]
    public async Task<ActionResult<AddingShiftLeaderResponse>> RegisterShiftLeader([FromBody] AddingShiftLeaderRequest dto)
    {
        // Debugger.Break();
        var leader = _mapper.Map<ShiftLeader>(dto);
        leader.Role = RoleState.ShiftLeader;

        // Get tenant from TenantResolutionMiddleware
        var tenant = HttpContext.Items[TenantResolutionMiddleware.TenantContextKey]?.ToString();
        
        // Tenant is now handled by the tenant-specific database schema, not as a property

        bool flag = await _shiftLeaderService.AddShiftLeaderAsync(leader);

        var role = leader.Role.ToString(); // "ShiftLeader"
        var token = _jwtService.GenerateToken(leader.ID, role, tenant ?? string.Empty);
        return Ok(new AddingShiftLeaderResponse { Token = token });
    }


}
