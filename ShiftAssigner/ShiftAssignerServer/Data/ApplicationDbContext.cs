using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using ShiftAssignerServer.Models;
using ShiftAssignerServer.Models.Stuff;
using ShiftAssignerServer.Models.WorkerScheduling;

namespace ShiftAssignerServer.Data;

public sealed class TenantModelCacheKeyFactory : IModelCacheKeyFactory
{
    public object Create(DbContext context, bool designTime)
    {
        return context switch
        {
            ApplicationDbContext app => (context.GetType(), app.TenantSchema, designTime),
            PureApplicationDbContext pure => (context.GetType(), pure.TenantSchema, designTime),
            _ => (context.GetType(), designTime)
        };
    }
}

public class PureApplicationDbContext : DbContext
{
    /// <summary>
    /// Current tenant schema for this DbContext instance.
    /// Used to map tenant-specific tables to the correct PostgreSQL schema.
    /// </summary>

    public PureApplicationDbContext(
        DbContextOptions options)
        : base(options)
    {
    }

    // ---------------------------------------------------------------------------------------------------------------
    // ---------------------------------------------------------------------------------------------------------------
    // ---------------------------------------------------------------------------------------------------------------

    public string TenantSchema { get; set; } = "Anatoliy";

    // ---------------------------------------------------------------------------------------------------------------
    // ---------------------------------------------------------------------------------------------------------------
    // ---------------------------------------------------------------------------------------------------------------
    // DbSets for all entities
    public DbSet<Worker> Workers { get; set; } = null!;
    public DbSet<ShiftLeader> ShiftLeaders { get; set; } = null!;
    public DbSet<BossTenant> BossTenants { get; set; } = null!;
    public DbSet<TeamHierarchy> StuffBookings { get; set; } = null!;
    public DbSet<TenantShiftConfig> ShiftConfigs { get; set; } = null!;
    public DbSet<ShiftPeriodConfig> ShiftPeriodConfigs { get; set; } = null!;

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
            entity.Property(e => e.IsPasswordRequired).IsRequired();
        });

        // Configure ShiftLeader entity (separate table for TPT inheritance)
        modelBuilder.Entity<ShiftLeader>(entity =>
        {
            entity.ToTable("shift_leaders", schema);
            entity.HasKey(e => e.ID);
            entity.Property(e => e.ID).IsRequired().HasMaxLength(50);
            entity.Property(e => e.FirstName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.LastName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.PhoneNumber).IsRequired().HasMaxLength(20);
            entity.Property(e => e.DateOfBirth).IsRequired();
            entity.Property(e => e.PasswordHash).IsRequired();
            entity.Property(e => e.IsActive).IsRequired();
            entity.Property(e => e.Role).IsRequired();
            entity.Property(e => e.IsPasswordRequired).IsRequired();
        });

        // Configure BossTenant entity (complete table with all properties)
        modelBuilder.Entity<BossTenant>(entity =>
        {
            entity.ToTable("boss_tenants", schema);
            entity.HasKey(e => e.ID);
            
            // All PersonBase properties
            entity.Property(e => e.ID).IsRequired().HasMaxLength(50);
            entity.Property(e => e.FirstName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.LastName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.PhoneNumber).IsRequired().HasMaxLength(20);
            entity.Property(e => e.DateOfBirth).IsRequired();
            entity.Property(e => e.Role).IsRequired();
            entity.Property(e => e.PasswordHash).IsRequired();
            entity.Property(e => e.IsActive).IsRequired();
            
            // BossTenant specific property
            entity.Property(e => e.Tenant).IsRequired().HasMaxLength(100);
        });

        // Configure StuffBooking entity
        modelBuilder.Entity<TeamHierarchy>(entity =>
        {
            entity.ToTable("team_hierarchy", schema);
            entity.HasKey(e => e.ID);
            entity.Property(e => e.ID).IsRequired().HasMaxLength(50);
            entity.Property(e => e.WorkerId).IsRequired().HasMaxLength(50);
            entity.Property(e => e.ShiftLeaderId).IsRequired().HasMaxLength(50);
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

        // Configure ShiftConfig entity
        modelBuilder.Entity<TenantShiftConfig>(entity =>
        {
            entity.ToTable("TenantShiftConfig", schema);
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).IsRequired();
            
            // Store the Shifts list as JSONB for flexibility and performance
            entity.Property(e => e.Shifts)
                  .HasColumnType("jsonb")
                  .HasConversion(
                      v => System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions?)null),
                      v => System.Text.Json.JsonSerializer.Deserialize<List<TenantShiftConfig.ShiftInfo>>(v, (System.Text.Json.JsonSerializerOptions?)null) ?? new List<TenantShiftConfig.ShiftInfo>()
                  );
                  
            // Index on JSONB for query performance
            entity.HasIndex(e => e.Shifts).HasMethod("gin");
        });

        // Configure ShiftPeriodConfig entity
        modelBuilder.Entity<ShiftPeriodConfig>(entity =>
        {
            entity.ToTable("shift_period_configs", schema);
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).IsRequired();
            entity.Property(e => e.StartFrom).IsRequired();
            entity.Property(e => e.IsActive).IsRequired();
            
            // Store the Period list as JSONB
            entity.Property(e => e.Period)
                  .HasColumnType("jsonb")
                  .HasConversion(
                      v => System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions?)null),
                      v => System.Text.Json.JsonSerializer.Deserialize<List<ShiftPeriodConfig.Day>>(v, (System.Text.Json.JsonSerializerOptions?)null) ?? new List<ShiftPeriodConfig.Day>()
                  );
                  
            // Index on JSONB for query performance
            entity.HasIndex(e => e.Period).HasMethod("gin");
        });
    }
}


public class ApplicationDbContext : PureApplicationDbContext
{
    private readonly ITenantProvider _tenantProvider;


    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options,
        ITenantProvider tenantProvider)
        : base(options)
    {
        _tenantProvider = tenantProvider;
        TenantSchema = tenantProvider.TenantSchema; // already sanitized
    }

}
