using RichardSzalay.MockHttp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace PhilipDaubmeier.WeConnectClient.Tests
{
    public class MockCookieHttpMessageHandler : MockHttpMessageHandler
    {
        private const string setCookieDescriptor = "Set-Cookie";

        public CookieContainer? CookieContainer { get; set; }
        public bool FollowRedirects { get; set; }

        public MockCookieHttpMessageHandler(bool followRedirects = true) => FollowRedirects = followRedirects;

        /// <summary>
        /// Adds all matching cookies of the CookieContainer to the response and processes
        /// all Set-Cookie response headers by adding them to the cookie container.
        /// </summary>
        /// <param name="request">The request being sent</param>
        /// <param name="cancellationToken">The token used to cancel the request</param>
        /// <returns>A Task containing the future response message</returns>
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            AddStoredCookies(request);

            var response = await base.SendAsync(request, cancellationToken);
            response = await FollowRedirect(request, response, cancellationToken);

            if (CookieContainer != null)
                ProcessReceivedCookies(response, CookieContainer);

            return response;
        }

        private async Task<HttpResponseMessage> FollowRedirect(HttpRequestMessage request, HttpResponseMessage response, CancellationToken cancellationToken)
        {
            var redirects = new[] { HttpStatusCode.MovedPermanently, HttpStatusCode.Found, HttpStatusCode.SeeOther };
            while (FollowRedirects && redirects.Contains(response.StatusCode))
            {
                if (request is null)
                    return response;

                request.RequestUri = new Uri(request?.RequestUri ?? new(string.Empty), response?.Headers?.Location ?? new(string.Empty));
                response = await base.SendAsync(request, cancellationToken);
            }

            return response;
        }

        private void AddStoredCookies(HttpRequestMessage request)
        {
            var cookies = request.RequestUri is null ? new() : CookieContainer?.GetCookies(request.RequestUri).ToList() ?? new();
            if (cookies.Count > 0)
                request.Headers.Add("Cookie", string.Join("; ", cookies.Select(x => $"{x.Name}={x.Value}")));
        }

        /// <summary>
        /// Simplified version of CookieHelper of dotnet runtime SocketsHttpHandler, which is used internally by HttpClientHandler:
        /// https://github.com/dotnet/runtime/blob/master/src/libraries/System.Net.Http/src/System/Net/Http/SocketsHttpHandler/CookieHelper.cs
        /// </summary>
        /// <remarks>
        /// The MIT License (MIT)
        /// Copyright(c) .NET Foundation and Contributors
        /// https://github.com/dotnet/runtime/blob/master/LICENSE.TXT
        /// </remarks>
        public static void ProcessReceivedCookies(HttpResponseMessage response, CookieContainer cookieContainer)
        {
            if (!response.Headers.TryGetValues(setCookieDescriptor, out IEnumerable<string>? values))
                return;

            // The header values are always a string[]
            var valuesArray = (string[])values;

            var requestUri = response.RequestMessage?.RequestUri;
            if (requestUri is null)
                return;

            for (int i = 0; i < valuesArray.Length; i++)
                try { cookieContainer.SetCookies(requestUri, valuesArray[i]); } catch { }
        }
    }

    public static class MockedRequestCookieExtensions
    {
        public static MockedRequest WithCookies(this MockedRequest source, IEnumerable<KeyValuePair<string, string>> cookies)
        {
            return source.WithCookiesAndHeaders(new List<KeyValuePair<string, string>>(), cookies);
        }

        public static MockedRequest WithCookiesAndHeaders(this MockedRequest source, IEnumerable<KeyValuePair<string, string>> headers, IEnumerable<KeyValuePair<string, string>> cookies)
        {
            var cookieHeader = string.Join("; ", cookies.Select(x => $"{x.Key}={x.Value}"));
            return source.WithHeaders(headers.Concat(new List<KeyValuePair<string, string>>() { new("Cookie", cookieHeader) }));
        }
    }
}