using System;
using ShiftAssignerServer.Repositories;

namespace ShiftAssignerServer.Models;

/// <summary>
/// Tenant (company) model. Implements <see cref="IAutoMapperEntities"/>
/// so it can be used with repository and mapping infrastructure.
/// </summary>
public class Tenant : IAutoMapperEntities
{
    public int ID { get; set; }

    // Non-nullable string - provide a default to satisfy nullable reference checks.
    public string CompanyName { get; set; } = string.Empty;
}
