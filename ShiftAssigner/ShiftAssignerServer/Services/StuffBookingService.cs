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
}
