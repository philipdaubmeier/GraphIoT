using RichardSzalay.MockHttp;
using RichardSzalay.MockHttp.Matchers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace PhilipDaubmeier.WeConnectClient.Tests
{
    public class MockCookieHttpMessageHandler : MockHttpMessageHandler
    {
        public const string CookieHeaderKey = "Cookie";
        public const string SetCookieHeaderKey = "Set-Cookie";

        public CookieContainer? CookieContainer { get; set; }
        public bool FollowRedirects { get; set; }

        private readonly List<IMockedRequest> mockedRequests = new();

        public MockCookieHttpMessageHandler(bool followRedirects = true) => FollowRedirects = followRedirects;

        public void AddMockedRequest(IMockedRequest mock) => mockedRequests.Add(mock);

        /// <summary>
        /// Adds all matching cookies of the CookieContainer to the response and processes
        /// all Set-Cookie response headers by adding them to the cookie container.
        /// Follows all 301 MovedPermanently, 302 Found and 303 SeeOther redirects.
        /// </summary>
        /// <param name="request">The request being sent</param>
        /// <param name="cancellationToken">The token used to cancel the request</param>
        /// <returns>A Task containing the future response message</returns>
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var response = await SendAsyncWithCookies(request, cancellationToken);

            response = await FollowRedirect(request, response, cancellationToken);

            return response;
        }

        private async Task<HttpResponseMessage> FollowRedirect(HttpRequestMessage request, HttpResponseMessage response, CancellationToken cancellationToken)
        {
            var redirects = new[] { HttpStatusCode.MovedPermanently, HttpStatusCode.Found, HttpStatusCode.SeeOther };
            while (FollowRedirects && redirects.Contains(response.StatusCode))
            {
                if (request?.RequestUri is null || response.Headers.Location is null)
                    return response;

                request = new HttpRequestMessage(HttpMethod.Get, new Uri(request.RequestUri, response.Headers.Location));
                response = await SendAsyncWithCookies(request, cancellationToken);
            }

            return response;
        }

        private async Task<HttpResponseMessage> SendAsyncWithCookies(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            AddStoredCookies(request);

            var response = await base.SendAsync(request, cancellationToken);

            ProcessReceivedCookies(response, CookieContainer);

            if (response.StatusCode == HttpStatusCode.NotFound)
                Assert.Equal(ExpectedCookies(request), request.Headers.GetValues(CookieHeaderKey).FirstOrDefault());

            return response;
        }

        private string ExpectedCookies(HttpRequestMessage request)
        {
            var matchersField = typeof(MockedRequest).GetFields(BindingFlags.NonPublic | BindingFlags.Instance).First(x => x.Name == "matchers");
            var headersField = typeof(HeadersMatcher).GetFields(BindingFlags.NonPublic | BindingFlags.Instance).First(x => x.Name == "headers");
            foreach (var mockedRequestInterface in mockedRequests)
            {
                if (mockedRequestInterface is not MockedRequest mockedRequest || matchersField is null)
                    continue;

                if (matchersField.GetValue(mockedRequest) is not List<IMockedRequestMatcher> matchers)
                    continue;

                if (!matchers.Where(x => x is not HeadersMatcher).All(x => x.Matches(request)))
                    continue;

                if (matchers.FirstOrDefault(x => x is HeadersMatcher) is not HeadersMatcher headersMatcher)
                    continue;

                if (headersField.GetValue(headersMatcher) is not IEnumerable<KeyValuePair<string, string>> headers)
                    continue;

                return headers.FirstOrDefault(x => x.Key == CookieHeaderKey).Value;
            }
            return string.Empty;
        }

        private void AddStoredCookies(HttpRequestMessage request)
        {
            var cookies = request.RequestUri is null ? new() : CookieContainer?.GetCookies(request.RequestUri).ToList() ?? new();
            if (cookies.Count > 0)
                request.Headers.Add(CookieHeaderKey, string.Join("; ", cookies.Select(x => $"{x.Name}={x.Value}")));
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
        public static void ProcessReceivedCookies(HttpResponseMessage response, CookieContainer? cookieContainer)
        {
            if (cookieContainer is null)
                return;

            if (!response.Headers.TryGetValues(SetCookieHeaderKey, out IEnumerable<string>? values))
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
}