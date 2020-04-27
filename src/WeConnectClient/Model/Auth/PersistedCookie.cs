using System;
using System.Net;
using System.Text.Json.Serialization;

namespace PhilipDaubmeier.WeConnectClient.Model.Auth
{
    /// <summary>
    /// Helper class for serializing and deserializing a Cookie instance with a minimal footprint,
    /// i.e only attributes that are necessary for the WeConnect portal are persisted:
    /// name, value, host, path and expiry.
    /// </summary>
    internal class PersistedCookie
    {
        public PersistedCookie() => (Name, Value, Path, Domain, Expires)
            = (string.Empty, string.Empty, string.Empty, string.Empty, DateTime.MinValue);

        public PersistedCookie(Cookie cookie) => (Name, Value, Path, Domain, Expires)
            = (cookie.Name, cookie.Value, cookie.Path, cookie.Domain, cookie.Expires);

        [JsonPropertyName("n")]
        public string Name { get; set; }

        [JsonPropertyName("v")]
        public string Value { get; set; }

        [JsonPropertyName("p")]
        public string Path { get; set; }

        [JsonPropertyName("d")]
        public string Domain { get; set; }

        [JsonPropertyName("e")]
        public DateTime Expires { get; set; }

        public Cookie ToCookie(Uri baseUri)
            => new Cookie(Name, Value, baseUri.AbsolutePath, baseUri.Host)
            { HttpOnly = true, Secure = true, Expires = Expires };
    }
}