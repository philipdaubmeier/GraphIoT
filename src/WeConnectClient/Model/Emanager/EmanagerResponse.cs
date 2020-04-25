using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace PhilipDaubmeier.WeConnectClient.Model.Emanager
{
    public class EmanagerResponse
    {
        public string ErrorCode { get; set; } = string.Empty;

        [JsonPropertyName("EManager")]
        public Emanager Emanager { get; set; } = new Emanager();
    }

    public class Emanager
    {
        public Rbc Rbc { get; set; } = new Rbc();
        public Rpc Rpc { get; set; } = new Rpc();
        public Rdt Rdt { get; set; } = new Rdt();
        public bool ActionPending { get; set; }
        public bool RdtAvailable { get; set; }
    }

    public class Rbc
    {
        public RbcStatus? Status { get; set; }
        public RbcSettings? Settings { get; set; }
    }

    public class Rpc
    {
        public RpcStatus? Status { get; set; }
        public RpcSettings? Settings { get; set; }
        public string ClimaterActionState { get; set; } = string.Empty;
        public bool AuAvailable { get; set; }
    }

    public class Rdt
    {
        public RdtStatus? Status { get; set; }
        public RdtSettings? Settings { get; set; }
        public bool AuxHeatingAllowed { get; set; }
        public bool AuxHeatingEnabled { get; set; }
    }

    public class RbcStatus
    {
        public int BatteryPercentage { get; set; }
        public string ChargingState { get; set; } = string.Empty;
        public string ChargingRemaningHour { get; set; } = string.Empty;
        public string ChargingRemaningMinute { get; set; } = string.Empty;
        public string ChargingReason { get; set; } = string.Empty;
        public string PluginState { get; set; } = string.Empty;
        public string LockState { get; set; } = string.Empty;
        public string ExtPowerSupplyState { get; set; } = string.Empty;
        public string Range { get; set; } = string.Empty;
        public int? ElectricRange { get; set; }
        public int? CombustionRange { get; set; }
        public int CombinedRange { get; set; }
        public bool RlzeUp { get; set; }
    }

    public class RbcSettings
    {
        public int ChargerMaxCurrent { get; set; }
        public int MaxAmpere { get; set; }
        public bool MaxCurrentReduced { get; set; }
    }

    public class RpcStatus
    {
        public string ClimatisationState { get; set; } = string.Empty;
        public int ClimatisationRemaningTime { get; set; }
        public object? WindowHeatingStateFront { get; set; }
        public object? WindowHeatingStateRear { get; set; }
        public object? ClimatisationReason { get; set; }
        public bool WindowHeatingAvailable { get; set; }
    }

    public class RpcSettings
    {
        public string TargetTemperature { get; set; } = string.Empty;
        public bool ClimatisationWithoutHVPower { get; set; }
        public bool Electric { get; set; }
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
        public Start Start { get; set; } = new Start();
        public End End { get; set; } = new End();
        public int? Index { get; set; }
        public List<string> Daypicker { get; set; } = new List<string>();
        public string StartDateActive { get; set; } = string.Empty;
        public string EndDateActive { get; set; } = string.Empty;
    }

    public class Start
    {
        public int? Hours { get; set; }
        public int? Minutes { get; set; }
    }

    public class End
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