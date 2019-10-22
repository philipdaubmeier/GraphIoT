using PhilipDaubmeier.DigitalstromClient.Model.Core;
using System;
using System.Collections.Generic;

namespace PhilipDaubmeier.DigitalstromClient.Model.PropertyTree
{
    public class DevicesAndLastOutputValuesResponse : IWiremessagePayload
    {
        public List<ZonesAndLastOutputValues> Zones { get; set; } = new List<ZonesAndLastOutputValues>();
    }

    public class ZonesAndLastOutputValues
    {
        public Zone ZoneID { get; set; } = 0;
        public List<DeviceAndLastOutputValues> Devices { get; set; } = new List<DeviceAndLastOutputValues>();
    }

    public class DeviceAndLastOutputValues
    {
        public string DsId { get; set; } = string.Empty;
        public List<LastOutputValuesStatus> Status { get; set; } = new List<LastOutputValuesStatus>();
    }

    public class LastOutputValuesStatus
    {
        public string LastChanged { get; set; } = string.Empty;
        public DateTime LastChangedTime
        {
            get
            {
                if (DateTime.TryParse(LastChanged.Replace("\\", "").Replace("\"", ""), out DateTime result))
                    return result;
                return DateTime.MinValue;
            }
        }

        public List<LastOutputValueInfo> Outputs { get; set; } = new List<LastOutputValueInfo>();
    }

    public class LastOutputValueInfo
    {
        public double Value { get; set; }
        public double TargetValue { get; set; }
    }
}