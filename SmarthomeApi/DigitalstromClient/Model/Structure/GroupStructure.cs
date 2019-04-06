using DigitalstromClient.Model.Core;
using System.Collections.Generic;

namespace DigitalstromClient.Model.Structure
{
    public class GroupStructure
    {
        public int id { get; set; }
        public string name { get; set; }
        public GroupName groupName { get { return name; } }
        public int color { get; set; }
        public Group group { get { return color; } }
        public int applicationType { get; set; }
        public bool isPresent { get; set; }
        public bool isValid { get; set; }
        public List<string> devices { get; set; }
        public List<int?> activeBasicScenes { get; set; }
    }
}
