using ShiftAssignerServer.Models.Stuff;

namespace ShiftAssignerServer.Requests;

public interface ITenantRegistrationMapper : IRegistrationMapper
{
    // string CompanyName { get; set; }

}

public class     public async Task<bool> AddBossTenantAsync(TenantRegisterRequest request)
    {
        var ptr = await _bossTenantRepository.InsertAsync(request);
    } : RegisterRequest
{
    public string Tenant { get; set; } = string.Empty;
}


public class TenantRegisterResponse : RegisterResponse
{
    public string Tenant { get; set; }
}

