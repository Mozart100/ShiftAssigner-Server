using ShiftAssignerServer.Data;
using ShiftAssignerServer.Models.Stuff;

namespace ShiftAssignerServer.Repositories;

public interface IShiftPeriodConfigRepository : IRepositoryBase<ShiftPeriodConfig> 
{
    Task<ShiftPeriodConfig?> GetByIdAsync(int id);
    Task<IEnumerable<ShiftPeriodConfig>> GetPeriodsByDateRangeAsync(DateOnly startDate, DateOnly endDate);
}

public sealed class ShiftPeriodConfigRepository : BaseRepository<ShiftPeriodConfig>, IShiftPeriodConfigRepository
{
    public ShiftPeriodConfigRepository(ApplicationDbContext context) : base(context) { }


    public async Task<ShiftPeriodConfig?> GetByIdAsync(int id)
    {
        return await FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<IEnumerable<ShiftPeriodConfig>> GetPeriodsByDateRangeAsync(DateOnly startDate, DateOnly endDate)
    {
        return await GetAllAsync(p => p.StartFrom >= startDate && p.StartFrom <= endDate);
    }
}