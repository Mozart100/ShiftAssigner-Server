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
        // var sql = $"CREATE SCHEMA IF NOT EXISTS \"{schemaName}\"";
        // await _context.Database.ExecuteSqlRawAsync(sql);

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
            
            // If EnsureCreatedAsync doesn't work, we can try to get the create script
            // and execute it manually
            var canConnect = await tenantCreationContext.Database.CanConnectAsync();
            if (canConnect)
            {
                // The context is working, so let's try a different approach
                var script = tenantCreationContext.Database.GenerateCreateScript();
                if (!string.IsNullOrEmpty(script))
                {
                    await tenantCreationContext.Database.ExecuteSqlRawAsync(script);
                }
            }
        }
    }

    private async Task<bool> CheckIfTablesExist(TenantCreationDbContext context, string schemaName)
    {
        try
        {
            var tableCount = await context.Database.SqlQueryRaw<int>(
                $"SELECT COUNT(*) FROM information_schema.tables WHERE table_schema = '{schemaName}'"
            ).FirstOrDefaultAsync();
            
            return tableCount > 0;
        }
        catch
        {
            return false;
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

        return cleaned;
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