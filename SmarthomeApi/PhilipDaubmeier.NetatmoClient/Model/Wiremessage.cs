namespace PhilipDaubmeier.NetatmoClient.Model
{
    public class Wiremessage<T> : IWiremessage<T> where T : class
    {
        public T Body { get; set; }
        public string Status { get; set; }
        public double TimeExec { get; set; }
        public int TimeServer { get; set; }
    }
}