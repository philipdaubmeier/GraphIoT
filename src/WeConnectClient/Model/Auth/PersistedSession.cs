using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json.Serialization;

namespace PhilipDaubmeier.WeConnectClient.Model.Auth
{
    /// <summary>
    /// Helper class for serializing and deserializing all necessary session data to stay
    /// logged in even if the application is restarted, i.e. the CSRF token, the portal
    /// base uri and all cookies including the session cookie containing the cryptographic
    /// proof of the current login state.
    /// </summary>
    internal class PersistedSession
    {
        public PersistedSession(string baseJsonUri, string csrf, CookieContainer cookieContainer)
        {
            BaseJsonUri = baseJsonUri;
            Csrf = csrf;
            Cookies = cookieContainer.GetCookies(BaseUri).Cast<Cookie>().Select(c => new PersistedCookie(c)).ToList();
        }

        [JsonPropertyName("u")]
        public string BaseJsonUri { get; set; }

        [JsonPropertyName("csrf")]
        public string Csrf { get; set; }

        [JsonPropertyName("c")]
        public List<PersistedCookie> Cookies { get; set; }

        public Uri BaseUri => new Uri(new Uri(BaseJsonUri), "/");

        public void AddToCookieContainer(CookieContainer cookieContainer)
        {
            foreach (var cookie in Cookies.Select(c => c.ToCookie(BaseUri)))
                cookieContainer.Add(cookie);
        }
    }
}