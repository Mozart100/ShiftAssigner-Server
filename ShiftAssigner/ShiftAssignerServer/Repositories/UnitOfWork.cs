using ShiftAssignerServer.Data;

namespace ShiftAssignerServer.Repositories;

public interface ITenantUnitOfWork
{
    IWorkerRepository WorkerRepository { get; }
    IShiftLeaderRepository ShiftLeaderRepository { get; }
    IMainSchemaRepository Tenants { get; }
    IBossTenantRepository BossTenantRepository { get; }
    ITenantShiftSchedulingRepository TenantShiftSchedulingRepository { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    int SaveChanges();
    
    /// <summary>
    /// Checks if any repository has pending changes and automatically saves if needed
    /// </summary>
    Task<bool> AutoSaveIfChangesAsync(CancellationToken cancellationToken = default);
    bool AutoSaveIfChanges();
}

public sealed class TenantUnitOfWork : ITenantUnitOfWork
{
    private readonly ApplicationDbContext _context;

    public IWorkerRepository WorkerRepository { get; }
    public IShiftLeaderRepository ShiftLeaderRepository { get; }
    public IMainSchemaRepository Tenants { get; }
    public IBossTenantRepository BossTenantRepository { get; }
    public ITeamHierarchyRepository TeamHierarchyRepository { get; }
    public ITenantShiftSchedulingRepository TenantShiftSchedulingRepository { get; }
    public IShiftPeriodSchedulingRepository ShiftPeriodSchedulingRepository { get; }

    public TenantUnitOfWork(
        ApplicationDbContext context,
        IWorkerRepository workerRepository,
        IShiftLeaderRepository shiftLeaderRepository,
        IMainSchemaRepository tenants,
        IBossTenantRepository bossTenantRepository,
        ITeamHierarchyRepository teamHierarchyRepository,
        ITenantShiftSchedulingRepository tenantShiftSchedulingRepository,
        IShiftPeriodSchedulingRepository shiftPeriodSchedulingRepository)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        WorkerRepository = workerRepository ?? throw new ArgumentNullException(nameof(workerRepository));
        ShiftLeaderRepository = shiftLeaderRepository ?? throw new ArgumentNullException(nameof(shiftLeaderRepository));
        Tenants = tenants ?? throw new ArgumentNullException(nameof(tenants));
        BossTenantRepository = bossTenantRepository ?? throw new ArgumentNullException(nameof(bossTenantRepository));
        TeamHierarchyRepository = teamHierarchyRepository;
        TenantShiftSchedulingRepository = tenantShiftSchedulingRepository ?? throw new ArgumentNullException(nameof(tenantShiftSchedulingRepository));
        ShiftPeriodSchedulingRepository = shiftPeriodSchedulingRepository ?? throw new ArgumentNullException(nameof(shiftPeriodSchedulingRepository));
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => _context.SaveChangesAsync(cancellationToken);

    public int SaveChanges()
        => _context.SaveChanges();

    /// <summary>
    /// Detects if any repository has changes and automatically saves them
    /// </summary>
    public async Task<bool> AutoSaveIfChangesAsync(CancellationToken cancellationToken = default)
    {
        if (HasAnyChanges())
        {
            await SaveChangesAsync(cancellationToken);
            return true;
        }
        return false;
    }

    /// <summary>
    /// Detects if any repository has changes and automatically saves them
    /// </summary>
    public bool AutoSaveIfChanges()
    {
        if (HasAnyChanges())
        {
            SaveChanges();
            return true;
        }
        return false;
    }

    /// <summary>
    /// Checks if any repository has pending database changes
    /// </summary>
    private bool HasAnyChanges()
    {
        return WorkerRepository.HasDataBaseChanged ||
               ShiftLeaderRepository.HasDataBaseChanged ||
               Tenants.HasDataBaseChanged ||
               BossTenantRepository.HasDataBaseChanged ||
               TeamHierarchyRepository.HasDataBaseChanged ||
               TenantShiftSchedulingRepository.HasDataBaseChanged ||
               ShiftPeriodSchedulingRepository.HasDataBaseChanged;
    }
}
