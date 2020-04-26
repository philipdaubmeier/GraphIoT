namespace PhilipDaubmeier.WeConnectClient.Model.Emanager
{
    public class Rpc
    {
        public RpcStatus? Status { get; set; }
        public RpcSettings? Settings { get; set; }
        public string ClimaterActionState { get; set; } = string.Empty;
        public bool AuAvailable { get; set; }
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
}