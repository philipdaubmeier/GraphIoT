using PhilipDaubmeier.DigitalstromClient.Model.Core;
using PhilipDaubmeier.DigitalstromDssMock;
using RichardSzalay.MockHttp;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace PhilipDaubmeier.DigitalstromClient.Twin.Tests
{
    public static class TwinMockExtensions
    {
        public static async Task MockDssEventAsync(this MockHttpMessageHandler mockHttp, DssEventSubscriber eventSubscriber, MockedRequest mockedEventResponse, string responseContent, int timeoutMillis = 1000)
        {
            DateTime start = DateTime.UtcNow;
            bool eventReceived = false;
            eventSubscriber.ApiEventRaised += (s, e) => eventReceived = true;

            // set the response and flush it - this will create a response in the long polling request in the event subscriber
            mockedEventResponse.Respond("application/json", responseContent);
            await Task.Delay(100);
            mockHttp.Flush();

            // give the event polling thread a chance to receive and handle the event, it will set the eventReceived
            while (!eventReceived && (DateTime.UtcNow - start).TotalMilliseconds < timeoutMillis)
                await Task.Delay(1);
        }

        public static async Task WaitForCallSceneAsync(this MockHttpMessageHandler mockHttp, MockedRequest mockedCallScene, Action setTwinAction, int timeoutMillis = 1000)
        {
            DateTime start = DateTime.UtcNow;
            var oldMatchCount = mockHttp.GetMatchCount(mockedCallScene);

            // execute the action that sets a value on the twin, which should trigger a CallScene request
            setTwinAction();

            // give the action worker thread a chance to consume the task and request the CallScene API
            while (oldMatchCount == mockHttp.GetMatchCount(mockedCallScene) && (DateTime.UtcNow - start).TotalMilliseconds < timeoutMillis)
                await Task.Delay(1);
        }

        public static async Task MockDssEventAndWaitAsync(this MockHttpMessageHandler mockHttp, MockedRequest mockedEventResponse, ApartmentState model, Zone zone, Group group, Scene scene, int timeoutMillis = 1000)
        {
            int numFlushed = 0;
            DateTime start = DateTime.UtcNow;
            var oldSceneTimestamp = model[zone, group].Timestamp;

            // set the response and flush it - this will create a response in the long polling request, just like the dss fired an event
            mockedEventResponse.Respond("application/json", scene.ToMockedSceneEvent(zone, group));
            mockHttp.Flush();

            // give the event polling thread a chance to receive and handle the event, notify the model and change it accordingly
            while ((oldSceneTimestamp == model[zone, group].Timestamp || scene != model[zone, group].Value)
                && (DateTime.UtcNow - start).TotalMilliseconds < timeoutMillis)
            {
                if ((DateTime.UtcNow - start).TotalMilliseconds > (numFlushed + 1) * 50)
                {
                    mockHttp.Flush();
                    numFlushed++;
                }   

                await Task.Delay(1);
            }
                
        }

        public static async Task WaitModelInitializedAsync(this ApartmentState model, int expectedCount, int timeoutMillis = 1000)
        {
            DateTime start = DateTime.UtcNow;

            int countAllItems() => model.Select(room => room.Value.Groups.Count() + room.Value.Sensors.Count()).Sum();

            // give the event polling thread a chance to receive and handle the event, notify the model and change it accordingly
            while (expectedCount != countAllItems() && (DateTime.UtcNow - start).TotalMilliseconds < timeoutMillis)
                await Task.Delay(1);
        }
    }
}