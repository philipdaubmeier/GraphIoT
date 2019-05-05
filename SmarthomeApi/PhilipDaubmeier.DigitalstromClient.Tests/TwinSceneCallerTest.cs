using PhilipDaubmeier.DigitalstromClient.Model.Core;
using PhilipDaubmeier.DigitalstromClient.Tests;
using RichardSzalay.MockHttp;
using System.Threading.Tasks;
using Xunit;

namespace PhilipDaubmeier.DigitalstromClient.Twin.Tests
{
    public class TwinSceneCallerTest
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
            using (var subscriber = new DssEventSubscriber(mockHttp.AddAuthMock().ToMockProvider(), null, null, 42))
            using (var sceneCaller = new DssSceneCaller(subscriber, model))
            {
                Assert.Equal(0, mockHttp.GetMatchCount(callSceneRequest1));
                Assert.Equal(0, mockHttp.GetMatchCount(callSceneRequest2));

                model[zoneKitchen, Color.Yellow].Value = SceneCommand.Preset1;
                await mockHttp.WaitForCallSceneAsync(callSceneRequest1);

                Assert.Equal(1, mockHttp.GetMatchCount(callSceneRequest1));
                Assert.Equal(0, mockHttp.GetMatchCount(callSceneRequest2));

                // even setting the same value again should result in a scene call
                model[zoneKitchen, Color.Yellow].Value = SceneCommand.Preset1;
                await mockHttp.WaitForCallSceneAsync(callSceneRequest1);

                Assert.Equal(2, mockHttp.GetMatchCount(callSceneRequest1));
                Assert.Equal(0, mockHttp.GetMatchCount(callSceneRequest2));

                model[zoneKitchen, Color.Black].Value = SceneCommand.DeepOff;
                await mockHttp.WaitForCallSceneAsync(callSceneRequest2);

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

            using (var subscriber = new DssEventSubscriber(mockHttp.AddAuthMock().ToMockProvider(), null, null, 42))
            using (var sceneCaller = new DssSceneCaller(subscriber, model))
            {
                Assert.Equal(0, mockHttp.GetMatchCount(callSceneRequest1));
                Assert.Equal(0, mockHttp.GetMatchCount(callSceneRequest2));

                model[zoneKitchen, Color.Yellow].Value = SceneCommand.Preset1;
                await mockHttp.WaitForCallSceneAsync(callSceneRequest1);

                Assert.Equal(1, mockHttp.GetMatchCount(callSceneRequest1));
                Assert.Equal(0, mockHttp.GetMatchCount(callSceneRequest2));

                // even setting the same value again should result in a scene call
                model[zoneKitchen, Color.Yellow].Value = SceneCommand.Preset1;
                await mockHttp.WaitForCallSceneAsync(callSceneRequest1);

                Assert.Equal(2, mockHttp.GetMatchCount(callSceneRequest1));
                Assert.Equal(0, mockHttp.GetMatchCount(callSceneRequest2));

                model[zoneKitchen, Color.Black].Value = SceneCommand.DeepOff;
                await mockHttp.WaitForCallSceneAsync(callSceneRequest2);

                Assert.Equal(2, mockHttp.GetMatchCount(callSceneRequest1));
                Assert.Equal(1, mockHttp.GetMatchCount(callSceneRequest2));
            }
        }
    }
}