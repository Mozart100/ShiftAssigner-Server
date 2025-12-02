using Microsoft.EntityFrameworkCore;
using ShiftAssignerServer.Models.Stuff;
using ShiftAssignerServer.Models;

namespace ShiftAssignerServer.Data;

/// <summary>
/// A dedicated DbContext for creating tenant-specific schemas and tables.
/// This context has the tenant schema baked in during construction.
/// </summary>
public class TenantCreationDbContext : DbContext
{
    private readonly string _tenantSchema;

    public TenantCreationDbContext(DbContextOptions options, string tenantSchema)
        : base(options)
    {
        _tenantSchema = tenantSchema;
    }

    public DbSet<Worker> Workers { get; set; }
    public DbSet<ShiftLeader> ShiftLeaders { get; set; }
    public DbSet<StuffBooking> StuffBookings { get; set; }
    public DbSet<Tenant> Tenants { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Use the tenant-specific schema for all entities
        var schema = _tenantSchema;

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
            entity.Property(e => e.DateOfBirth).IsRequired();
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

        // Configure Tenant entity
        modelBuilder.Entity<Tenant>(entity =>
        {
            entity.ToTable("tenants", schema);
            entity.HasKey(e => e.CompanyName);
            entity.Property(e => e.CompanyName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.IsActive).IsRequired();
        });
    }
}