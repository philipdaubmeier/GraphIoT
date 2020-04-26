namespace PhilipDaubmeier.NetatmoClient.Model
{
    public class Wiremessage<T> : IWiremessage<T> where T : class
    {
        public T? Body { get; set; }
        public string Status { get; set; } = string.Empty;
        public double? TimeExec { get; set; } = 0d;
        public int? TimeServer { get; set; }
        public ErrorInfo? Error { get; set; }
    }
}