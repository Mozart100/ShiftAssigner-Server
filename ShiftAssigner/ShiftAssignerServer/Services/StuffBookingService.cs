using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using ShiftAssignerServer.Models.Stuff;
using ShiftAssignerServer.Repositories;
using static ShiftAssignerServer.Models.Stuff.Worker;

namespace ShiftAssignerServer.Services;

public interface IStuffBookingService
{
    Task<bool> AssignAsync(StuffBooking booking);
    Task<bool> ReassignAsync(string tenant, string workerId, string newShiftLeaderId, DateOnly periodStart, DateOnly? periodEnd = null, string? notes = null);
    // Get workers supervised by a leader for a given tenant and period range (periodStart .. optional periodEnd)
    Task<IEnumerable<PubWorker>> GetWorkersForLeaderPeriodAsync(string tenant, string shiftLeaderId, DateOnly periodStart, DateOnly? periodEnd = null);
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

    public async Task<bool> ReassignAsync(string tenant, string workerId, string newShiftLeaderId, DateOnly periodStart, DateOnly? periodEnd = null, string? notes = null)
    {
        // Soft delete any existing bookings for this worker in the same tenant that overlap the specified period
        // Set EndPeriod to the day before the new assignment starts
        await _stuffBookingRepository.UpdateAsync(x =>
            x.IsActive
            && x.Tenant.Equals(tenant, StringComparison.InvariantCultureIgnoreCase)
            && x.WorkerId.Equals(workerId, StringComparison.InvariantCultureIgnoreCase)
            && (
                (periodEnd is null && x.PeriodStart.Equals(periodStart)) ||
                (periodEnd is not null && ((x.PeriodEnd ?? x.PeriodStart) >= periodStart && x.PeriodStart <= periodEnd.Value))
            ),
            booking =>
            {
                booking.IsActive = false;
                booking.PeriodEnd = DateOnly.FromDateTime(DateTime.UtcNow);
            });

        var newBooking = new StuffBooking
        {
            WorkerId = workerId,
            ShiftLeaderId = newShiftLeaderId,
            Tenant = tenant,
            PeriodStart = periodStart,
            PeriodEnd = periodEnd,
            Notes = notes ?? string.Empty,
            IsActive = true
        };

        await _stuffBookingRepository.InsertAsync(newBooking);
        return true;
    }

    public async Task<IEnumerable<PubWorker>> GetWorkersForLeaderPeriodAsync(string tenant, string shiftLeaderId, DateOnly periodStart, DateOnly? periodEnd = null)
    {
        var assignments = await _stuffBookingRepository.GetAllAsync(x =>
            x.IsActive
            && x.Tenant.Equals(tenant, StringComparison.InvariantCultureIgnoreCase)
            && x.ShiftLeaderId.Equals(shiftLeaderId, StringComparison.InvariantCultureIgnoreCase)
            && (
                (periodEnd is null && x.PeriodStart.Equals(periodStart)) ||
                (periodEnd is not null &&
                 ((x.PeriodEnd ?? x.PeriodStart) >= periodStart && x.PeriodStart <= periodEnd.Value))
            ));

        if (assignments is null) return System.Linq.Enumerable.Empty<PubWorker>();

        var workerIds = assignments.Select(a => a.WorkerId).ToArray();

        var workers = new List<Worker>();
        foreach (var id in workerIds)
        {
            var w = _workerRepo.FirstOrDefault(x => x.ID.Equals(id, StringComparison.InvariantCultureIgnoreCase) && x.IsActive);
            if (w is not null) workers.Add(w);
        }

        var dtos = _mapper.Map<IEnumerable<PubWorker>>(workers);
        return dtos;
    }
}
