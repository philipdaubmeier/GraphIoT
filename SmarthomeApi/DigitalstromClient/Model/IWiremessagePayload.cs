namespace DigitalstromClient.Model
{
    public abstract class IWiremessagePayload<T> where T : class
    {
        public class Wiremessage : Wiremessage<T> { }
    }
}
