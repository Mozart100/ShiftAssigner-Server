using ShiftAssignerServer.Data;
using ShiftAssignerServer.Models.WorkerScheduling;

namespace ShiftAssignerServer.Repositories;

// public interface IShiftSchedulingRepository : IRepositoryBase<TenantShiftScheduling> 
// {
//     Task<IEnumerable<TenantShiftScheduling>> GetActiveConfigsAsync();
//     Task<TenantShiftScheduling?> GetByIdAsync(int id);
// }

public interface ITenantShiftSchedulingRepository : IRepositoryBase<TenantShiftScheduling> 
{
}

public sealed class TenantShiftSchedulingRepository : BaseRepository<TenantShiftScheduling>, ITenantShiftSchedulingRepository
{
    public TenantShiftSchedulingRepository(ApplicationDbContext context) : base(context)
    {
    }

}