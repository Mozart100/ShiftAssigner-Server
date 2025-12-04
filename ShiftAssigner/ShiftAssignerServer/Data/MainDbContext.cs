using Microsoft.EntityFrameworkCore;
using ShiftAssignerServer.Models;

namespace ShiftAssignerServer.Data;

/// <summary>
/// Main database context for global/master data that exists in the default schema.
/// This context handles cross-tenant data like company registry, system configuration, etc.
/// The schema is automatically created when the application starts.
/// </summary>
public class MainDbContext : DbContext
{
    public MainDbContext(DbContextOptions<MainDbContext> options) : base(options)
    {
    }

    // Master/Global data - exists in default schema
    public DbSet<Company> Companies { get; set; } = null!;

    /// <summary>
    /// Ensures the main database schema is created when the application starts.
    /// This should be called during application startup.
    /// </summary>
    public void EnsureMainSchemaCreated()
    {
        try
        {
            // Create the database and schema if they don't exist
            Database.EnsureCreated();
        }
        catch (Exception ex)
        {
            // Log the exception (you might want to use ILogger here)
            throw new InvalidOperationException("Failed to create main database schema", ex);
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure Company entity - in default/public schema (master data)
        modelBuilder.Entity<Company>(entity =>
        {
            entity.ToTable("companies"); // No schema specified = default/public schema
            entity.HasKey(e => e.CompanyName);
            entity.Property(e => e.CompanyName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.IsActive).IsRequired();
        });
    }
}