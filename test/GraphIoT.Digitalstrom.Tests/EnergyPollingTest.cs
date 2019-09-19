using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace PhilipDaubmeier.GraphIoT.Digitalstrom.Tests
{
    public class EnergyPollingTest : IClassFixture<IntegrationTestWebHostFactory<IntegrationTestStartup>>
    {
        private readonly IntegrationTestWebHostFactory<IntegrationTestStartup> _factory;

        public EnergyPollingTest(IntegrationTestWebHostFactory<IntegrationTestStartup> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task TestEnergyPollingService()
        {
            // Create a client, which in turn creates the host with all hosted services
            // and the digitalstrom polling services will start running
            _factory.CreateClient();

            // Init the mock for the database
            var db = await _factory.InitDb();

            // We should see that the database is empty at first, with no recorded data
            Assert.Null(db.DsEnergyHighresDataSet.FirstOrDefault()?.EnergyCurvesEveryMeter);

            // After 100ms (see configured TimerInterval, plus buffer) the polling hosted service
            // should have written all values to the database that we have mocked with the test setup
            for (int i = 0; i < 10; i++)
            {
                await Task.Delay(100);
                if (db.DsEnergyHighresDataSet.FirstOrDefault()?.EnergyCurvesEveryMeter != null)
                    break;
            }
            Assert.NotNull(db.DsEnergyHighresDataSet.FirstOrDefault()?.EnergyCurvesEveryMeter);

            // We should have at least one polled dataset of 600 values (i.e. 10 min in a 1 sec interval)
            // for both metering circuits that we are mocking (see DigitalstromDssMockExtensions.AddEnergyMeteringMocks)
            var storedValues = db.DsEnergyHighresDataSet.FirstOrDefault()?.EnergySeriesEveryMeter;
            Assert.Equal(2, storedValues.Count());
            Assert.True(600 <= storedValues.First().Value.Trimmed().Count);
            Assert.True(600 <= storedValues.Last().Value.Trimmed().Count);
        }
    }
}