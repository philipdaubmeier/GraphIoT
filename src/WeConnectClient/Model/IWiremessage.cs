namespace PhilipDaubmeier.WeConnectClient.Model
{
    internal interface IWiremessage<T> where T : class
    {
        string ErrorCode { get; }
        bool HasError { get; }
        T Body { get; }
    }
}