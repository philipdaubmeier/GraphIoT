using RichardSzalay.MockHttp;
using System;
using System.Collections.Generic;
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

        /// <summary>
        /// Maps the request to the most appropriate configured response and processes
        /// all Set-Cookie response headers by adding them to the cookie container.
        /// </summary>
        /// <param name="request">The request being sent</param>
        /// <param name="cancellationToken">The token used to cancel the request</param>
        /// <returns>A Task containing the future response message</returns>
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var response = await base.SendAsync(request, cancellationToken);

            if (CookieContainer != null)
                ProcessReceivedCookies(response, CookieContainer);

            return response;
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

            Uri requestUri = response.RequestMessage.RequestUri;

            for (int i = 0; i < valuesArray.Length; i++)
                try { cookieContainer.SetCookies(requestUri, valuesArray[i]); } catch { }
        }
    }
}