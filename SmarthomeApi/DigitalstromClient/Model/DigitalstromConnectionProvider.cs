using DigitalstromClient.Network;
using System.Net;

namespace DigitalstromClient.Model
{
    public class DigitalstromConnectionProvider : IDigitalstromConnectionProvider
    {
        public UriPriorityList Uris { get; private set; }
        public IDigitalstromAuth AuthData { get; private set; }
        public WebProxy Proxy { get; private set; }

        public DigitalstromConnectionProvider(UriPriorityList uris, IDigitalstromAuth authData, WebProxy proxy = null)
        {
            Uris = uris;
            AuthData = authData;
            Proxy = proxy;
        }
    }
}