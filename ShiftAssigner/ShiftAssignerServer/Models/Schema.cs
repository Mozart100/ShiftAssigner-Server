using ShiftAssignerServer.Common;

namespace ShiftAssignerServer.Models;

/// <summary>
/// Tenant (company) model. Implements <see cref="IAutoMapperEntities"/>
/// so it can be used with repository and mapping infrastructure.
/// </summary>
public class Schema : IAutoMapperEntities, IActiveEntity
{
    // public int ID { get; set; }  

    // Non-nullable string - provide a default to satisfy nullable reference checks.
    public string CompanyName { get; set; } = string.Empty;

    /// <summary>
    /// Soft delete flag. When false, the entity is considered logically deleted.
    /// </summary>
    public bool IsActive { get; set; } = true;
}
