using ShiftAssignerServer.Models.Stuff;

namespace ShiftAssignerServer.Repositories;

public interface IShiftLeaderRepository : IRepositoryBase<ShiftLeader>
{
    
}


public class ShiftLeaderRepository : RepositoryBase<ShiftLeader> , IShiftLeaderRepository
{
    public ShiftLeaderRepository()
    {
    }
}
