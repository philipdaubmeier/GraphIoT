using PhilipDaubmeier.DigitalstromClient.Model.Core;
using System.Collections.Generic;

namespace PhilipDaubmeier.DigitalstromClient.Model.PropertyTree
{
    public class ZonesAndLastCalledScenesResponse : IWiremessagePayload
    {
        public List<ZoneAndLastCalledScenes> Zones { get; set; } = new List<ZoneAndLastCalledScenes>();
    }

    public class ZoneAndLastCalledScenes
    {
        public Zone ZoneID { get; set; } = 0;
        public List<GroupAndLastCalledScenes> Groups { get; set; } = new List<GroupAndLastCalledScenes>();
    }

    public class GroupAndLastCalledScenes
    {
        public Group Group { get; set; } = Color.White;
        public Scene LastCalledScene { get; set; } = SceneCommand.Unknown;
    }
}