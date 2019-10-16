namespace PhilipDaubmeier.DigitalstromClient.Model
{
    internal class Wiremessage<T> where T : class, IWiremessagePayload
    {
        private bool? _ok;
        private string _message;
        private T _result;

        public bool Ok
        {
            get => _ok ?? false;
            set => _ok = value;
        }

        public string Message
        {
            get => _ok == null ? "Warning! No OK value set at all!" : Ok ? "Success" : _message;
            set => _message = value;
        }

        public T Result
        {
            get => Ok ? _result : null;
            set => _result = value;
        }
    }
}