using ShiftAssignerServer.Data;
using ShiftAssignerServer.Models;

namespace ShiftAssignerServer.Repositories;

public interface ITenantRepository : IRepositoryBase<Company> { }

public class TenantRepository : BaseRepository<Company>, ITenantRepository
{
    public TenantRepository(ApplicationDbContext context) : base(context) { }
}


