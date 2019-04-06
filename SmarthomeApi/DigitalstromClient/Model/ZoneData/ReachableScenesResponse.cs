using System.Collections.Generic;

namespace DigitalstromClient.Model.ZoneData
{
    public class ReachableScenesResponse : IWiremessagePayload<ReachableScenesResponse>
    {
        public List<int> reachableScenes { get; set; }
    }
}