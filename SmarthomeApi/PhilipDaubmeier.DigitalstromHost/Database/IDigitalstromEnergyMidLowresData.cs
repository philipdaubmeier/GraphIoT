using PhilipDaubmeier.CompactTimeSeries;
using PhilipDaubmeier.TimeseriesHostCommon.Database;

namespace PhilipDaubmeier.DigitalstromHost.Database
{
    public interface IDigitalstromEnergyMidLowresData : ITimeSeriesDbEntity
    {
        string CircuitId { get; set; }

        DigitalstromCircuit Circuit { get; set; }

        TimeSeries<int> EnergySeries { get; }
    }
}