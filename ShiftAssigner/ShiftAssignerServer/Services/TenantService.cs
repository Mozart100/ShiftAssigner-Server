using ShiftAssignerServer.Models;
using ShiftAssignerServer.Repositories;
public interface ITenantService
{
    Task<bool> AddTenantAsync(string companyName);
}


public class TenantService : ITenantService
{
    private readonly ITenantRepository _tenantRepository;

    public TenantService(ITenantRepository tenantRepository)
    {
        this._tenantRepository = tenantRepository;
    }

    public async Task<bool> AddTenantAsync(string companyName)
    {
        var tenant = await _tenantRepository.InsertAsync(new Tenant { CompanyName = companyName });
        return true;
    }
}