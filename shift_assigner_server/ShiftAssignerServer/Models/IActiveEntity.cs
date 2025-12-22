namespace ShiftAssignerServer.Models;

/// <summary>
/// Base interface for entities that support soft delete functionality through IsActive flag
/// </summary>
public interface IActiveEntity
{
    /// <summary>
    /// Indicates whether this entity is active (not soft deleted)
    /// </summary>
    bool IsActive { get; set; }
}