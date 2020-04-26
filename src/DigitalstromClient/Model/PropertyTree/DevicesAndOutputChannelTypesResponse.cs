using PhilipDaubmeier.DigitalstromClient.Model.Core;
using System.Collections.Generic;

namespace PhilipDaubmeier.DigitalstromClient.Model.PropertyTree
{
    public class DevicesAndOutputChannelTypesResponse : IWiremessagePayload
    {
        public List<ZonesAndOutputChannelTypes> Zones { get; set; } = new List<ZonesAndOutputChannelTypes>();
    }

    public class ZonesAndOutputChannelTypes
    {
        public Zone ZoneID { get; set; } = 0;
        public List<DeviceAndOutputChannelTypes> Devices { get; set; } = new List<DeviceAndOutputChannelTypes>();
    }

    public class DeviceAndOutputChannelTypes
    {
        public string DsId { get; set; } = string.Empty;
        public List<OutputChannelTypeInfo> OutputChannels { get; set; } = new List<OutputChannelTypeInfo>();
    }

    public class OutputChannelTypeInfo
    {
        public string Id { get; set; } = string.Empty;
    }
}