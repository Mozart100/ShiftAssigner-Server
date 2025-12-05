using ShiftAssignerServer.Data;

namespace ShiftAssignerServer.Repositories;

public interface ITenantUnitOfWork
{
    IWorkerRepository Workers { get; }
    IShiftLeaderRepository ShiftLeaders { get; }
    IStuffBookingRepository StuffBookings { get; }
    ITenantRepository Tenants { get; }
    IBossTenantRepository BossTenantRepository { get; }

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

    public IWorkerRepository Workers { get; }
    public IShiftLeaderRepository ShiftLeaders { get; }
    public IStuffBookingRepository StuffBookings { get; }
    public ITenantRepository Tenants { get; }
    public IBossTenantRepository BossTenantRepository { get; }

    public TenantUnitOfWork(
        ApplicationDbContext context,
        IWorkerRepository workers,
        IShiftLeaderRepository shiftLeaders,
        IStuffBookingRepository stuffBookings,
        ITenantRepository tenants,
        IBossTenantRepository bossTenantRepository)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        Workers = workers ?? throw new ArgumentNullException(nameof(workers));
        ShiftLeaders = shiftLeaders ?? throw new ArgumentNullException(nameof(shiftLeaders));
        StuffBookings = stuffBookings ?? throw new ArgumentNullException(nameof(stuffBookings));
        Tenants = tenants ?? throw new ArgumentNullException(nameof(tenants));
        BossTenantRepository = bossTenantRepository ?? throw new ArgumentNullException(nameof(bossTenantRepository));
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
        return Workers.HasDataBaseChanged ||
               ShiftLeaders.HasDataBaseChanged ||
               StuffBookings.HasDataBaseChanged ||
               Tenants.HasDataBaseChanged ||
               BossTenantRepository.HasDataBaseChanged;
    }
}
