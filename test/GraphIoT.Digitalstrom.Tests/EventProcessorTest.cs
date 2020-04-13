using Microsoft.Extensions.DependencyInjection;
using PhilipDaubmeier.DigitalstromClient.Model.Core;
using PhilipDaubmeier.DigitalstromClient.Model.Events;
using PhilipDaubmeier.DigitalstromDssMock;
using RichardSzalay.MockHttp;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace PhilipDaubmeier.GraphIoT.Digitalstrom.Tests
{
    public class EventProcessorTest : IClassFixture<IntegrationTestWebHostFactory<IntegrationTestStartup>>
    {
        private readonly IntegrationTestWebHostFactory<IntegrationTestStartup> _factory;

        public EventProcessorTest(IntegrationTestWebHostFactory<IntegrationTestStartup> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task TestDsSceneEventProcessorHostedService()
        {
            // Create a client, which in turn creates the host with all hosted services
            // and the digitalstrom event processor service will start running
            _factory.CreateClient();

            // Init the mock for http requests to the digitalstrom server and mock for the database
            var mockHttp = _factory.Server.Host.Services.GetRequiredService<MockHttpMessageHandler>();
            var db = await _factory.InitDb();

            // We should see that the database is empty at first, with no recorded digitalstrom events
            Assert.Null(db.DsSceneEventDataSet.FirstOrDefault()?.EventStreamEncoded);

            // After 100ms (see configured ItemCollectionInterval, plus buffer) the event processor hosted service
            // should have written all ds events to the database that we have mocked with the test setup
            for (int i = 0; i < 100; i++)
            {
                await Task.Delay(100);
                if (db.DsSceneEventDataSet.FirstOrDefault()?.EventStreamEncoded != null)
                    break;
            }
            Assert.NotNull(db.DsSceneEventDataSet.FirstOrDefault()?.EventStreamEncoded);

            var storedEvents = db.DsSceneEventDataSet.FirstOrDefault()?.EventStream;
            Assert.True(storedEvents.Count() > 1);
            Assert.Equal(32027, (int)storedEvents.Last().Properties.ZoneID);
            Assert.Equal((int)SceneCommand.Preset0, (int)storedEvents.First().Properties.SceneID);
            Assert.Equal((int)SystemEvent.CallScene, (int)storedEvents.First().SystemEvent.Type);

            // Turn off event sending
            mockHttp.AutoFlush = false;
            mockHttp.Flush();
            await Task.Delay(200);

            // Change the returned event content and reset the database
            _factory.MockedEventResponse.Respond("application/json", SceneCommand.Alarm1.ToMockedSceneEvent());
            await _factory.InitDb();

            mockHttp.Flush();

            await Task.Delay(200);

            storedEvents = db.DsSceneEventDataSet.FirstOrDefault()?.EventStream;
            Assert.Equal((int)SceneCommand.Preset0, (int)storedEvents?.Last().Properties.SceneID);
        }
    }
}