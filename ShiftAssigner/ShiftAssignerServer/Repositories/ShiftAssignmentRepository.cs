using ShiftAssignerServer.Models.Stuff;

namespace ShiftAssignerServer.Repositories;

public interface IShiftAssignmentRepository : IRepositoryBase<ShiftAssignment>
{
}

public class ShiftAssignmentRepository : RepositoryBase<ShiftAssignment>, IShiftAssignmentRepository
{
    public ShiftAssignmentRepository()
    {
    }
}
