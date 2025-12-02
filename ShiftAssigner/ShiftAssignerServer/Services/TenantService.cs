using System.Linq;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
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
        // Create a temporary DbContext configured for the tenant schema
        var connectionString = _context.Database.GetConnectionString();
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseNpgsql(connectionString);

        // Create a temporary tenant provider that returns the target schema
        var tempTenantProvider = new TempTenantProvider(schemaName);

        using var tenantContext = new ApplicationDbContext(optionsBuilder.Options, tempTenantProvider);

        // Use EF Core's migration capabilities to create the schema and all tables
        await tenantContext.Database.EnsureCreatedAsync();
        
        // EnsureCreatedAsync will handle creating all tables, relationships, and indexes
        // If it succeeds, all tables are created properly
    }

    // Temporary tenant provider for schema creation
    private class TempTenantProvider : ITenantProvider
    {
        private readonly string _schemaName;

        public TempTenantProvider(string schemaName)
        {
            _schemaName = schemaName;
        }

        public string TenantId => _schemaName;
        public string TenantSchema => _schemaName;
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