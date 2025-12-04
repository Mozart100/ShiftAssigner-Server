using System.Linq;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using ShiftAssignerServer.Data;
using ShiftAssignerServer.Models;
using ShiftAssignerServer.Models.Stuff;
using ShiftAssignerServer.Repositories;
using ShiftAssignerServer.Requests;
public interface ITenantService
{
    Task<bool> AddBossTenantAsync(TenantRegisterRequest request);
    Task<bool> CreateIfNoxExistedTenantSchemaAsync(string companyName);
    Task<AllTenantsResponse> GetAllTenantsAsync();
}


public class TenantService : ITenantService
{
    private readonly IBossTenantRepository _bossTenantRepository;
    private readonly IMapper _mapper;
    private readonly ApplicationDbContext _context;

    public TenantService(IBossTenantRepository bossTenantRepository, IMapper mapper, ApplicationDbContext context)
    {
        this._bossTenantRepository = bossTenantRepository;
        _mapper = mapper;
        _context = context;
    }

    public async Task<bool> CreateIfNoxExistedTenantSchemaAsync(string companyName)
    {
        // Create a dedicated schema for this tenant
        await CreateTenantSchemaAsync(companyName);

        // Create the tenant record
        return true;
    }

    private async Task CreateTenantSchemaAsync(string companyName)
    {
        // Use TenantProvider's SanitizeSchemaName method
        var schemaName = TenantProvider.SanitizeSchemaName(companyName);

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
        
        // Check if database exists and force table creation
        var canConnect = await tenantCreationContext.Database.CanConnectAsync();
        if (canConnect)
        {
            // Generate and execute the creation script
            var script = tenantCreationContext.Database.GenerateCreateScript();
            if (!string.IsNullOrEmpty(script))
            {
                // Replace any default schema references with our tenant schema
                script = script.Replace("CREATE TABLE ", $"CREATE TABLE IF NOT EXISTS ");
                await tenantCreationContext.Database.ExecuteSqlRawAsync(script);
            }
            else
            {
                // Fallback: try EnsureCreated
                await tenantCreationContext.Database.EnsureCreatedAsync();
            }
        }
    }

    public async Task<AllTenantsResponse> GetAllTenantsAsync()
    {
        var result = new AllTenantsResponse();
        var tenants = await _bossTenantRepository.GetAllAsync();

        if (tenants.IsEmpty())
        {
            return result;
        }

        // Filter only active tenants
        // foreach (var tenant in tenants.Where(t => t.IsActive))
        // {
        //     result.Tenants.Add(tenant.CompanyName);
        // }

        return null;
    }

    public async Task<bool> AddBossTenantAsync(TenantRegisterRequest request)
    {
        var bossTenant = _mapper.Map<BossTenant>(request);
        var ptr = await _bossTenantRepository.InsertAsync(bossTenant);

        return true;
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