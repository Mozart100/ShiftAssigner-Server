using AutoMapper;
using ShiftAssignerServer.Models.Stuff;
using ShiftAssignerServer.Repositories;
using ShiftAssignerServer.Requests;

namespace ShiftAssignerServer.Services;

public interface ITeamHierarchyService
{
    Task<bool> AssignAsync(TeamHierarchy booking);
    Task<bool> ReassignAsync(ReassignWorkerRequest reassignWorkerRequest);
    Task<GetWorkerPerShiftLeaderResponse?> GetShiftLeaderWithWorkersAsync(string shiftLeaderId);
}

public class TeamHierarchyService : ITeamHierarchyService
{
    private readonly IStuffBookingRepository _stuffBookingRepository;
    private readonly IWorkerRepository _workerRepository;
    private readonly IShiftLeaderRepository _shiftLeaderRepo;
    private readonly IMapper _mapper;

    public TeamHierarchyService(IStuffBookingRepository stuffBookingRepository, IWorkerRepository workerRepo, IShiftLeaderRepository shiftLeaderRepo, IMapper mapper)
    {
        _stuffBookingRepository = stuffBookingRepository;
        _workerRepository = workerRepo;
        _shiftLeaderRepo = shiftLeaderRepo;
        _mapper = mapper;
    }

    public async Task<bool> AssignAsync(TeamHierarchy booking)
    {
        await _stuffBookingRepository.InsertAsync(booking);
        return true;
    }

    public async Task<bool> ReassignAsync(ReassignWorkerRequest reassignRequest)
    {
        if (reassignRequest == null ||
            reassignRequest.WorkerIds == null ||
            !reassignRequest.WorkerIds.Any() ||
            string.IsNullOrWhiteSpace(reassignRequest.ReassignToShiftLeaderId))
        {
            return false;
        }

        try
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);

            foreach (var workerId in reassignRequest.WorkerIds)
            {
                // Find current active assignment for this worker
                var currentAssignment = await _stuffBookingRepository.FirstOrDefaultAsync(x =>
                    x.WorkerId == workerId &&
                    x.IsActive &&
                    x.PeriodEnd == null);

                if (currentAssignment != null)
                {
                    // Deactivate current assignment (set IsActive = false)
                    await _stuffBookingRepository.UpdateAsync(
                        x => x.ID == currentAssignment.ID,
                        booking =>
                        {
                            booking.PeriodEnd = today;
                            booking.IsActive = false;
                            booking.Notes += $" | Deactivated on {today:yyyy-MM-dd} - Reassigned";
                        });
                }

                // Add new record with new shift leader assignment
                var newAssignment = new TeamHierarchy
                {
                    WorkerId = workerId,
                    ShiftLeaderId = reassignRequest.ReassignToShiftLeaderId,
                    PeriodStart = today,
                    PeriodEnd = null, // Open-ended
                    Notes = $"New assignment after reassignment. {reassignRequest.Notes}".Trim(),
                    IsActive = true
                };

                await _stuffBookingRepository.InsertAsync(newAssignment);
            }

            return true;
        }
        catch (Exception)
        {
            // Log exception in real application
            return false;
        }
    }

    public async Task<GetWorkerPerShiftLeaderResponse?> GetShiftLeaderWithWorkersAsync(string shiftLeaderId)
    {
        // Get the shift leader
        var shiftLeader = await _shiftLeaderRepo.FirstOrDefaultAsync(sl => sl.ID == shiftLeaderId);

        // Get current active assignments for this shift leader
        var activeAssignments = await _stuffBookingRepository.GetAllAsync(sb =>
            sb.ShiftLeaderId == shiftLeaderId &&
            sb.IsActive);

        // Get workers for these assignments
        var workerIds = activeAssignments.Select(a => a.WorkerId).ToList();
        var workers = new List<GetWorkerPerShiftLeaderResponse.Worker>();

        foreach (var activeAssignment in activeAssignments)
        {
            var worker = await _workerRepository.FirstOrDefaultAsync(w => w.ID == activeAssignment.WorkerId);
            workers.Add(new GetWorkerPerShiftLeaderResponse.Worker
            {
                ID = worker.ID,
                FirstName = worker.FirstName,
                LastName = worker.LastName
            });
        }

        // Create the response
        return new GetWorkerPerShiftLeaderResponse
        {
            ShiftLeaderID = shiftLeader.ID,
            ShiftLeaderFirstName = shiftLeader.FirstName,
            ShiftLeaderLastName = shiftLeader.LastName,
            Workers = workers
        };
    }
}
