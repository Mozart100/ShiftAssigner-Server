using Microsoft.CodeAnalysis.Host;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ShiftAssignerServer.Models.WorkerScheduling;

public partial class ShiftPeriodScheduling : IActiveEntity
{
    public int Id { get; set; }
    public string ShiftLeaderId { get; set; }

    public DateOnly StartFrom { get; set; }

    public List<Day> Period { get; set; } = new List<Day>();

    public bool IsActive { get; set; }

    public partial class Day
    {
        public DateOnly DateOnly { get; set; }

        public List<ShiftInfo> Shifts { get; set; } = new List<ShiftInfo>();


        public partial class ShiftInfo
        {
            public string ShiftName { get; set; } //Morning,Night
            public int AmountOfWorkers { get; set; }

            // public List<string> AssignedWorkerIds { get; set; } = new List<string>();

            // [NotMapped]
            public List<string> WorkerIds { get; set; } = new List<string>();
        }

    }
}


public partial class ShiftPeriodScheduling
{
    [NotMapped]
    [JsonIgnore]
    public DateOnly LastDay => Period?.Any() == true ? Period.Max(d => d.DateOnly) : StartFrom;
    
    public partial class Day
    {
        // public string Name { get; set; } // Sunday,Monday...

        [NotMapped]
        [JsonIgnore]
        public string DayOfTheWeek => DateOnly.DayOfWeek.ToString();

        public partial class ShiftInfo
        {
            /// <summary>
            /// Total capacity for this shift (same as AmountOfWorkers)
            /// </summary>
            [NotMapped]
            [JsonIgnore]
            public int Capacity => AmountOfWorkers;

            /// <summary>
            /// Number of workers currently assigned to this shift
            /// </summary>
            [NotMapped]
            [JsonIgnore]
            public int AssignedCount => WorkerIds?.Count ?? 0;

            /// <summary>
            /// Number of remaining spots available for this shift
            /// </summary>
            [NotMapped]
            [JsonIgnore]
            public int RemainingSpots => Math.Max(0, AmountOfWorkers - AssignedCount);

            /// <summary>
            /// Whether this shift is fully staffed
            /// </summary>
            [NotMapped]
            [JsonIgnore]
            public bool IsFullyStaffed => !HasAvailableSpots;

            /// <summary>
            /// Whether this shift has available spots
            /// </summary>
            [NotMapped]
            [JsonIgnore]
            public bool HasAvailableSpots => AssignedCount < AmountOfWorkers;
        }
    }
}
