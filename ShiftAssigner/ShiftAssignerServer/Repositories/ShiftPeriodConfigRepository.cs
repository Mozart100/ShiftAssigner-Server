using ShiftAssignerServer.Data;
using ShiftAssignerServer.Models.Stuff;

namespace ShiftAssignerServer.Repositories;

public interface IShiftPeriodConfigRepository : IRepositoryBase<ShiftPeriodScheduling> 
{
    Task<IEnumerable<ShiftPeriodScheduling>> GetActivePeriodsAsync();
    Task<ShiftPeriodScheduling?> GetByIdAsync(int id);
    Task<IEnumerable<ShiftPeriodScheduling>> GetPeriodsByDateRangeAsync(DateOnly startDate, DateOnly endDate);
}

public sealed class ShiftPeriodConfigRepository : BaseRepository<ShiftPeriodScheduling>, IShiftPeriodConfigRepository
{
    public ShiftPeriodConfigRepository(ApplicationDbContext context) : base(context) { }

    public async Task<IEnumerable<ShiftPeriodScheduling>> GetActivePeriodsAsync()
    {
        return await GetAllAsync(p => p.IsActive);
    }

    public async Task<ShiftPeriodScheduling?> GetByIdAsync(int id)
    {
        return await FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<IEnumerable<ShiftPeriodScheduling>> GetPeriodsByDateRangeAsync(DateOnly startDate, DateOnly endDate)
    {
        return await GetAllAsync(p => p.StartFrom >= startDate && p.StartFrom <= endDate);
    }
}