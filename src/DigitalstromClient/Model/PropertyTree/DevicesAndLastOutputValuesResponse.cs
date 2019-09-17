using PhilipDaubmeier.DigitalstromClient.Model.Core;
using System;
using System.Collections.Generic;

namespace PhilipDaubmeier.DigitalstromClient.Model.PropertyTree
{
    public class DevicesAndLastOutputValuesResponse : IWiremessagePayload
    {
        public List<ZonesAndLastOutputValues> Zones { get; set; }
    }

    public class ZonesAndLastOutputValues
    {
        public Zone ZoneID { get; set; }
        public List<DeviceAndLastOutputValues> Devices { get; set; }
    }

    public class DeviceAndLastOutputValues
    {
        public string DsId { get; set; }
        public List<LastOutputValuesStatus> Status { get; set; }
    }

    public class LastOutputValuesStatus
    {
        public string LastChanged { get; set; }
        public DateTime LastChangedTime
        {
            get
            {
                if (DateTime.TryParse(LastChanged.Replace("\\", "").Replace("\"", ""), out DateTime result))
                    return result;
                return DateTime.MinValue;
            }
        }

        public List<LastOutputValueInfo> Outputs { get; set; }
    }

    public class LastOutputValueInfo
    {
        public double Value { get; set; }
        public double TargetValue { get; set; }
    }
}