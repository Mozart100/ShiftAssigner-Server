using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using ShiftAssignerServer.Models.Stuff;
using ShiftAssignerServer.Services;

namespace ShiftAssignerServer.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class ShiftLeadersController : ControllerBase
{
    private readonly InMemoryUserStore _store;

    public ShiftLeadersController(InMemoryUserStore store)
    {
        _store = store;
    }

    // GET: api/v1/ShiftLeaders
    [HttpGet]
    public ActionResult<IEnumerable<ShiftLeader>> GetAll()
    {
        var all = _store.GetAll();
        var leaders = all.OfType<ShiftLeader>().ToList();
        return Ok(leaders);
    }
}
