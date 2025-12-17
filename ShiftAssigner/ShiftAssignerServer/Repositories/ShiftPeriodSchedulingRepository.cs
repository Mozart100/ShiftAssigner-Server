using ShiftAssignerServer.Data;
using ShiftAssignerServer.Models.WorkerScheduling;

namespace ShiftAssignerServer.Repositories;

public interface IShiftPeriodSchedulingRepository : IRepositoryBase<ShiftPeriodScheduling> 
{
}

public sealed class ShiftPeriodSchedulingRepository : BaseRepository<ShiftPeriodScheduling>, IShiftPeriodSchedulingRepository
{
    public ShiftPeriodSchedulingRepository(ApplicationDbContext context) : base(context) { }

}