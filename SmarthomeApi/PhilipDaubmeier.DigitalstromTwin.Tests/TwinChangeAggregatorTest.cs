using PhilipDaubmeier.DigitalstromClient;
using PhilipDaubmeier.DigitalstromClient.Model.Core;
using PhilipDaubmeier.DigitalstromClient.Model.Events;
using PhilipDaubmeier.DigitalstromDssMock;
using RichardSzalay.MockHttp;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace PhilipDaubmeier.DigitalstromTwin.Tests
{
    public class TwinChangeAggregatorTest
    {
        private readonly Zone zoneKitchen = 32027;

        [Fact]
        public async Task TestSceneCallOnModelChangeEmptyModel()
        {
            var mockHttp = new MockHttpMessageHandler();
            mockHttp.AddInitialAndSubscribeMocks();

            var callSceneRequest1 = mockHttp.AddCallSceneMock(zoneKitchen, Color.Yellow, SceneCommand.Preset1);
            var callSceneRequest2 = mockHttp.AddCallSceneMock(zoneKitchen, Color.Black, SceneCommand.DeepOff);

            var model = new ApartmentState();
            using (var dssClient = new DigitalstromDssClient(mockHttp.AddAuthMock().ToMockProvider()))
            using (var subscriber = new DssEventSubscriber(dssClient, new List<SystemEventName>() { SystemEvent.CallScene }, 42))
            using (var aggregator = new TwinChangeAggregator(model))
            {
                aggregator.SceneChangedInternal += (s, e) => dssClient.CallScene(e.Zone, e.Group, e.Scene).Wait();

                Assert.Equal(0, mockHttp.GetMatchCount(callSceneRequest1));
                Assert.Equal(0, mockHttp.GetMatchCount(callSceneRequest2));

                await mockHttp.WaitForCallSceneAsync(callSceneRequest1, () =>
                {
                    model[zoneKitchen, Color.Yellow].Value = SceneCommand.Preset1;
                });

                Assert.Equal(1, mockHttp.GetMatchCount(callSceneRequest1));
                Assert.Equal(0, mockHttp.GetMatchCount(callSceneRequest2));

                // even setting the same value again should result in a scene call
                await mockHttp.WaitForCallSceneAsync(callSceneRequest1, () =>
                {
                    model[zoneKitchen, Color.Yellow].Value = SceneCommand.Preset1;
                });

                Assert.Equal(2, mockHttp.GetMatchCount(callSceneRequest1));
                Assert.Equal(0, mockHttp.GetMatchCount(callSceneRequest2));

                await mockHttp.WaitForCallSceneAsync(callSceneRequest2, () =>
                {
                    model[zoneKitchen, Color.Black].Value = SceneCommand.DeepOff;
                });

                Assert.Equal(2, mockHttp.GetMatchCount(callSceneRequest1));
                Assert.Equal(1, mockHttp.GetMatchCount(callSceneRequest2));
            }
        }

        [Fact]
        public async Task TestSceneCallOnModelChangePrefilledModel()
        {
            var mockHttp = new MockHttpMessageHandler();
            mockHttp.AddInitialAndSubscribeMocks();

            var callSceneRequest1 = mockHttp.AddCallSceneMock(zoneKitchen, Color.Yellow, SceneCommand.Preset1);
            var callSceneRequest2 = mockHttp.AddCallSceneMock(zoneKitchen, Color.Black, SceneCommand.DeepOff);

            var model = new ApartmentState();
            model[zoneKitchen, Color.Yellow].Value = SceneCommand.Preset0;
            model[zoneKitchen, Color.Black].Value = SceneCommand.Preset0;

            using (var dssClient = new DigitalstromDssClient(mockHttp.AddAuthMock().ToMockProvider()))
            using (var subscriber = new DssEventSubscriber(dssClient, new List<SystemEventName>() { SystemEvent.CallScene }, 42))
            using (var aggregator = new TwinChangeAggregator(model))
            {
                aggregator.SceneChangedInternal += (s, e) => dssClient.CallScene(e.Zone, e.Group, e.Scene).Wait();

                Assert.Equal(0, mockHttp.GetMatchCount(callSceneRequest1));
                Assert.Equal(0, mockHttp.GetMatchCount(callSceneRequest2));

                await mockHttp.WaitForCallSceneAsync(callSceneRequest1, () =>
                {
                    model[zoneKitchen, Color.Yellow].Value = SceneCommand.Preset1;
                });

                Assert.Equal(1, mockHttp.GetMatchCount(callSceneRequest1));
                Assert.Equal(0, mockHttp.GetMatchCount(callSceneRequest2));

                // even setting the same value again should result in a scene call
                await mockHttp.WaitForCallSceneAsync(callSceneRequest1, () =>
                {
                    model[zoneKitchen, Color.Yellow].Value = SceneCommand.Preset1;
                });

                Assert.Equal(2, mockHttp.GetMatchCount(callSceneRequest1));
                Assert.Equal(0, mockHttp.GetMatchCount(callSceneRequest2));

                await mockHttp.WaitForCallSceneAsync(callSceneRequest2, () =>
                {
                    model[zoneKitchen, Color.Black].Value = SceneCommand.DeepOff;
                });

                Assert.Equal(2, mockHttp.GetMatchCount(callSceneRequest1));
                Assert.Equal(1, mockHttp.GetMatchCount(callSceneRequest2));
            }
        }
    }
}