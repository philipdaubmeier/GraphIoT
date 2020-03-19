using System.Net.Http;

namespace PhilipDaubmeier.ViessmannClient.Model
{
    public interface IViessmannConnectionProvider<T>
    {
        /// <summary>
        /// Credential information and a token store object.
        /// </summary>
        IViessmannAuth AuthData { get; }

        /// <summary>
        /// A HttpClient to inject for the connection to the Viessmann servers,
        /// to be used for working with HttpClientFactory pattern or for test mocking purposes.
        /// </summary>
        HttpClient Client { get; }

        /// <summary>
        /// A HttpClient to inject for authentication that does not follow redirects.
        /// </summary>
        HttpClient AuthClient { get; }

        string VitotrolDeviceId { get; }
        string VitotrolInstallationId { get; }

        string PlattformInstallationId { get; }
        string PlattformGatewayId { get; }
        string PlattformApiClientId { get; }
        string PlattformApiClientSecret { get; }
    }
}