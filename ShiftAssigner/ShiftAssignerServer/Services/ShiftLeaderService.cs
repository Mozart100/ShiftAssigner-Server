using AutoMapper;
using ShiftAssignerServer.Models.Stuff;
using ShiftAssignerServer.Repositories;

namespace ShiftAssignerServer.Services;

public interface IShiftLeaderService
{
    Task<bool> AddTenantAsync(ShiftLeader leader);
    Task<IEnumerable<PubShiftLeader>> GetAllShiftLeaderAsync(string perTenant);
}

public class ShiftLeaderService : IShiftLeaderService
{
    private readonly IShiftLeaderRepository _repo;
    private readonly IMapper _shiftLeaderRepository;

    public ShiftLeaderService(IShiftLeaderRepository repo, IMapper mapper)
    {
        _repo = repo;
        _shiftLeaderRepository = mapper;
    }


    public async Task<bool> AddTenantAsync(ShiftLeader shiftLeader)
    {
        var model = await _repo.InsertAsync(shiftLeader);
        return true;
    }

    public async Task<IEnumerable<PubShiftLeader>> GetAllShiftLeaderAsync(string perTenant)
    {
        var leaders = await _repo.GetAllAsync(x => x.IsActive && x.Tenant.Equals(perTenant, StringComparison.CurrentCultureIgnoreCase)  );

        if (leaders.IsEmpty())
        {
            return [];
        }

        var dtos = _shiftLeaderRepository.Map<IEnumerable<PubShiftLeader>>(leaders);
        return dtos;
    }
}
