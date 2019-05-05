using PhilipDaubmeier.DigitalstromClient.Model.Core;
using PhilipDaubmeier.DigitalstromClient.Model.Events;
using PhilipDaubmeier.DigitalstromClient.Tests;
using RichardSzalay.MockHttp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace PhilipDaubmeier.DigitalstromClient.Twin.Tests
{
    public class TwinSceneCallerTest
    {
        private readonly Zone zoneKitchen = 32027;

        [Fact]
        public void TestSceneCallOnModelChange()
        {
            var mockHttp = new MockHttpMessageHandler();
            mockHttp.AddInitialAndSubscribeMocks();

            using (var subscriber = new DssEventSubscriber(mockHttp.AddAuthMock().ToMockProvider(), null, null, 42))
            {
                var model = new ApartmentState();
                var sceneCaller = new DssSceneCaller(subscriber, model);

                model[zoneKitchen, Color.Yellow].Value = SceneCommand.Preset1;
            }
        }
    }
}