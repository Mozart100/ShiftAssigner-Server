using static ShiftAssignerServer.Models.Stuff.Worker;

namespace ShiftAssignerServer.Requests;



public class GetWorkerPerShiftLeaderResponse
{
    public class Person
    {
        public string ID { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
    }

    public class Worker : Person
    {
    }

    public class ShiftLeader : Person
    {
        public List<Worker> Workers { get; set; }
    }

}
