using ShiftAssignerServer.Models;

namespace ShiftAssignerServer.Models.Stuff
{
    public partial class ShiftPeriodConfig : IActiveEntity
    {
        public int Id { get; set; }

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


    public partial class ShiftPeriodConfig
    {
        public partial class Day
        {
            // public string Name { get; set; } // Sunday,Monday...

            public string DayOfTheWeek => DateOnly.DayOfWeek.ToString();


            public partial class ShiftInfo
            {
            }

        }
    }

}
