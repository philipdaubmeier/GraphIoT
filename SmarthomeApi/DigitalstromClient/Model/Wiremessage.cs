namespace DigitalstromClient.Model
{
    public abstract class Wiremessage<T> where T : class
    {
        private bool? _ok;
        private string _message;
        private T _result;

        public bool ok
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

        public string message
        {
            get
            {
                if (_ok == null)
                    return "Warning! No OK value set at all!";

                return ok ? "Success" : _message;
            }
            set
            {
                _message = value;
            }
        }

        public T result
        {
            get
            {
                return ok ? _result : null;
            }
            set
            {
                _result = value;
            }
        }
    }
}
