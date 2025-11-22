using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using ShiftAssignerServer.Models.Stuff;
using ShiftAssignerServer.Repositories;
using static ShiftAssignerServer.Models.Stuff.Worker;

namespace ShiftAssignerServer.Services;

public interface IShiftAssignmentService
{
    Task<bool> AssignAsync(ShiftAssignment assignment);
    // Get workers supervised by a leader for a given tenant and period range (periodStart .. optional periodEnd)
    Task<IEnumerable<PubWorker>> GetWorkersForLeaderPeriodAsync(string tenant, string shiftLeaderId, DateOnly periodStart, DateOnly? periodEnd = null);
}

public class ShiftAssignmentService : IShiftAssignmentService
{
    private readonly IShiftAssignmentRepository _repo;
    private readonly IWorkerRepository _workerRepo;
    private readonly IMapper _mapper;

    public ShiftAssignmentService(IShiftAssignmentRepository repo, IWorkerRepository workerRepo, IMapper mapper)
    {
        _repo = repo;
        _workerRepo = workerRepo;
        _mapper = mapper;
    }

    public async Task<bool> AssignAsync(ShiftAssignment assignment)
    {
        await _repo.InsertAsync(assignment);
        return true;
    }

    public async Task<IEnumerable<PubWorker>> GetWorkersForLeaderPeriodAsync(string tenant, string shiftLeaderId, DateOnly periodStart, DateOnly? periodEnd = null)
    {
        // If periodEnd is supplied, treat assignment as matching any assignment whose period intersects the range
        var assignments = await _repo.GetAllAsync(x =>
            x.Tenant.Equals(tenant, StringComparison.InvariantCultureIgnoreCase)
            && x.ShiftLeaderId.Equals(shiftLeaderId, StringComparison.InvariantCultureIgnoreCase)
            && (
                (periodEnd is null && x.PeriodStart.Equals(periodStart)) ||
                (periodEnd is not null &&
                 // assignment intersects [periodStart, periodEnd]
                 ((x.PeriodEnd ?? x.PeriodStart) >= periodStart && x.PeriodStart <= periodEnd.Value))
            ));

        if (assignments is null) return System.Linq.Enumerable.Empty<PubWorker>();

        var workerIds = assignments.Select(a => a.WorkerId).ToArray();

        var workers = new List<Worker>();
        foreach (var id in workerIds)
        {
            var w = _workerRepo.FirstOrDefault(x => x.ID.Equals(id, StringComparison.InvariantCultureIgnoreCase));
            if (w is not null) workers.Add(w);
        }

        var dtos = _mapper.Map<IEnumerable<PubWorker>>(workers);
        return dtos;
    }
}
