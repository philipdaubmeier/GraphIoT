namespace PhilipDaubmeier.WeConnectClient.Model
{
    internal class Wiremessage<T> : IWiremessage<T> where T : class, new()
    {
        public string ErrorCode { get; set; } = string.Empty;

        public bool HasError => !int.TryParse(ErrorCode, out int errorCode) || errorCode != 0;

        public virtual T Body { get; set; } = new T();
    }
}