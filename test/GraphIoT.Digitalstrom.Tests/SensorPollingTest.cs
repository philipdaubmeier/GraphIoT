using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace PhilipDaubmeier.GraphIoT.Digitalstrom.Tests
{
    public class SensorPollingTest : IClassFixture<IntegrationTestWebHostFactory<IntegrationTestStartup>>
    {
        private readonly IntegrationTestWebHostFactory<IntegrationTestStartup> _factory;

        public SensorPollingTest(IntegrationTestWebHostFactory<IntegrationTestStartup> factory)
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
            Assert.Null(db.DsSensorDataSet.FirstOrDefault()?.TemperatureCurve);
            Assert.Null(db.DsSensorDataSet.FirstOrDefault()?.HumidityCurve);

            // After 100ms (see configured TimerInterval, plus buffer) the polling hosted service
            // should have written all values to the database that we have mocked with the test setup
            for (int i = 0; i < 10; i++)
            {
                await Task.Delay(100);
                if (db.DsSensorDataSet.FirstOrDefault()?.TemperatureCurve != null
                    && db.DsSensorDataSet.FirstOrDefault()?.HumidityCurve != null)
                    break;
            }
            Assert.NotNull(db.DsSensorDataSet.FirstOrDefault()?.TemperatureCurve);
            Assert.NotNull(db.DsSensorDataSet.FirstOrDefault()?.HumidityCurve);

            // for both temperature und humidity values that we are mocking (see DigitalstromDssMockExtensions.AddSensorMocks)
            var storedValuesTemperature = db.DsSensorDataSet.FirstOrDefault()?.TemperatureSeries;
            var storedValuesHumidity = db.DsSensorDataSet.FirstOrDefault()?.HumiditySeries;
            Assert.True(1 <= storedValuesTemperature.Trimmed().Count);
            Assert.True(1 <= storedValuesHumidity.Trimmed().Count);
        }
    }
}