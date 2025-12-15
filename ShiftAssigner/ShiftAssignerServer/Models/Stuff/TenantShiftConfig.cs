using System;

namespace ShiftAssignerServer.Models.Stuff;

public class TenantShiftConfig
{
    public int Id { get; set; }
    public List<ShiftInfo> Shifts { get; set; } = new List<ShiftInfo>();

    public bool IsActive { get; set; }
    public DateOnly Created { get; set; }


    public class ShiftInfo
    {
        public string ShiftName { get; set; } //Morning,Night
        public int MinimumAmountOfWorkers { get; set; }
        public int MaximumAmountOfWorkers { get; set; }
    }
}
