using RichardSzalay.MockHttp;
using System.Threading.Tasks;
using Xunit;

namespace PhilipDaubmeier.WeConnectClient.Tests
{
    public class WeConnectPortalClientTest
    {
        [Fact]
        public async Task TestGetEManager()
        {
            var mockHttp = new MockHttpMessageHandler();

            mockHttp.When($"{MockWeConnectConnection.BaseUri}/-/emanager/get-emanager")
                    .Respond("application/json",
                    @"{
                    }");

            var client = new WeConnectPortalClient(mockHttp.AddAuthMock().ToMockProvider());

            var result = await client.GetEManager();
        }
    }
}