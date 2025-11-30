using ShiftAssignerServer.Data;

namespace ShiftAssignerServer.Repositories;

public interface IUnitOfWork
{
    IWorkerRepository Workers { get; }
    IShiftLeaderRepository ShiftLeaders { get; }
    IStuffBookingRepository StuffBookings { get; }
    ITenantRepository Tenants { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    int SaveChanges();
}

public sealed class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;

    public IWorkerRepository Workers { get; }
    public IShiftLeaderRepository ShiftLeaders { get; }
    public IStuffBookingRepository StuffBookings { get; }
    public ITenantRepository Tenants { get; }

    public UnitOfWork(
        ApplicationDbContext context,
        IWorkerRepository workers,
        IShiftLeaderRepository shiftLeaders,
        IStuffBookingRepository stuffBookings,
        ITenantRepository tenants)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        Workers = workers ?? throw new ArgumentNullException(nameof(workers));
        ShiftLeaders = shiftLeaders ?? throw new ArgumentNullException(nameof(shiftLeaders));
        StuffBookings = stuffBookings ?? throw new ArgumentNullException(nameof(stuffBookings));
        Tenants = tenants ?? throw new ArgumentNullException(nameof(tenants));
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => _context.SaveChangesAsync(cancellationToken);

    public int SaveChanges()
        => _context.SaveChanges();
}
