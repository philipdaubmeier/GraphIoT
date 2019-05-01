using System.Collections.Generic;

namespace PhilipDaubmeier.DigitalstromClient.Model.ZoneData
{
    public class ReachableScenesResponse : IWiremessagePayload<ReachableScenesResponse>
    {
        public List<int> reachableScenes { get; set; }
        public List<string> userSceneNames { get; set; }
    }
}