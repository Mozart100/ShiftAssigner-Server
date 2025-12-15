using ShiftAssignerServer.Data;
using ShiftAssignerServer.Models.Stuff;

namespace ShiftAssignerServer.Repositories;

public interface IShiftConfigRepository : IRepositoryBase<ShiftConfig> 
{
    Task<IEnumerable<ShiftConfig>> GetActiveConfigsAsync();
    Task<ShiftConfig?> GetByIdAsync(int id);
}

public sealed class ShiftConfigRepository : BaseRepository<ShiftConfig>, IShiftConfigRepository
{
    public ShiftConfigRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<ShiftConfig>> GetActiveConfigsAsync()
    {
        return await FirstOrDefaultAsync(sc => sc.IsActive) != null 
            ? await GetAllAsync()
            : Enumerable.Empty<ShiftConfig>();
    }

    public async Task<ShiftConfig?> GetByIdAsync(int id)
    {
        return await FirstOrDefaultAsync(sc => sc.Id == id);
    }
}