using System.Linq;
using System.Collections.Generic;

namespace DigitalstromClient.Model.Core
{
    public class GroupName
    {
        public enum Name
        {
            Broadcast,
            Yellow,
            Gray,
            Heating,
            Cooling,
            Ventilation,
            Window,
            Recirculation,
            Controltemperature,
            Cyan,
            Magenta,
            Black,
            Reserved1,
            Reserved2
        }

        private static Dictionary<string, Name> _mapping = new Dictionary<string, Name>()
        {
            {"broadcast", Name.Broadcast },
            {"yellow", Name.Yellow },
            {"gray", Name.Gray },
            {"heating", Name.Heating },
            {"cooling", Name.Cooling },
            {"ventilation", Name.Ventilation },
            {"window", Name.Window },
            {"recirculation", Name.Recirculation },
            {"controltemperature", Name.Controltemperature},
            {"cyan", Name.Cyan },
            {"magenta", Name.Magenta },
            {"black", Name.Black },
            {"reserved1", Name.Reserved1 },
            {"reserved2", Name.Reserved2 }
        };

        private Name _name = Name.Broadcast;

        private GroupName(string name)
        {
            _mapping.TryGetValue(name, out _name);
        }

        public static implicit operator string(GroupName name)
        {
            return _mapping.ToDictionary(x => x.Value, x => x.Key)[name];
        }

        public static implicit operator GroupName(string name)
        {
            return new GroupName(name);
        }

        public static implicit operator Name(GroupName name)
        {
            return name._name;
        }
    }
}
