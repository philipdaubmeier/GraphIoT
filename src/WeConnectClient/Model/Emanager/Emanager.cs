namespace PhilipDaubmeier.WeConnectClient.Model.Emanager
{
    public class Emanager
    {
        public Rbc Rbc { get; set; } = new Rbc();
        public Rpc Rpc { get; set; } = new Rpc();
        public Rdt Rdt { get; set; } = new Rdt();
        public bool ActionPending { get; set; }
        public bool RdtAvailable { get; set; }
    }
}