using ShiftAssignerServer.Data;
using ShiftAssignerServer.Models;

namespace ShiftAssignerServer.Repositories;

public interface IMainSchemaRepository : IRepositoryBase<Schema> { }

public class MainSchemaRepository : BaseRepository<Schema>, IMainSchemaRepository
{
    public MainSchemaRepository(ApplicationDbContext context) : base(context) { }
}


