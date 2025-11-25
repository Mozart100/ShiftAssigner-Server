using Microsoft.AspNetCore.Mvc;
using ShiftAssignerServer.Requests;

namespace ShiftAssignerServer.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class TenantsController : ControllerBase
{
    private readonly ITenantService _tenantService;

    public TenantsController(ITenantService tenantService)
    {
        _tenantService = tenantService;
    }

    [HttpGet]
    public async Task<ActionResult<AllTenantsResponse>> GetAll()
    {
        var tenants = await _tenantService.GetAllTenantsAsync();
        return Ok(tenants);
    }
}
