namespace PhilipDaubmeier.SonnenClient.Model
{
    public interface IWiremessage<T> where T : class
    {
        T? ContainedData { get; }
    }
}