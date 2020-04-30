using System;
using System.Collections.Generic;
using System.Globalization;

namespace PhilipDaubmeier.WeConnectClient.Model.VehicleInfo
{
    public class VehicleDetails
    {
        public List<string> LastConnectionTimeStamp { get; set; } = new List<string>();
        public string DistanceCovered { get; set; } = string.Empty;
        public string Range { get; set; } = string.Empty;
        public string ServiceInspectionData { get; set; } = string.Empty;
        public string OilInspectionData { get; set; } = string.Empty;
        public bool ShowOil { get; set; }
        public bool ShowService { get; set; }
        public bool FlightMode { get; set; }

        public DateTime LastConnection
        {
            get
            {
                if (LastConnectionTimeStamp.Count < 2)
                    return DateTime.MinValue;

                if (!DateTime.TryParseExact(LastConnectionTimeStamp[0], "dd'-'MM'-'yyyy", CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out DateTime datePart))
                    return DateTime.MinValue;

                if (!DateTime.TryParseExact(LastConnectionTimeStamp[1], "HH':'mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime timePart))
                    return DateTime.MinValue;

                return (datePart + new TimeSpan(timePart.Hour, timePart.Minute, 0)).ToUniversalTime();
            }
        }
    }
}