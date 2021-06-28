using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace PhilipDaubmeier.DigitalstromClient.Model.Core
{
    /// <summary>
    /// All colors used in digitalstrom.
    /// </summary>
    /// <remarks>
    /// Each group type has one of these colors assigned to them, but not each color has
    /// exactly one associated group type, especially not one single assigned group id!
    /// Blue is used for Heating (ID: 3), Cooling (ID: 9), Ventilation (ID: 10) and some more.
    /// Red and Green are not guaranteed to be only assigned to IDs 6 and 7, respectively.
    /// White is used as unknown type or single device, usually - but not restricted to - ID 0.
    /// </remarks>
    public enum Color
    {
        White = 0,
        Yellow = 1,
        Gray = 2,
        Blue = 3,
        Cyan = 4,
        Magenta = 5,
        Red = 6,
        Green = 7,
        Black = 8
    }

    /// <summary>
    /// All group types usable in digitalstrom.
    /// </summary>
    public enum GroupType
    {
        ///<summary>[Color: White] Single Device White Various, individual per device</summary>
        Various = 0,
        ///<summary>[Color: Yellow] Room lights, Garden lights, Building illuminations</summary>
        Light = 1,
        ///<summary>[Color: Gray] Blinds, Shades, Awnings, Curtains Gray Blinds</summary>
        Shading = 2,
        ///<summary>[Color: Blue] Heating actuators</summary>
        Heating = 3,
        ///<summary>[Color: Blue] Cooling actuators</summary>
        Cooling = 9,
        ///<summary>[Color: Blue] Ventilation actuators</summary>
        Ventilation = 10,
        ///<summary>[Color: Blue] Windows actuators for opening and closing</summary>
        Window = 11,
        ///<summary>[Color: Blue] Ceiling fan, Fan coil units</summary>
        AirRecirculation = 12,
        ///<summary>[Color: Blue] Ventilation system</summary>
        ApartmentVentilation = 64,
        ///<summary>[Color: Blue] Blue Single room temperature control</summary>
        TemperatureControl = 48,
        ///<summary>[Color: Cyan] Playing music or radio</summary>
        Audio = 4,
        ///<summary>[Color: Magenta] TV, Video</summary>
        Video = 5,
        ///<summary>[Color: Red] Security related functions, Alarms</summary>
        Security = 6,
        ///<summary>[Color: Green] Access related functions, door bell</summary>
        Access = 7,
        ///<summary>[Color: Black] Configurable</summary>
        Joker = 8
    }

    public record Group(int Type) : IComparable, IComparable<Group>, IFormattable
    {
        public int Type { get; init; } = NormalizeGroupType(Type);

        private static int NormalizeGroupType(int groupType)
        {
            groupType = Math.Min(Math.Max(groupType, 0), 64);
            if (groupType > 12 && !(groupType == 48 || groupType == 64))
                return 0;
            return groupType;
        }

        public static implicit operator int(Group group) => group.Type;

        public static implicit operator Group(string groupID)
        {
            if (!int.TryParse(groupID, out int color))
                return new Group((int)Color.White);

            return new Group(color);
        }

        public static implicit operator Group(long group) => (int)group;

        public static implicit operator Group(int group) => new(group);

        public static implicit operator Group(Color color) => new((int)color);

        public static implicit operator Group(GroupType type) => new((int)type);

        public static implicit operator Color(Group group)
        {
            return group.Type switch
            {
                0 => Color.White,
                8 => Color.Black,
                var x when x >= 9 && x <= 12 => Color.Blue,
                48 => Color.Blue,
                64 => Color.Blue,
                _ => (Color)group.Type
            };
        }

        public static implicit operator GroupType(Group group)
        {
            if (group.Type == 48 || group.Type == 64)
                return (GroupType)group.Type;
            if (group.Type <= 0 || group.Type > 12)
                return GroupType.Various;

            return (GroupType)group.Type;
        }

        public int CompareTo(Group? value) => Type.CompareTo(value?.Type);

        public int CompareTo(object? value) => Type.CompareTo((value as Group)?.Type ?? value);

        public static IEnumerable<Group> GetGroups()
        {
            return ((Color[])Enum.GetValues(typeof(Color))).Select(x => (Group)x);
        }

        public override string ToString() => ToString(null, null);

        /// <summary>
        /// Converts the numeric value of this instance to its equivalent string representation
        /// using the specified format and culture-specific format information.
        /// </summary>
        /// <param name="format">
        /// Null for an invariant default format 'ID {group-number}: {color} - {group-name}'.
        /// "D" or "d" for a localized displayable string of the group name,
        /// if available for the given language of the format provider.
        /// </param>
        /// <param name="formatProvider">An object that supplies culture-specific formatting information.</param>
        /// <returns>
        /// The string representation of the value of this instance as specified by format and provider.
        /// </returns>
        public string ToString(string? format = null, IFormatProvider? formatProvider = null)
        {
            if (format is null)
                return $"ID {Type}: {(Color)this} - {(GroupType)this}";

            if (!format.Equals("d", StringComparison.InvariantCultureIgnoreCase))
                throw new FormatException($"Did not recognize format '{format}'");

            if (formatProvider is CultureInfo culture)
                Locale.Group.Culture = culture;

            return Type switch
            {
                (int)GroupType.Various => Locale.Group.Device,
                (int)GroupType.Light => Locale.Group.Light,
                (int)GroupType.Shading => Locale.Group.Shade,
                (int)GroupType.Heating => Locale.Group.Heating,
                (int)GroupType.Cooling => Locale.Group.Cooling,
                (int)GroupType.Ventilation => Locale.Group.Ventilation,
                (int)GroupType.Window => Locale.Group.Window,
                (int)GroupType.AirRecirculation => Locale.Group.AirRecirculation,
                (int)GroupType.ApartmentVentilation => Locale.Group.ApartmentVentilation,
                (int)GroupType.TemperatureControl => Locale.Group.TemperatureControl,
                (int)GroupType.Audio => Locale.Group.Audio,
                (int)GroupType.Video => Locale.Group.Video,
                (int)GroupType.Security => Locale.Group.Security,
                (int)GroupType.Access => Locale.Group.Access,
                (int)GroupType.Joker => Locale.Group.Joker,
                _ => $"{Locale.Group.Unknown} ({Type})",
            };
        }
    }
}