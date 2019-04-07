using DigitalstromClient.Network;
using System.Net;

namespace DigitalstromClient.Model
{
    public interface IDigitalstromConnectionProvider
    {
        UriPriorityList Uris { get; }
        IDigitalstromAuth AuthData { get; }
        WebProxy Proxy { get; }
    }
}