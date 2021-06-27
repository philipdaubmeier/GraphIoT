using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PhilipDaubmeier.DigitalstromClient.Model.Core;
using PhilipDaubmeier.DigitalstromClient.Model.Events;
using PhilipDaubmeier.DigitalstromDssMock;
using PhilipDaubmeier.DigitalstromTimeSeriesApi.Database;
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

            static async Task DelayUntilDataWritten(IntegrationTestDbContext db, int expected)
            {
                for (int i = 0; i < 100; i++)
                {
                    await Task.Delay(100);
                    if (db.DsSceneEventDataSet.AsNoTracking().FirstOrDefault()?.EventStreamEncoded != null &&
                        db.DsSceneEventDataSet.AsNoTracking().FirstOrDefault()?.EventStream.Count() > 1 &&
                        db.DsSceneEventDataSet.AsNoTracking().FirstOrDefault()?.EventStream.Last().Properties.SceneID == expected)
                        break;
                }
            }

            // After 100ms (see configured ItemCollectionInterval, plus buffer) the event processor hosted service
            // should have written all ds events to the database that we have mocked with the test setup
            await DelayUntilDataWritten(db, (int)SceneCommand.Preset0);
            Assert.NotNull(db.DsSceneEventDataSet.AsNoTracking().FirstOrDefault()?.EventStreamEncoded);

            var storedEvents = db.DsSceneEventDataSet.AsNoTracking().FirstOrDefault()?.EventStream;
            Assert.True(!(storedEvents is null) && storedEvents.Count() > 1);
            Assert.Equal(32027, storedEvents is null ? -1 : (int)storedEvents.Last().Properties.ZoneID);
            Assert.Equal((int)SceneCommand.Preset0, storedEvents is null ? -1 : (int)storedEvents.First().Properties.SceneID);
            Assert.Equal((int)SystemEvent.CallScene, storedEvents is null ? -1 : (int)storedEvents.First().SystemEvent.Type);

            // Turn off event sending
            mockHttp.AutoFlush = false;
            mockHttp.Flush();

            // Change the returned event content and reset the database
            await _factory.InitDb();
            await Task.Delay(200);

            _factory.MockedEventResponse.Respond("application/json", SceneCommand.Alarm1.ToMockedSceneEvent());

            mockHttp.Flush();

            await DelayUntilDataWritten(db, (int)SceneCommand.Alarm1);

            storedEvents = db.DsSceneEventDataSet.AsNoTracking().FirstOrDefault()?.EventStream;
            Assert.NotNull(storedEvents);
            Assert.Equal((int)SceneCommand.Alarm1, storedEvents is null ? -1 : (int)storedEvents.Last().Properties.SceneID);
        }
    }
}