using NodaTime;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PhilipDaubmeier.DigitalstromClient.Model.Energy
{
    public class EnergyMeteringResponse : IWiremessagePayload
    {
        public string Type { get; set; } = string.Empty;
        public string Unit { get; set; } = string.Empty;
        public int Resolution { get; set; }
        public List<List<double>> Values { get; set; } = new List<List<double>>();

        public IEnumerable<KeyValuePair<DateTime, double>> TimeSeries => Values.Select(x =>
                new KeyValuePair<DateTime, double>(Instant.FromUnixTimeSeconds((long)x.FirstOrDefault()).ToDateTimeUtc(), x.LastOrDefault()));
    }
}