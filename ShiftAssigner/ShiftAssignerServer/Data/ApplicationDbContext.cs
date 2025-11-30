using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using ShiftAssignerServer.Models;
using ShiftAssignerServer.Models.Stuff;

namespace ShiftAssignerServer.Data;

public sealed class TenantModelCacheKeyFactory : IModelCacheKeyFactory
{
    public object Create(DbContext context, bool designTime)
    {
        if (context is ApplicationDbContext appContext)
        {
            // Include schema in the cache key so each tenant schema gets its own model
            return (context.GetType(), appContext.TenantSchema, designTime);
        }

        return (context.GetType(), designTime);
    }
}


public class ApplicationDbContext : DbContext
{
    private readonly ITenantProvider _tenantProvider;

    /// <summary>
    /// Current tenant schema for this DbContext instance.
    /// Used to map tenant-specific tables to the correct PostgreSQL schema.
    /// </summary>
    public string TenantSchema { get; }

    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options,
        ITenantProvider tenantProvider)
        : base(options)
    {
        _tenantProvider = tenantProvider;
        TenantSchema = tenantProvider.TenantSchema; // already sanitized
    }

    // DbSets for all entities
    public DbSet<Worker> Workers { get; set; } = null!;
    public DbSet<ShiftLeader> ShiftLeaders { get; set; } = null!;
    public DbSet<StuffBooking> StuffBookings { get; set; } = null!;
    public DbSet<Tenant> Tenants { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Use the tenant-specific schema for tenant-bound entities
        var schema = TenantSchema;

        // Configure Worker entity (base type for TPT inheritance)
        modelBuilder.Entity<Worker>(entity =>
        {
            entity.ToTable("workers", schema);
            entity.HasKey(e => e.ID);
            entity.Property(e => e.ID).IsRequired().HasMaxLength(50);
            entity.Property(e => e.FirstName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.LastName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.PhoneNumber).IsRequired().HasMaxLength(20);
            entity.Property(e => e.PasswordHash).IsRequired();
            entity.Property(e => e.IsActive).IsRequired();
            entity.Property(e => e.Role).IsRequired();
        });

        // Configure ShiftLeader entity (separate table for TPT inheritance)
        modelBuilder.Entity<ShiftLeader>(entity =>
        {
            entity.ToTable("shift_leaders", schema);
            entity.Property(e => e.Tenant).IsRequired().HasMaxLength(100);
        });

        // Configure StuffBooking entity
        modelBuilder.Entity<StuffBooking>(entity =>
        {
            entity.ToTable("stuff_bookings", schema);
            entity.HasKey(e => e.ID);
            entity.Property(e => e.ID).IsRequired().HasMaxLength(50);
            entity.Property(e => e.WorkerId).IsRequired().HasMaxLength(50);
            entity.Property(e => e.ShiftLeaderId).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Tenant).IsRequired().HasMaxLength(100);
            entity.Property(e => e.PeriodStart).IsRequired();
            entity.Property(e => e.PeriodEnd);
            entity.Property(e => e.ReassignmentScheduledDate);
            entity.Property(e => e.Notes).HasMaxLength(1000);
            entity.Property(e => e.IsActive).IsRequired();

            // Indexes for performance
            entity.HasIndex(e => new { e.WorkerId, e.IsActive });
            entity.HasIndex(e => new { e.ShiftLeaderId, e.IsActive });
            entity.HasIndex(e => e.ReassignmentScheduledDate)
                  .HasFilter("\"ReassignmentScheduledDate\" IS NOT NULL");
        });

        // Configure Tenant entity - always in public schema (master data)
        modelBuilder.Entity<Tenant>(entity =>
        {
            entity.ToTable("tenants", schema);
            entity.HasKey(e => e.CompanyName);
            entity.Property(e => e.CompanyName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.IsActive).IsRequired();
        });
    }
}
