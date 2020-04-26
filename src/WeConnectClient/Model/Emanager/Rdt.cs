using System.Collections.Generic;

namespace PhilipDaubmeier.WeConnectClient.Model.Emanager
{
    public class Rdt
    {
        public RdtStatus? Status { get; set; }
        public RdtSettings? Settings { get; set; }
        public bool AuxHeatingAllowed { get; set; }
        public bool AuxHeatingEnabled { get; set; }
    }

    public class RdtStatus
    {
        public List<Timer> Timers { get; set; } = new List<Timer>();
        public List<Profile> Profiles { get; set; } = new List<Profile>();
    }

    public class RdtSettings
    {
        public int MinChargeLimit { get; set; }
        public int LowerLimitMax { get; set; }
    }

    public class Timer
    {
        public int TimerId { get; set; }
        public int TimerProfileId { get; set; }
        public string TimerStatus { get; set; } = string.Empty;
        public string TimerChargeScheduleStatus { get; set; } = string.Empty;
        public string TimerClimateScheduleStatus { get; set; } = string.Empty;
        public string TimerExpStatusTimestamp { get; set; } = string.Empty;
        public string TimerProgrammedStatus { get; set; } = string.Empty;
        public Schedule Schedule { get; set; } = new Schedule();
        public string StartDateActive { get; set; } = string.Empty;
        public string TimeRangeActive { get; set; } = string.Empty;
    }

    public class Schedule
    {
        public int Type { get; set; }
        public TimeOfDay Start { get; set; } = new TimeOfDay();
        public TimeOfDay End { get; set; } = new TimeOfDay();
        public int? Index { get; set; }
        public List<string> Daypicker { get; set; } = new List<string>();
        public string StartDateActive { get; set; } = string.Empty;
        public string EndDateActive { get; set; } = string.Empty;
    }

    public class TimeOfDay
    {
        public int? Hours { get; set; }
        public int? Minutes { get; set; }
    }

    public class Profile
    {
        public int ProfileId { get; set; }
        public string ProfileName { get; set; } = string.Empty;
        public string TimeStamp { get; set; } = string.Empty;
        public bool Charging { get; set; }
        public bool Climatisation { get; set; }
        public int TargetChargeLevel { get; set; }
        public bool NightRateActive { get; set; }
        public string NightRateTimeStart { get; set; } = string.Empty;
        public string NightRateTimeEnd { get; set; } = string.Empty;
        public int ChargeMaxCurrent { get; set; }
        public string HeaterSource { get; set; } = string.Empty;
    }
}