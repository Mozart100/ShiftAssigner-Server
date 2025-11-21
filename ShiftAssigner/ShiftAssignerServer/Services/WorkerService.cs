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
    Task<IEnumerable<Worker>> GetAllAsync(string perTenant);
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
        var workers = await _repo.GetAllAsync(x => x.Tenant.Equals(perTenant, StringComparison.CurrentCultureIgnoreCase));
        if (workers is null) 
        {
            return [];
        }


        var dtos = _mapper.Map<IEnumerable<PubWorker>>(workers);
        return dtos;
    }

    Task<IEnumerable<Worker>> IWorkerService.GetAllAsync(string perTenant)
    {
        throw new NotImplementedException();
    }
}
