using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using ShiftAssignerServer.Models.Stuff;
using ShiftAssignerServer.Requests;
using ShiftAssignerServer.Services;

namespace ShiftAssignerServer.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class ShiftLeadersController : ControllerBase
{
    private readonly IShiftLeaderService _service;

    public ShiftLeadersController(IShiftLeaderService service)
    {
        _service = service;
    }

    // GET: api/v1/ShiftLeaders/{tenant}
    [HttpGet("{tenant}")]
    public async Task<ActionResult<GetShiftLeaderPerTenantResponse>> GetAllPerTenant(string tenant)
    {
        var leaders = await _service.GetAllAsync(tenant);
        return Ok(new GetShiftLeaderPerTenantResponse{ ShifLeaders = leaders });
    }
}
