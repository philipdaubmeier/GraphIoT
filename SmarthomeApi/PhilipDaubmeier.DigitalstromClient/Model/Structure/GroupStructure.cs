using PhilipDaubmeier.DigitalstromClient.Model.Core;
using System.Collections.Generic;

namespace PhilipDaubmeier.DigitalstromClient.Model.Structure
{
    public class GroupStructure
    {
        public Group Id { get; set; }
        public GroupName Name { get; set; }
        public Group Color { get; set; }
        public int ApplicationType { get; set; }
        public bool IsPresent { get; set; }
        public bool IsValid { get; set; }
        public List<DSUID> Devices { get; set; }
        public List<Scene> ActiveBasicScenes { get; set; }
    }
}