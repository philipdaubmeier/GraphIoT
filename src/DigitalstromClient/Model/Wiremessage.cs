namespace PhilipDaubmeier.DigitalstromClient.Model
{
    internal class Wiremessage<T> where T : class, IWiremessagePayload
    {
        private bool? _ok;
        private string _message;
        private T _result;

        public bool Ok
        {
            get
            {
                return _ok ?? false;
            }
            set
            {
                _ok = value;
            }
        }

        public string Message
        {
            get
            {
                if (_ok == null)
                    return "Warning! No OK value set at all!";

                return Ok ? "Success" : _message;
            }
            set
            {
                _message = value;
            }
        }

        public T Result
        {
            get
            {
                return Ok ? _result : null;
            }
            set
            {
                _result = value;
            }
        }
    }
}