using PhilipDaubmeier.DigitalstromClient.Model.Core;
using PhilipDaubmeier.DigitalstromClient.Model.Events;
using PhilipDaubmeier.DigitalstromDssMock;
using RichardSzalay.MockHttp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace PhilipDaubmeier.DigitalstromClient.Twin.Tests
{
    public class TwinEventSubscriberTest
    {
        [Fact]
        public async Task TestEventSubscription()
        {
            var mockHttp = new MockHttpMessageHandler();

            var subscription = mockHttp.When($"{MockDigitalstromConnection.BaseUri}/json/event/subscribe")
                                       .WithQueryString($"token={MockDigitalstromConnection.AppToken}")
                                       .Respond("application/json", @"{ ""ok"": true }");

            using (var eventSubscriber = new DssEventSubscriber(mockHttp.AddAuthMock().ToMockProvider()))
            {
                // Wait for the threads to start and make the subscriptions
                await Task.Delay(100);
            }
            
            Assert.Equal(2, mockHttp.GetMatchCount(subscription));
        }

        [Fact]
        public async Task TestEventsNoErrors()
        {
            var mockHttp = new MockHttpMessageHandler();
            mockHttp.AddInitialAndSubscribeMocks();

            var mockedEvent = mockHttp.When($"{MockDigitalstromConnection.BaseUri}/json/event/get")
                    .WithExactQueryString($"subscriptionID=42&timeout=60000&token={MockDigitalstromConnection.AppToken}")
                    .Respond("application/json", SceneCommand.Preset0.ToMockedSceneEvent());

            using (var subscriber = new DssEventSubscriber(mockHttp.AddAuthMock().ToMockProvider(), null, 42))
            {
                await Task.Delay(100);
                mockHttp.AutoFlush = false;
                try { mockHttp.Flush(); } catch { }

                var events = new List<DssEvent>();
                var errors = new List<Exception>();
                subscriber.ApiEventRaised += (s, e) => { events.Add(e.ApiEvent); };
                subscriber.ErrorOccured += (s, e) => { errors.Add(e.Error); };

                await mockHttp.MockDssEventAsync(subscriber, mockedEvent, SceneCommand.Preset1.ToMockedSceneEvent());

                await mockHttp.MockDssEventAsync(subscriber, mockedEvent, SceneCommand.Preset2.ToMockedSceneEvent());

                await mockHttp.MockDssEventAsync(subscriber, mockedEvent, SceneCommand.Preset1.ToMockedSceneEvent());

                Assert.NotEmpty(events.Where(e => e.SystemEvent == SystemEvent.CallScene));
                Assert.Empty(errors);
            }
        }

        [Fact]
        public async Task TestNotifyChangedSceneModel()
        {
            var mockHttp = new MockHttpMessageHandler();
            mockHttp.AddInitialAndSubscribeMocks();

            var mockedEvent = mockHttp.When($"{MockDigitalstromConnection.BaseUri}/json/event/get")
                    .WithExactQueryString($"subscriptionID=42&timeout=60000&token={MockDigitalstromConnection.AppToken}")
                    .Respond("application/json", SceneCommand.Preset0.ToMockedSceneEvent());

            using (var subscriber = new DssEventSubscriber(mockHttp.AddAuthMock().ToMockProvider(), null, 42))
            {
                await Task.Delay(300);
                mockHttp.AutoFlush = false;
                try { mockHttp.Flush(); } catch { }

                int numChangedEvents = 0;
                subscriber.ApiEventRaised += (s, e) => { numChangedEvents++; };

                await mockHttp.MockDssEventAsync(subscriber, mockedEvent, SceneCommand.Preset1.ToMockedSceneEvent());

                Assert.Equal(1, numChangedEvents);

                await mockHttp.MockDssEventAsync(subscriber, mockedEvent, SceneCommand.Preset2.ToMockedSceneEvent());

                Assert.Equal(2, numChangedEvents);

                await mockHttp.MockDssEventAsync(subscriber, mockedEvent, SceneCommand.Preset1.ToMockedSceneEvent());

                Assert.Equal(3, numChangedEvents);
            }
        }
    }
}