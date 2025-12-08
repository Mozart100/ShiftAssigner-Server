using AutoMapper;
using ShiftAssignerServer.Models.Stuff;
using ShiftAssignerServer.Repositories;
using ShiftAssignerServer.Requests;

namespace ShiftAssignerServer.Services;

public interface IStuffBookingService
{
    Task<bool> AssignAsync(StuffBooking booking);
    Task<bool> ReassignAsync(ReassignWorkerRequest reassignWorkerRequest);
}

public class StuffBookingService : IStuffBookingService
{
    private readonly IStuffBookingRepository _stuffBookingRepository;
    private readonly IWorkerRepository _workerRepo;
    private readonly IMapper _mapper;

    public StuffBookingService(IStuffBookingRepository stuffBookingRepository, IWorkerRepository workerRepo, IMapper mapper)
    {
        _stuffBookingRepository = stuffBookingRepository;
        _workerRepo = workerRepo;
        _mapper = mapper;
    }

    public async Task<bool> AssignAsync(StuffBooking booking)
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
                    x.PeriodEnd == null );

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
                var newAssignment = new StuffBooking
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
}
