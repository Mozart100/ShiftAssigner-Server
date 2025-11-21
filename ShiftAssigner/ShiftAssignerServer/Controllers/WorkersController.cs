using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ShiftAssignerServer.Models.Stuff;
using ShiftAssignerServer.Services;

namespace ShiftAssignerServer.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class WorkersController : ControllerBase
{
    private readonly IWorkerService _service;

    public WorkersController(IWorkerService service)
    {
        _service = service;
    }

    // GET: api/v1/Workers/{tenant}
    [HttpGet("{tenant}")]
    public async Task<ActionResult<IEnumerable<Worker>>> GetAllPerTenant(string tenant)
    {
        var workers = await _service.GetAllAsync(tenant);
        return Ok(workers);
    }
}
