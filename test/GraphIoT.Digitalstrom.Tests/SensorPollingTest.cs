using PhilipDaubmeier.DigitalstromDssMock;
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
        public async Task TestSensorPollingService()
        {
            // Create a client, which in turn creates the host with all hosted services
            // and the digitalstrom polling services will start running
            _factory.CreateClient();

            // Init the mock for the database
            var db = await _factory.InitDb();

            // We should see that the database is empty at first, with no recorded data
            var sensorDataSet = db.DsSensorDataSet.Where(x => x.ZoneId == DigitalstromDssMockExtensions.ZoneIdKitchen).FirstOrDefault();
            Assert.Null(sensorDataSet?.TemperatureCurve);
            Assert.Null(sensorDataSet?.HumidityCurve);

            // After 100ms (see configured TimerInterval, plus buffer) the polling hosted service
            // should have written all values to the database that we have mocked with the test setup
            for (int i = 0; i < 100; i++)
            {
                sensorDataSet = db.DsSensorDataSet.Where(x => x.ZoneId == DigitalstromDssMockExtensions.ZoneIdKitchen).FirstOrDefault();
                await Task.Delay(100);
                if (sensorDataSet?.TemperatureCurve != null && sensorDataSet?.HumidityCurve != null)
                    break;
            }
            Assert.NotNull(sensorDataSet);
            Assert.NotNull(sensorDataSet?.TemperatureCurve);
            Assert.NotNull(sensorDataSet?.HumidityCurve);

            // for both temperature und humidity values that we are mocking (see DigitalstromDssMockExtensions.AddSensorMocks)
            Assert.NotEmpty(sensorDataSet?.TemperatureSeries.Trimmed());
            Assert.NotEmpty(sensorDataSet?.HumiditySeries.Trimmed());
        }
    }
}