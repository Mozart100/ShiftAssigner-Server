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
    Task<IEnumerable<PubWorker>> GetAllAsync(string perTenant);
}

public class WorkerService : IWorkerService
{
    private readonly IWorkerRepository _repo;
    private readonly IMapper _mapper;

    public WorkerService(IWorkerRepository repo, IMapper mapper)
    {
        _repo = repo;
        _mapper = mapper;
    }

    public async Task<bool> AddWorker(Worker worker)
    {
        var ptr = await _repo.InsertAsync(worker);
        return true;
    }

    public async Task<IEnumerable<PubWorker>> GetAllAsync(string perTenant)
    {
        // Tenant information has been moved to ShiftAssignment records.
        // Returning all active workers here; filtering by tenant should be done via assignments/leader endpoints.
        var workers = await _repo.GetAllAsync();
        if (workers is null)
        {
            return [];
        }

        // Filter only active workers
        var activeWorkers = workers.Where(w => w.IsActive);

        var dtos = _mapper.Map<IEnumerable<PubWorker>>(activeWorkers);
        return dtos;
    }

    // Interface implemented above - no explicit fallback required.
}
