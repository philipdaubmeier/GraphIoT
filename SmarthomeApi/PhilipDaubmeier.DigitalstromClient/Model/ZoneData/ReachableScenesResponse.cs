using PhilipDaubmeier.DigitalstromClient.Model.Core;
using System.Collections.Generic;

namespace PhilipDaubmeier.DigitalstromClient.Model.ZoneData
{
    public class ReachableScenesResponse : IWiremessagePayload
    {
        public List<Scene> ReachableScenes { get; set; }
        public List<string> UserSceneNames { get; set; }
    }
}