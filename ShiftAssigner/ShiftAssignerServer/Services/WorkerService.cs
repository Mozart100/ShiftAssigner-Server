using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using ShiftAssignerServer.Models.Stuff;
using ShiftAssignerServer.Repositories;
using static ShiftAssignerServer.Models.Stuff.Worker;

namespace ShiftAssignerServer.Services;

public interface IWorkerService
{
    Task<bool> AddWorker(Worker worker);
    Task<IEnumerable<PubWorker>> GetAllActiveWorkersPerShiftLeaderAsync(string perShiftLeader);
}

public class WorkerService : IWorkerService
{
    private readonly IWorkerRepository _workerRepository;
    private readonly IMapper _mapper;

    public WorkerService(IWorkerRepository repo, IMapper mapper)
    {
        _workerRepository = repo;
        _mapper = mapper;
    }

    public async Task<bool> AddWorker(Worker worker)
    {
        var ptr = await _workerRepository.InsertAsync(worker);
        return true;
    }

    public async Task<IEnumerable<PubWorker>> GetAllActiveWorkersPerShiftLeaderAsync(string perShiftLeader)
    {
        var workers = await _workerRepository.GetAllAsync(x => x.IsActive);
        if (workers is null)
        {
            return [];
        }

        var dtos = _mapper.Map<IEnumerable<PubWorker>>(workers);
        return dtos;
    }

    // Interface implemented above - no explicit fallback required.
}
