using PhilipDaubmeier.DigitalstromClient.Model.Auth;
using PhilipDaubmeier.DigitalstromClient.Network;
using System;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;

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
        /// If this property is returning a valid X509Certificate, only responses from
        /// the server is accepted if it responds with exactly this server certificate
        /// (which may also be a self-signed one), i.e. a valid digitally signed cert
        /// with the same fingerprint, issuer and serial number. If it returns null,
        /// any valid cert is accepted but no self-signed ones of a Digitalstrom server.
        /// </summary>
        X509Certificate2 ServerCertificate { get; }

        /// <summary>
        /// Gets or sets a callback method to validate the server certificate, which
        /// is called if no ServerCertificate is set.
        /// </summary>
        Func<X509Certificate2, bool> ServerCertificateValidationCallback { get; }

        /// <summary>
        /// A HttpMessageHandler to inject for the connection to the Digitalstrom server,
        /// either for mocking purposes for testing or also for setting a proxy server etc.
        /// If null is returned, the Digitalstrom clients will create a default handler.
        /// </summary>
        HttpMessageHandler Handler { get; }
    }
}