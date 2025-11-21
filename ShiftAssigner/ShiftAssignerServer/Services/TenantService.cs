using AutoMapper;
using ShiftAssignerServer.Models;
using ShiftAssignerServer.Repositories;
using ShiftAssignerServer.Requests;
public interface ITenantService
{
    Task<bool> AddTenantAsync(string companyName);
    Task<TenantResponse> GetAllTenantsAsync();
}


public class TenantService : ITenantService
{
    private readonly ITenantRepository _tenantRepository;
    private readonly IMapper _mapper;

    public TenantService(ITenantRepository tenantRepository, IMapper mapper)
    {
        this._tenantRepository = tenantRepository;
        _mapper = mapper;
    }

    public async Task<bool> AddTenantAsync(string companyName)
    {
        var tenant = await _tenantRepository.InsertAsync(new Tenant { CompanyName = companyName });
        return true;
    }

    public async Task<TenantResponse> GetAllTenantsAsync()
    {
        var result = new TenantResponse();
        var tenants = await _tenantRepository.GetAllAsync();

        if(tenants.IsEmpty())
        {
            return result;
        }

        foreach (var tenant in tenants)
        {
            result.Tenants.Add(tenant.CompanyName);
        }

        return result;
    }
}