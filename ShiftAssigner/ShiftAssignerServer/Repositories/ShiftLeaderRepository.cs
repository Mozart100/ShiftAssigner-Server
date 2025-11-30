using ShiftAssignerServer.Data;
using ShiftAssignerServer.Models.Stuff;

namespace ShiftAssignerServer.Repositories;

public interface IShiftLeaderRepository : IRepositoryBase<ShiftLeader> { }

public class ShiftLeaderRepository : BaseRepository<ShiftLeader>, IShiftLeaderRepository
{
    public ShiftLeaderRepository(ApplicationDbContext context) : base(context) { }
}
