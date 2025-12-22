using ShiftAssignerServer.Data;
using ShiftAssignerServer.Models.Stuff;

namespace ShiftAssignerServer.Repositories;

public interface IBossTenantRepository : IRepositoryBase<BossTenant> { }
public class BossTenantRepository : BaseRepository<BossTenant>, IBossTenantRepository
{
    public BossTenantRepository(ApplicationDbContext context) : base(context) { }
}


