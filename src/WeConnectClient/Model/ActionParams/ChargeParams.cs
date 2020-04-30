namespace PhilipDaubmeier.WeConnectClient.Model.ActionParams
{
    internal class ChargeParams
    {
        public ChargeParams(bool start, int batteryPercent)
            => (TriggerAction, BatteryPercent) = (start, batteryPercent.ToString());

        public bool TriggerAction { get; }
        public string BatteryPercent { get; }
    }
}