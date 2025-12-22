using Microsoft.EntityFrameworkCore;
using ShiftAssignerServer.Models.Stuff;
using ShiftAssignerServer.Models;

namespace ShiftAssignerServer.Data;

/// <summary>
/// A dedicated DbContext for creating tenant-specific schemas and tables.
/// This context has the tenant schema baked in during construction.
/// </summary>
public class TenantCreationDbContext : PureApplicationDbContext
{

    public TenantCreationDbContext(DbContextOptions options, string tenantSchema)
        : base(options)
    {
        TenantSchema = tenantSchema;
    }
}