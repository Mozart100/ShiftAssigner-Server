using ShiftAssignerServer.Data;
using ShiftAssignerServer.Models.WorkerScheduling;

namespace ShiftAssignerServer.Repositories;

public interface IShiftConfigRepository : IRepositoryBase<TenantShiftConfig> 
{
    Task<IEnumerable<TenantShiftConfig>> GetActiveConfigsAsync();
    Task<TenantShiftConfig?> GetByIdAsync(int id);
}

public interface ITenantShiftConfigRepository : IRepositoryBase<TenantShiftConfig> 
{
    Task<IEnumerable<TenantShiftConfig>> GetActiveConfigsAsync();
    Task<TenantShiftConfig?> GetByIdAsync(int id);
}

public sealed class TenantShiftConfigRepository : BaseRepository<TenantShiftConfig>, ITenantShiftConfigRepository, IShiftConfigRepository
{
    public TenantShiftConfigRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<TenantShiftConfig>> GetActiveConfigsAsync()
    {
        return await FirstOrDefaultAsync(sc => sc.IsActive) != null 
            ? await GetAllAsync()
            : Enumerable.Empty<TenantShiftConfig>();
    }

    public async Task<TenantShiftConfig?> GetByIdAsync(int id)
    {
        return await FirstOrDefaultAsync(sc => sc.Id == id);
    }
}