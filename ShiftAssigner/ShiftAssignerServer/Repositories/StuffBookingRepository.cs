using ShiftAssignerServer.Data;
using ShiftAssignerServer.Models.Stuff;

namespace ShiftAssignerServer.Repositories;

public interface IStuffBookingRepository : IRepositoryBase<StuffBooking> { }

public class StuffBookingRepository : BaseRepository<StuffBooking>, IStuffBookingRepository
{
    public StuffBookingRepository(ApplicationDbContext context) : base(context) { }
}
