namespace PhilipDaubmeier.WeConnectClient.Model.ActionParams
{
    internal class WindowsMeltParams
    {
        public WindowsMeltParams(bool start)
            => TriggerAction = start;

        public bool TriggerAction { get; }
    }
}