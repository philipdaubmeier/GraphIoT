namespace PhilipDaubmeier.WeConnectClient.Model.Emanager
{
    public class Rbc
    {
        public RbcStatus? Status { get; set; }
        public RbcSettings? Settings { get; set; }
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
}