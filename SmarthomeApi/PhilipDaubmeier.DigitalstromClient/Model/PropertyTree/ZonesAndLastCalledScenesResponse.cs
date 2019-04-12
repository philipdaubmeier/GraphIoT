using System.Collections.Generic;

namespace PhilipDaubmeier.DigitalstromClient.Model.PropertyTree
{
    public class ZonesAndLastCalledScenesResponse : IWiremessagePayload<ZonesAndLastCalledScenesResponse>
    {
        public List<ZoneAndLastCalledScenes> zones { get; set; }
    }

    public class ZoneAndLastCalledScenes
    {
        public int ZoneID { get; set; }
        public List<GroupAndLastCalledScenes> groups { get; set; }
    }

    public class GroupAndLastCalledScenes
    {
        public int group { get; set; }
        public int lastCalledScene { get; set; }
    }
}