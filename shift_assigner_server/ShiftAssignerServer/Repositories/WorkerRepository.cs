using ShiftAssignerServer.Data;
using ShiftAssignerServer.Models.Stuff;

namespace ShiftAssignerServer.Repositories;

// public interface IWorkerRepository : IRepositoryBase<Worker>
// {
// }

// public class WorkerRepository : RepositoryBase<Worker>, IWorkerRepository
// {
//     public WorkerRepository()
//     {
//     }
// }


public interface IWorkerRepository : IRepositoryBase<Worker> { }

public sealed class WorkerRepository : BaseRepository<Worker>, IWorkerRepository
{
    public WorkerRepository(ApplicationDbContext context) : base(context) { }
}