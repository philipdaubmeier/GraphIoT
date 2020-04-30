namespace PhilipDaubmeier.WeConnectClient.Model.ActionParams
{
    internal class ClimateParams
    {
        public ClimateParams(bool start, bool electricClima)
            => (TriggerAction, ElectricClima) = (start, electricClima);

        public bool TriggerAction { get; }
        public bool ElectricClima { get; }
    }
}