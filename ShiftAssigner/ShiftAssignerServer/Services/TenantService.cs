using System.Linq;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using ShiftAssignerServer.Data;
using ShiftAssignerServer.Models;
using ShiftAssignerServer.Repositories;
using ShiftAssignerServer.Requests;
public interface ITenantService
{
    Task<bool> AddTenantAsync(string companyName);
    Task<AllTenantsResponse> GetAllTenantsAsync();
}


public class TenantService : ITenantService
{
    private readonly ITenantRepository _tenantRepository;
    private readonly IMapper _mapper;
    private readonly ApplicationDbContext _context;

    public TenantService(ITenantRepository tenantRepository, IMapper mapper, ApplicationDbContext context)
    {
        this._tenantRepository = tenantRepository;
        _mapper = mapper;
        _context = context;
    }

    public async Task<bool> AddTenantAsync(string companyName)
    {
        // Create a dedicated schema for this tenant
        await CreateTenantSchemaAsync(companyName);

        // Create the tenant record
        var tenant = await _tenantRepository.InsertAsync(new Tenant { CompanyName = companyName });
        return true;
    }

    private async Task CreateTenantSchemaAsync(string companyName)
    {
        // Sanitize the company name to create a valid PostgreSQL schema name
        var schemaName = SanitizeSchemaName(companyName);

        // Create the schema using EF Core
        var sql = $"CREATE SCHEMA IF NOT EXISTS \"{schemaName}\"";
        await _context.Database.ExecuteSqlRawAsync(sql);

        // Create tenant-specific tables in the new schema
        await CreateTenantTablesAsync(schemaName);
    }

    private async Task CreateTenantTablesAsync(string schemaName)
    {
        // Create the schema first
        var createSchemaSql = $@"CREATE SCHEMA IF NOT EXISTS ""{schemaName}""";
        await _context.Database.ExecuteSqlRawAsync(createSchemaSql);

        // Create a dedicated DbContext for tenant table creation
        var connectionString = _context.Database.GetConnectionString();
        var optionsBuilder = new DbContextOptionsBuilder()
            .UseNpgsql(connectionString);

        using var tenantCreationContext = new TenantCreationDbContext(optionsBuilder.Options, schemaName);
        
        // Check if tables already exist
        var tablesExist = await CheckIfTablesExist(tenantCreationContext, schemaName);
        
        if (!tablesExist)
        {
            // Force create the database structure for this context
            await tenantCreationContext.Database.EnsureCreatedAsync();
            
            // If EnsureCreatedAsync doesn't work, let's manually create the tables
            await CreateTablesManually(tenantContext, schemaName);
        }
        catch (Exception ex)
        {
            var message = ex.Message;
            // Re-throw to see the actual error
            throw;
        }
    }

    private async Task CreateTablesManually(PureApplicationDbContext context, string schemaName)
    {
        var tableCreationCommands = new[]
        {
            $@"CREATE TABLE IF NOT EXISTS ""{schemaName}"".""workers"" (
                ""ID"" character varying(50) NOT NULL,
                ""FirstName"" character varying(100) NOT NULL,
                ""LastName"" character varying(100) NOT NULL,
                ""PhoneNumber"" character varying(20) NOT NULL,
                ""PasswordHash"" text NOT NULL,
                ""IsActive"" boolean NOT NULL DEFAULT true,
                ""Role"" integer NOT NULL,
                ""DateOfBirth"" date NOT NULL,
                CONSTRAINT ""PK_workers"" PRIMARY KEY (""ID"")
            )",
            
            $@"CREATE TABLE IF NOT EXISTS ""{schemaName}"".""shift_leaders"" (
                ""ID"" character varying(50) NOT NULL,
                ""Tenant"" character varying(100) NOT NULL,
                CONSTRAINT ""PK_shift_leaders"" PRIMARY KEY (""ID""),
                CONSTRAINT ""FK_shift_leaders_workers"" FOREIGN KEY (""ID"") 
                    REFERENCES ""{schemaName}"".""workers"" (""ID"") ON DELETE CASCADE
            )",
            
            $@"CREATE TABLE IF NOT EXISTS ""{schemaName}"".""stuff_bookings"" (
                ""ID"" character varying(50) NOT NULL,
                ""WorkerId"" character varying(50) NOT NULL,
                ""ShiftLeaderId"" character varying(50) NOT NULL,
                ""Tenant"" character varying(100) NOT NULL,
                ""PeriodStart"" date NOT NULL,
                ""PeriodEnd"" date,
                ""ReassignmentScheduledDate"" date,
                ""Notes"" character varying(1000),
                ""IsActive"" boolean NOT NULL DEFAULT true,
                CONSTRAINT ""PK_stuff_bookings"" PRIMARY KEY (""ID"")
            )",
            
            $@"CREATE TABLE IF NOT EXISTS ""{schemaName}"".""tenants"" (
                ""CompanyName"" character varying(100) NOT NULL,
                ""IsActive"" boolean NOT NULL DEFAULT true,
                CONSTRAINT ""PK_tenants"" PRIMARY KEY (""CompanyName"")
            )",
            
            // Create indexes
            $@"CREATE INDEX IF NOT EXISTS ""IX_stuff_bookings_WorkerId_IsActive"" 
                ON ""{schemaName}"".""stuff_bookings"" (""WorkerId"", ""IsActive"")",
            $@"CREATE INDEX IF NOT EXISTS ""IX_stuff_bookings_ShiftLeaderId_IsActive"" 
                ON ""{schemaName}"".""stuff_bookings"" (""ShiftLeaderId"", ""IsActive"")",
            $@"CREATE INDEX IF NOT EXISTS ""IX_stuff_bookings_ReassignmentScheduledDate"" 
                ON ""{schemaName}"".""stuff_bookings"" (""ReassignmentScheduledDate"") 
                WHERE ""ReassignmentScheduledDate"" IS NOT NULL"
        };

        foreach (var command in tableCreationCommands)
        {
            await context.Database.ExecuteSqlRawAsync(command);
        }
    }

    private static string SanitizeSchemaName(string value)
    {
        var cleaned = value
            .ToLowerInvariant()
            .Replace(" ", "_")
            .Replace("-", "_")
            .Replace(".", "_")
            .Where(c => char.IsLetterOrDigit(c) || c == '_')
            .Aggregate("", (current, c) => current + c);

        // PostgreSQL identifiers must not start with a digit
        if (string.IsNullOrEmpty(cleaned) || (!char.IsLetter(cleaned[0]) && cleaned[0] != '_'))
        {
            cleaned = "_" + cleaned;
        }
    }


    public async Task<AllTenantsResponse> GetAllTenantsAsync()
    {
        var result = new AllTenantsResponse();
        var tenants = await _tenantRepository.GetAllAsync();

        if (tenants.IsEmpty())
        {
            return result;
        }

        // Filter only active tenants
        foreach (var tenant in tenants.Where(t => t.IsActive))
        {
            result.Tenants.Add(tenant.CompanyName);
        }

        return result;
    }
}

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