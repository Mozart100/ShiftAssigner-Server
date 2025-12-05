using AutoMapper;
using ShiftAssignerServer.Models.Stuff;
using ShiftAssignerServer.Repositories;
using ShiftAssignerServer.Requests;

namespace ShiftAssignerServer.Services;

public interface IShiftLeaderService
{
    Task<bool> AddShiftLeaderAsync(ShiftLeader leader);
    Task<IEnumerable<PubShiftLeader>> GetAllShiftLeaderAsync(string perTenant);
    Task<bool> LoginAsync(LoginShiftLeaderRequest request);
}

public class ShiftLeaderService : IShiftLeaderService
{
    private readonly ITenantUnitOfWork _tenantUnitOfWork;
    private readonly IMapper _shiftLeaderRepository;

    public ShiftLeaderService(ITenantUnitOfWork tenantUnitOfWork, IMapper mapper)
    {
        _tenantUnitOfWork = tenantUnitOfWork;
        _shiftLeaderRepository = mapper;
    }


    public async Task<bool> AddShiftLeaderAsync(ShiftLeader shiftLeader)
    {
        var model = await _tenantUnitOfWork.ShiftLeaders.InsertAsync(shiftLeader);
        return true;
    }

    public async Task<IEnumerable<PubShiftLeader>> GetAllShiftLeaderAsync(string perTenant)
    {
        var leaders = await _tenantUnitOfWork.ShiftLeaders.GetAllAsync(x => x.IsActive);

        if (leaders.IsEmpty())
        {
            return [];
        }

        var dtos = _shiftLeaderRepository.Map<IEnumerable<PubShiftLeader>>(leaders);
        return dtos;
    }

    public async Task<bool> LoginAsync(LoginShiftLeaderRequest request)
    {
        var updateResult = await _tenantUnitOfWork.ShiftLeaders.UpdateAsync(x => x.ID == request.ID, y =>
        {
            y.PasswordHash = request.Password;
            y.IsPasswordRequired = false;
        });

        return updateResult; // Return actual result instead of always true
    }
}
