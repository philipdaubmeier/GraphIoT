using RichardSzalay.MockHttp;
using System.Net.Http;

namespace PhilipDaubmeier.WeConnectClient.Tests
{
    public static class MockCookieHttpMessageHandlerExtensions
    {
        public static MockedRequest When(this MockCookieHttpMessageHandler handler, HttpMethod method, string url)
        {
            var message = ((MockHttpMessageHandler)handler).When(method, url);
            handler.AddMockedRequest(message);
            return message;
        }

        public static MockedRequest When(this MockCookieHttpMessageHandler handler, string url)
        {
            var message = ((MockHttpMessageHandler)handler).When(url);
            handler.AddMockedRequest(message);
            return message;
        }
    }
}