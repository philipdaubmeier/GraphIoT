namespace PhilipDaubmeier.NetatmoClient.Model
{
    public interface IWiremessage<T> where T : class
    {
        T? Body { get; }
        string Status { get; }
        double TimeExec { get; }
        int TimeServer { get; }
    }
}