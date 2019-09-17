using System;
using System.Collections.Generic;
using System.Linq;
using NodaTime;

namespace PhilipDaubmeier.DigitalstromClient.Model.Energy
{
    public class EnergyMeteringResponse : IWiremessagePayload
    {
        public string Type { get; set; }
        public string Unit { get; set; }
        public int Resolution { get; set; }
        public List<List<double>> Values { get; set; }

        public IEnumerable<KeyValuePair<DateTime, double>> TimeSeries => Values.Select(x => 
                new KeyValuePair<DateTime, double>(Instant.FromUnixTimeSeconds((long)x.FirstOrDefault()).ToDateTimeUtc(), x.LastOrDefault()));
    }
}