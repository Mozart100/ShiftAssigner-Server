using ShiftAssignerServer.Models.Stuff;

namespace ShiftAssignerServer.Repositories;

public interface IStuffBookingRepository : IRepositoryBase<StuffBooking>
{
}

public class StuffBookingRepository : RepositoryBase<StuffBooking>, IStuffBookingRepository
{
    public StuffBookingRepository()
    {
    }
}
