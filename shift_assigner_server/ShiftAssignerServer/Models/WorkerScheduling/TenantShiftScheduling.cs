using ShiftAssignerServer.Common;

namespace ShiftAssignerServer.Models.WorkerScheduling;

public interface IShiftMapper :IAutoMapperEntities
{
     List<TenantShiftScheduling.ShiftInfo> Shifts { get; set; } 

}

public class TenantShiftScheduling : IActiveEntity
{
    public int Id { get; set; }
    public List<ShiftInfo> Shifts { get; set; } = new List<ShiftInfo>();

    public bool IsActive { get; set; }
    public DateOnly Created { get; set; }


    public class ShiftInfo
    {
        public string ShiftName { get; set; } = string.Empty; //Morning,Night
        public int MinimumAmountOfWorkers { get; set; }
        public int MaximumAmountOfWorkers { get; set; }
    }
}
