using System.Collections.Generic;
using System.Linq;

namespace PhilipDaubmeier.DigitalstromClient.Model.Core
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

        private static readonly Dictionary<string, Name> _mapping = new Dictionary<string, Name>()
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

        private readonly Name _name = Name.Broadcast;

        private GroupName(Name name)
        {
            _name = name;
        }

        public static implicit operator string(GroupName name)
        {
            return _mapping.ToDictionary(x => x.Value, x => x.Key)[name];
        }

        public static implicit operator GroupName(string name)
        {
            if (!_mapping.TryGetValue(name, out Name enumName))
                return new GroupName(Name.Broadcast);

            return new GroupName(enumName);
        }

        public static implicit operator Name(GroupName name)
        {
            return name._name;
        }

        public static implicit operator GroupName(Name name)
        {
            return new GroupName(name);
        }
    }
}