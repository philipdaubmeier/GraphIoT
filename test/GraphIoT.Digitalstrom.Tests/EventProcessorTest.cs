using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PhilipDaubmeier.DigitalstromClient.Model.Core;
using PhilipDaubmeier.DigitalstromClient.Model.Events;
using PhilipDaubmeier.DigitalstromDssMock;
using PhilipDaubmeier.DigitalstromTimeSeriesApi.Database;
using RichardSzalay.MockHttp;
using System.Collections.Generic;
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
            Assert.NotNull(mockHttp);

            // We should see that the database is empty at first, with no recorded digitalstrom events
            Assert.Null(db.DsSceneEventDataSet.FirstOrDefault()?.EventStreamEncoded);

            async Task<IEnumerable<DssEvent>> DelayUntilDataWritten(IntegrationTestDbContext db, int expected)
            {
                for (int i = 0; i < 200; i++)
                {
                    await Task.Delay(100);

                    if (mockHttp is null)
                        continue;
                    mockHttp.Flush();

                    if (db.DsSceneEventDataSet.AsNoTracking().FirstOrDefault()?.EventStreamEncoded is null)
                        continue;

                    var expectedItems = db.DsSceneEventDataSet.AsNoTracking().FirstOrDefault()?.EventStream
                                          .Where(x => x.Properties.SceneID == expected).ToList();
                    if (expectedItems is not null && expectedItems.Count > 0)
                        return expectedItems;
                }
                return new List<DssEvent>();
            }

            // After 100ms (see configured ItemCollectionInterval, plus buffer) the event processor hosted service
            // should have written all ds events to the database that we have mocked with the test setup
            var storedEvents1 = await DelayUntilDataWritten(db, (int)SceneCommand.Preset0);
            Assert.NotNull(db.DsSceneEventDataSet.AsNoTracking().FirstOrDefault()?.EventStreamEncoded);

            Assert.NotNull(storedEvents1);
            if (storedEvents1 is null)
                return;
            Assert.NotEmpty(storedEvents1);
            Assert.Equal(32027, (int)storedEvents1.Last().Properties.ZoneID);
            Assert.Equal((int)SceneCommand.Preset0, (int)storedEvents1.First().Properties.SceneID);
            Assert.Equal((int)SystemEvent.CallScene, (int)storedEvents1.First().SystemEvent.Type);

            // Turn off event sending
            mockHttp.AutoFlush = false;
            mockHttp.Flush();

            // Change the returned event content and reset the database
            db.DsSceneEventDataSet.RemoveRange(db.DsSceneEventDataSet);
            await db.SaveChangesAsync();
            await _factory.InitDb();
            await Task.Delay(100);

            _factory.MockedEventResponse.Respond("application/json", SceneCommand.Alarm1.ToMockedSceneEvent());

            var storedEvents2 = await DelayUntilDataWritten(db, (int)SceneCommand.Alarm1);

            Assert.NotNull(storedEvents2);
            if (storedEvents2 is null)
                return;
            Assert.NotEmpty(storedEvents2);
            Assert.NotEmpty(storedEvents2.Where(x => x.Properties.SceneID == (int)SceneCommand.Alarm1));
        }
    }
}