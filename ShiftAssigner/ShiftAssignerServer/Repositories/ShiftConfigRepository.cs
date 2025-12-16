using ShiftAssignerServer.Data;
using ShiftAssignerServer.Models.WorkerScheduling;

namespace ShiftAssignerServer.Repositories;

public interface IShiftConfigRepository : IRepositoryBase<TenantShiftScheduling> 
{
    Task<IEnumerable<TenantShiftScheduling>> GetActiveConfigsAsync();
    Task<TenantShiftScheduling?> GetByIdAsync(int id);
}

public interface ITenantShiftConfigRepository : IRepositoryBase<TenantShiftScheduling> 
{
    Task<IEnumerable<TenantShiftScheduling>> GetActiveConfigsAsync();
    Task<TenantShiftScheduling?> GetByIdAsync(int id);
}

public sealed class TenantShiftConfigRepository : BaseRepository<TenantShiftScheduling>, ITenantShiftConfigRepository, IShiftConfigRepository
{
    public TenantShiftConfigRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<TenantShiftScheduling>> GetActiveConfigsAsync()
    {
        return await FirstOrDefaultAsync(sc => sc.IsActive) != null 
            ? await GetAllAsync()
            : Enumerable.Empty<TenantShiftScheduling>();
    }

    public async Task<TenantShiftScheduling?> GetByIdAsync(int id)
    {
        return await FirstOrDefaultAsync(sc => sc.Id == id);
    }
}