using PhilipDaubmeier.CompactTimeSeries;
using System;

namespace PhilipDaubmeier.DigitalstromHost.Database
{
    public interface IDigitalstromEnergyMidLowresData
    {
        string CircuitId { get; set; }

        DigitalstromCircuit Circuit { get; set; }

        DateTime Key { get; set; }

        TimeSeries<int> EnergySeries { get; set; }
    }
}