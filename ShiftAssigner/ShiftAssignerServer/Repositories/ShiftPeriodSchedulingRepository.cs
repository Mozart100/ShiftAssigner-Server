using ShiftAssignerServer.Data;
using ShiftAssignerServer.Models.Stuff;

namespace ShiftAssignerServer.Repositories;

public interface IShiftPeriodSchedulingRepository : IRepositoryBase<ShiftPeriodScheduling> 
{
}

public sealed class ShiftPeriodSchedulingRepository : BaseRepository<ShiftPeriodScheduling>, IShiftPeriodSchedulingRepository
{
    public ShiftPeriodSchedulingRepository(ApplicationDbContext context) : base(context) { }

}