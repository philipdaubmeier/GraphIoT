using PhilipDaubmeier.DigitalstromClient.Model.Core;
using PhilipDaubmeier.DigitalstromDssMock;
using RichardSzalay.MockHttp;
using System.Threading.Tasks;
using Xunit;

namespace PhilipDaubmeier.DigitalstromTwin.Tests
{
    public class TwinMainTest
    {
        private readonly Zone zoneKitchen = 32027;

        [Fact]
        public async Task TestSceneCallOnTwinChange()
        {
            var mockHttp = new MockHttpMessageHandler();
            mockHttp.AddInitialAndSubscribeMocks();

            var callSceneRequest1 = mockHttp.AddCallSceneMock(zoneKitchen, Color.Yellow, SceneCommand.Preset1);
            var callSceneRequest2 = mockHttp.AddCallSceneMock(zoneKitchen, Color.Black, SceneCommand.DeepOff);

            using var twin = new DigitalstromDssTwin(mockHttp.AddAuthMock().ToMockProvider());
            await twin.WaitModelInitializedAsync(2);

            Assert.Equal(0, mockHttp.GetMatchCount(callSceneRequest1));
            Assert.Equal(0, mockHttp.GetMatchCount(callSceneRequest2));

            await mockHttp.WaitForCallSceneAsync(callSceneRequest1, () =>
            {
                twin[zoneKitchen, Color.Yellow].Value = SceneCommand.Preset1;
            });

            Assert.Equal(1, mockHttp.GetMatchCount(callSceneRequest1));
            Assert.Equal(0, mockHttp.GetMatchCount(callSceneRequest2));

            // even setting the same value again should result in a scene call
            await mockHttp.WaitForCallSceneAsync(callSceneRequest1, () =>
            {
                twin[zoneKitchen, Color.Yellow].Value = SceneCommand.Preset1;
            });

            Assert.Equal(2, mockHttp.GetMatchCount(callSceneRequest1));
            Assert.Equal(0, mockHttp.GetMatchCount(callSceneRequest2));

            await mockHttp.WaitForCallSceneAsync(callSceneRequest2, () =>
            {
                twin[zoneKitchen, Color.Black].Value = SceneCommand.DeepOff;
            });

            Assert.Equal(2, mockHttp.GetMatchCount(callSceneRequest1));
            Assert.Equal(1, mockHttp.GetMatchCount(callSceneRequest2));
        }

        [Fact]
        public async Task TestTwinChangeOnCallSceneEvent()
        {
            var mockHttp = new MockHttpMessageHandler();
            mockHttp.AddInitialAndSubscribeMocks();

            var mockedEvent = mockHttp.When($"{MockDigitalstromConnection.BaseUri}/json/event/get")
                    .WithExactQueryString($"subscriptionID=10&timeout=60000&token={MockDigitalstromConnection.AppToken}")
                    .Respond("application/json", SceneCommand.Preset0.ToMockedSceneEvent());

            var callSceneRequest1 = mockHttp.AddCallSceneMock(zoneKitchen, Color.Yellow, SceneCommand.Preset1);

            using var twin = new DigitalstromDssTwin(mockHttp.AddAuthMock().ToMockProvider());
            await twin.WaitModelInitializedAsync(3);
            mockHttp.AutoFlush = false;
            try { mockHttp.Flush(); } catch { }

            int numChangedEvents = 0;
            twin.SceneChanged += (s, e) => { numChangedEvents++; };

            Assert.Equal(0, mockHttp.GetMatchCount(callSceneRequest1));

            await mockHttp.MockDssEventAndWaitAsync(mockedEvent, twin, zoneKitchen, Color.Yellow, SceneCommand.Preset1);

            Assert.Equal((int)SceneCommand.Preset1, (int)twin[zoneKitchen, Color.Yellow].Value);
            Assert.Equal(0, mockHttp.GetMatchCount(callSceneRequest1));
            Assert.True(1 <= numChangedEvents);

            // even firing a dss event with the same value again should result in a scene call (due to a new timestamp of the scene state)
            await mockHttp.MockDssEventAndWaitAsync(mockedEvent, twin, zoneKitchen, Color.Yellow, SceneCommand.Preset1);

            Assert.Equal((int)SceneCommand.Preset1, (int)twin[zoneKitchen, Color.Yellow].Value);
            Assert.Equal(0, mockHttp.GetMatchCount(callSceneRequest1));
            Assert.True(2 <= numChangedEvents);

            await mockHttp.MockDssEventAndWaitAsync(mockedEvent, twin, zoneKitchen, Color.Yellow, SceneCommand.Preset2);

            Assert.Equal((int)SceneCommand.Preset2, (int)twin[zoneKitchen, Color.Yellow].Value);
            Assert.Equal(0, mockHttp.GetMatchCount(callSceneRequest1));
            Assert.True(3 <= numChangedEvents);
        }

        [Fact]
        public async Task TestTwinTwoWaySync()
        {
            var mockHttp = new MockHttpMessageHandler();
            mockHttp.AddInitialAndSubscribeMocks();

            var callSceneRequest = mockHttp.AddCallSceneMock(zoneKitchen, Color.Yellow, SceneCommand.Preset2);

            var mockedEvent = mockHttp.When($"{MockDigitalstromConnection.BaseUri}/json/event/get")
                    .WithExactQueryString($"subscriptionID=10&timeout=60000&token={MockDigitalstromConnection.AppToken}")
                    .Respond("application/json", SceneCommand.Preset0.ToMockedSceneEvent());

            using var twin = new DigitalstromDssTwin(mockHttp.AddAuthMock().ToMockProvider());
            await twin.WaitModelInitializedAsync(3);
            mockHttp.AutoFlush = false;
            try { mockHttp.Flush(); } catch { }

            // subscribe to twin changes of the yellow group in the kitchen
            var changedCount = 0;
            twin[zoneKitchen, Color.Yellow].PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == "Value")
                    changedCount++;
            };

            // the dss reports an event from outside the twin (maybe someone pressed a button, or called a scene via digitalstrom app)
            int changedCountBefore1 = changedCount;
            await mockHttp.MockDssEventAndWaitAsync(mockedEvent, twin, zoneKitchen, Color.Yellow, SceneCommand.Preset1);

            Assert.Equal((int)SceneCommand.Preset1, (int)twin[zoneKitchen, Color.Yellow].Value);
            Assert.True(changedCount > changedCountBefore1);

            // programatically set a new scene on the twin, which should result in a CallScene api request
            await mockHttp.WaitForCallSceneAsync(callSceneRequest, () =>
            {
                twin[zoneKitchen, Color.Yellow].Value = SceneCommand.Preset2;
            });

            Assert.Equal((int)SceneCommand.Preset2, (int)twin[zoneKitchen, Color.Yellow].Value);
            Assert.True(1 >= mockHttp.GetMatchCount(callSceneRequest));

            // the called scene in turn results in a dss event that is fired back to the twin
            int changedCountBefore2 = changedCount;
            await mockHttp.MockDssEventAndWaitAsync(mockedEvent, twin, zoneKitchen, Color.Yellow, SceneCommand.Preset2);

            Assert.Equal((int)SceneCommand.Preset2, (int)twin[zoneKitchen, Color.Yellow].Value);
            Assert.True(changedCount > changedCountBefore2);
        }
    }
}