using System.Collections.Generic;

namespace ShiftAssignerServer.Requests;



public class GetWorkerPerShiftLeaderResponse
{
    public class Worker
    {
        public string ID { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
    }

    public string ShiftLeaderID { get; set; }
    public string ShiftLeaderFirstName { get; set; } = string.Empty;
    public string ShiftLeaderLastName { get; set; } = string.Empty;

    public List<Worker> Workers { get; set; }

}
