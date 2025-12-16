using ShiftAssignerServer.Data;
using ShiftAssignerServer.Models.Stuff;

namespace ShiftAssignerServer.Repositories;

public interface ITeamHierarchyRepository : IRepositoryBase<TeamHierarchy> { }

public sealed class TeamHierarchyRepository : BaseRepository<TeamHierarchy>, ITeamHierarchyRepository
{
    public TeamHierarchyRepository(ApplicationDbContext context) : base(context) { }
}
