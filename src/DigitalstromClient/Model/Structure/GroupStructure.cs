using PhilipDaubmeier.DigitalstromClient.Model.Core;
using System.Collections.Generic;

namespace PhilipDaubmeier.DigitalstromClient.Model.Structure
{
    public class GroupStructure
    {
        public Group Id { get; set; } = Core.Color.White;
        public GroupName Name { get; set; } = GroupName.Name.Broadcast;
        public Group Color { get; set; } = Core.Color.White;
        public int ApplicationType { get; set; }
        public bool IsPresent { get; set; }
        public bool IsValid { get; set; }
        public List<Dsuid> Devices { get; set; } = new List<Dsuid>();
        public List<Scene> ActiveBasicScenes { get; set; } = new List<Scene>();
    }
}