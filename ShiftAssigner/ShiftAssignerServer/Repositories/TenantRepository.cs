using ShiftAssignerServer.Data;
using ShiftAssignerServer.Models;

namespace ShiftAssignerServer.Repositories;

public interface ITenantRepository : IRepositoryBase<Tenant> { }

public class TenantRepository : BaseRepository<Tenant>, ITenantRepository
{
    public TenantRepository(ApplicationDbContext context) : base(context) { }
}
