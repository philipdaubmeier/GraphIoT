using PhilipDaubmeier.DigitalstromClient.Model.Core;
using System.Collections.Generic;

namespace PhilipDaubmeier.DigitalstromClient.Model.PropertyTree
{
    public class ZonesAndLastCalledScenesResponse : IWiremessagePayload<ZonesAndLastCalledScenesResponse>
    {
        public List<ZoneAndLastCalledScenes> Zones { get; set; }
    }

    public class ZoneAndLastCalledScenes
    {
        public Zone ZoneID { get; set; }
        public List<GroupAndLastCalledScenes> Groups { get; set; }
    }

    public class GroupAndLastCalledScenes
    {
        public Group Group { get; set; }
        public Scene LastCalledScene { get; set; }
    }
}