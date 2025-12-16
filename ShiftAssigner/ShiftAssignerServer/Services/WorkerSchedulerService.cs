using ShiftAssignerServer.Models.Stuff;
using ShiftAssignerServer.Models.WorkerScheduling;
using ShiftAssignerServer.Repositories;

namespace ShiftAssignerServer.Services;

public interface IWorkerSchedulerService
{
    Task<IEnumerable<ShiftPeriodConfig>> GetActiveShiftPeriodsAsync();
    Task<ShiftPeriodConfig?> GetShiftPeriodByIdAsync(int periodId);
    Task<IEnumerable<ShiftPeriodConfig>> GetShiftPeriodsForDateRangeAsync(DateOnly startDate, DateOnly endDate);
    Task<TenantShiftConfig?> GetTenantShiftConfigAsync();
    Task<IEnumerable<TenantShiftConfig>> GetActiveTenantShiftConfigsAsync();
    Task<bool> CreateShiftPeriodAsync(ShiftPeriodConfig shiftPeriod);
    Task<bool> UpdateShiftPeriodAsync(ShiftPeriodConfig shiftPeriod);
    Task<bool> AssignWorkerToShiftAsync(int periodId, string shiftName, DateOnly date, string workerId);
    Task<bool> RemoveWorkerFromShiftAsync(int periodId, string shiftName, DateOnly date, string workerId);
    Task<IEnumerable<string>> GetWorkersForShiftAsync(int periodId, string shiftName, DateOnly date);
}

public class WorkerSchedulerService : IWorkerSchedulerService
{
    private readonly IShiftPeriodConfigRepository _shiftPeriodConfigRepository;
    private readonly IShiftConfigRepository _tenantShiftConfigRepository;

    public WorkerSchedulerService(
        IShiftPeriodConfigRepository shiftPeriodConfigRepository,
        IShiftConfigRepository tenantShiftConfigRepository)
    {
        _shiftPeriodConfigRepository = shiftPeriodConfigRepository ?? throw new ArgumentNullException(nameof(shiftPeriodConfigRepository));
        _tenantShiftConfigRepository = tenantShiftConfigRepository ?? throw new ArgumentNullException(nameof(tenantShiftConfigRepository));
    }

    public async Task<IEnumerable<ShiftPeriodConfig>> GetActiveShiftPeriodsAsync()
    {
        return await _shiftPeriodConfigRepository.GetAllActiveAsync();
    }

    public async Task<ShiftPeriodConfig?> GetShiftPeriodByIdAsync(int periodId)
    {
        return await _shiftPeriodConfigRepository.GetByIdAsync(periodId);
    }

    public async Task<IEnumerable<ShiftPeriodConfig>> GetShiftPeriodsForDateRangeAsync(DateOnly startDate, DateOnly endDate)
    {
        return await _shiftPeriodConfigRepository.GetPeriodsByDateRangeAsync(startDate, endDate);
    }

    public async Task<TenantShiftConfig?> GetTenantShiftConfigAsync()
    {
        var configs = await _tenantShiftConfigRepository.GetActiveConfigsAsync();
        return configs.FirstOrDefault();
    }

    public async Task<IEnumerable<TenantShiftConfig>> GetActiveTenantShiftConfigsAsync()
    {
        return await _tenantShiftConfigRepository.GetActiveConfigsAsync();
    }

    public async Task<bool> CreateShiftPeriodAsync(ShiftPeriodConfig shiftPeriod)
    {
        try
        {
            await _shiftPeriodConfigRepository.InsertAsync(shiftPeriod);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> UpdateShiftPeriodAsync(ShiftPeriodConfig shiftPeriod)
    {
        try
        {
            var success = await _shiftPeriodConfigRepository.UpdateAsync(
                x => x.Id == shiftPeriod.Id,
                entity =>
                {
                    entity.StartFrom = shiftPeriod.StartFrom;
                    entity.Period = shiftPeriod.Period;
                    entity.IsActive = shiftPeriod.IsActive;
                });

            return success;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> AssignWorkerToShiftAsync(int periodId, string shiftName, DateOnly date, string workerId)
    {
        try
        {
            var shiftPeriod = await _shiftPeriodConfigRepository.GetByIdAsync(periodId);
            if (shiftPeriod == null) return false;

            var day = shiftPeriod.Period.FirstOrDefault(d => d.DateOnly == date);
            if (day == null) return false;

            var shift = day.Shifts.FirstOrDefault(s => s.ShiftName.Equals(shiftName, StringComparison.OrdinalIgnoreCase));
            if (shift == null) return false;

            // Check if worker is already assigned
            if (shift.WorkerIds.Contains(workerId)) return true;

            // Check if we've reached the maximum number of workers
            if (shift.WorkerIds.Count >= shift.AmountOfWorkers) return false;

            shift.WorkerIds.Add(workerId);

            return await UpdateShiftPeriodAsync(shiftPeriod);
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> RemoveWorkerFromShiftAsync(int periodId, string shiftName, DateOnly date, string workerId)
    {
        try
        {
            var shiftPeriod = await _shiftPeriodConfigRepository.GetByIdAsync(periodId);
            if (shiftPeriod == null) return false;

            var day = shiftPeriod.Period.FirstOrDefault(d => d.DateOnly == date);
            if (day == null) return false;

            var shift = day.Shifts.FirstOrDefault(s => s.ShiftName.Equals(shiftName, StringComparison.OrdinalIgnoreCase));
            if (shift == null) return false;

            var removed = shift.WorkerIds.Remove(workerId);
            if (!removed) return false;

            return await UpdateShiftPeriodAsync(shiftPeriod);
        }
        catch
        {
            return false;
        }
    }

    public async Task<IEnumerable<string>> GetWorkersForShiftAsync(int periodId, string shiftName, DateOnly date)
    {
        try
        {
            var shiftPeriod = await _shiftPeriodConfigRepository.GetByIdAsync(periodId);
            if (shiftPeriod == null) return Enumerable.Empty<string>();

            var day = shiftPeriod.Period.FirstOrDefault(d => d.DateOnly == date);
            if (day == null) return Enumerable.Empty<string>();

            var shift = day.Shifts.FirstOrDefault(s => s.ShiftName.Equals(shiftName, StringComparison.OrdinalIgnoreCase));
            if (shift == null) return Enumerable.Empty<string>();

            return shift.WorkerIds;
        }
        catch
        {
            return Enumerable.Empty<string>();
        }
    }
}