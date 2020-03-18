using PhilipDaubmeier.DigitalstromClient.Model.Auth;
using PhilipDaubmeier.DigitalstromClient.Network;
using System.Net.Http;

namespace PhilipDaubmeier.DigitalstromClient
{
    public interface IDigitalstromConnectionProvider
    {
        /// <summary>
        /// A list of uris where the Digitalstrom server can be reached. Usually used
        /// with either exacly one uri or with two uris: one local LAN uri and an
        /// internet exposed one as fallback.
        /// </summary>
        UriPriorityList Uris { get; }

        /// <summary>
        /// Credential information and a token store object.
        /// </summary>
        IDigitalstromAuth AuthData { get; }

        /// <summary>
        /// Returnes a HttpClient instance or createes one if not existing. This property
        /// will be fetched typically once per DigitalstromDssClient instance and could
        /// be used with HttpClientFactory.CreateClient. Whatever the exact implementation, the
        /// IDigitalstromConnectionProvider implementing class is responsible for
        /// disposing the created client or make sure the lifecycle is already been taken
        /// care of (like in case of the HttpClientFactory).
        /// </summary>
        HttpClient HttpClient { get; }

        /// <summary>
        /// Returnes a HttpClient instance with special setup for long polling of digitalstrom
        /// events. Implementations can opt return a reference to the same instance of the
        /// property HttpClient per default.
        /// </summary>
        HttpClient LongPollingHttpClient { get; }
    }
}