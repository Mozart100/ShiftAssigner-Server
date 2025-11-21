using ShiftAssignerServer.Models.Stuff;

namespace ShiftAssignerServer.Repositories;

public interface IWorkerRepository : IRepositoryBase<Worker>
{
}

public class WorkerRepository : RepositoryBase<Worker>, IWorkerRepository
{
    public WorkerRepository()
    {
    }
}
