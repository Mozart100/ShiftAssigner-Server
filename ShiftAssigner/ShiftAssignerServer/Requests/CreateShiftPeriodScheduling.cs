using ShiftAssignerServer.Models.WorkerScheduling;

namespace ShiftAssignerServer.Requests;

public class CreateShiftPeriodSchedulingRequest
{
    /// <summary>
    /// When the scheduling period starts
    /// </summary>
    public DateOnly StartFrom { get; set; }

    /// <summary>
    /// List of days in the scheduling period with their shifts
    /// </summary>
    public List<CreateDaySchedule> NextPeriod { get; set; } = new List<CreateDaySchedule>();

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

        /// <summary>
        /// Number of workers needed for this shift
        /// </summary>
        public int AmountOfWorkers { get; set; }
    }
}

public class CreateShiftPeriodSchedulingResponse
{
    public int Id { get; set; }
    public string ShiftLeaderId { get; set; } = string.Empty;
    public DateOnly StartFrom { get; set; }
    public DateOnly LastDate { get; set; }
    public string Message { get; set; } = string.Empty;
    public bool Success { get; set; }
}