namespace ShiftAssignerServer.Requests;

public class WorkerShiftPeriodSchedulingResponse
{
    public DateOnly StartFrom { get; set; }

    /// <summary>
    /// List of days in the scheduling period with their shifts
    /// </summary>
    public List<CreateDaySchedule> Period { get; set; } = new List<CreateDaySchedule>();
    
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;

    public class CreateDaySchedule
    {
        /// <summary>
        /// The specific date for this day
        /// </summary>
        public DateOnly Date { get; set; }

        /// <summary>
        /// List of shifts for this day
        /// </summary>
        public List<CreateShiftInfo> Shifts { get; set; } = new();
    }

    public class CreateShiftInfo
    {
        /// <summary>
        /// Name of the shift (e.g., "Morning", "Evening", "Night")
        /// </summary>
        public string ShiftName { get; set; } = string.Empty;
    }
}