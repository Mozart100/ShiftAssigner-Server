using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using ShiftAssignerServer.Models.Stuff;
using ShiftAssignerServer.Repositories;
using ShiftAssignerServer.Requests;

namespace ShiftAssignerServer.Services;

public interface IShiftLeaderService
{
    Task<bool> AddTenantAsync(ShiftLeader leader);
    Task<IEnumerable<PubShiftLeader>> GetAllAsync(string perTenant);
}

public class ShiftLeaderService : IShiftLeaderService
{
    private readonly IShiftLeaderRepository _repo;
    private readonly IMapper _mapper;

    public ShiftLeaderService(IShiftLeaderRepository repo, IMapper mapper)
    {
        _repo = repo;
        _mapper = mapper;
    }


    public async Task<bool> AddTenantAsync(ShiftLeader shiftLeader)
    {
        var model = await _repo.InsertAsync(shiftLeader);
        return true;
    }

    public async Task<IEnumerable<PubShiftLeader>> GetAllAsync(string perTenant)
    {
        var leaders = await _repo.GetAllAsync(x => x.Tenant.Equals(perTenant, StringComparison.CurrentCultureIgnoreCase) && x.IsActive);

        if (leaders.IsEmpty())
        {
            return [];
        }

        var dtos = _mapper.Map<IEnumerable<PubShiftLeader>>(leaders);
        return dtos;
    }
}
