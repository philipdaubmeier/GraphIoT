using PhilipDaubmeier.DigitalstromClient.Model.Core;
using System.Collections.Generic;

namespace PhilipDaubmeier.DigitalstromClient.Model.PropertyTree
{
    public class DevicesAndOutputChannelTypesResponse : IWiremessagePayload
    {
        public List<ZonesAndOutputChannelTypes> Zones { get; set; }
    }

    public class ZonesAndOutputChannelTypes
    {
        public Zone ZoneID { get; set; }
        public List<DeviceAndOutputChannelTypes> Devices { get; set; }
    }

    public class DeviceAndOutputChannelTypes
    {
        public string DsId { get; set; }
        public List<OutputChannelTypeInfo> OutputChannels { get; set; }
    }

    public class OutputChannelTypeInfo
    {
        public string Id { get; set; }
    }
}