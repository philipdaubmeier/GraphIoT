using System.Collections.Generic;
using System.Linq;

namespace PhilipDaubmeier.DigitalstromClient.Model.Core
{
    public record GroupName(GroupName.Name NameValue)
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

        private static readonly Dictionary<string, Name> _mapping = new()
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

        public Name NameValue { get; init; } = NameValue;

        public static implicit operator string(GroupName name)
            => _mapping.ToDictionary(x => x.Value, x => x.Key)[name];

        public static implicit operator GroupName(string name)
            => new(_mapping.TryGetValue(name, out Name enumName) ? enumName : Name.Broadcast);

        public static implicit operator Name(GroupName name) => name.NameValue;

        public static implicit operator GroupName(Name name) => new(name);
    }
}