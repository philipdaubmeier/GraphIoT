using System;
using System.Collections.Generic;
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

    public class Group : IComparable, IComparable<Group>, IEquatable<Group>
    {
        private readonly int _group = (int)Color.Yellow;

        private Group(int colorCode)
        {
            _group = Math.Min(Math.Max(colorCode, 0), 64);
            if (_group > 12 && !(_group == 48 || _group == 64))
                _group = 0;
        }

        public static implicit operator int(Group group)
        {
            return group._group;
        }

        public static implicit operator Group(long group)
        {
            return (int)group;
        }

        public static implicit operator Group(int group)
        {
            return new Group(group);
        }

        public static implicit operator Group(string groupID)
        {
            if (!int.TryParse(groupID, out int color))
                return new Group((int)Color.White);

            return new Group(color);
        }

        public static implicit operator Group(Color color)
        {
            return new Group((int)color);
        }

        public static implicit operator Group(GroupType type)
        {
            return new Group((int)type);
        }

        public static implicit operator Color(Group group)
        {
            if (group._group <= 0)
                return Color.White;

            if ((group._group >= 9 && group._group <= 12) || group._group == 48 || group._group == 64)
                return Color.Blue;

            if (group._group > 8)
                return Color.White;

            return (Color)group._group;
        }

        public static implicit operator GroupType(Group group)
        {
            if (group._group == 48 || group._group == 64)
                return (GroupType)group._group;
            if (group._group <= 0 || group._group > 12)
                return GroupType.Various;

            return (GroupType)group._group;
        }

        public static IEnumerable<Group> GetGroups()
        {
            return ((Color[])Enum.GetValues(typeof(Color))).Select(x => (Group)x);
        }

        public static bool operator !=(Group group1, Group group2)
        {
            return !(group1 == group2);
        }

        public static bool operator ==(Group group1, Group group2)
        {
            if (group1 is null || group2 is null)
                return ReferenceEquals(group1, group2);
            return group1._group == group2._group;
        }

        public int CompareTo(Group value)
        {
            return _group.CompareTo(value._group);
        }

        public int CompareTo(object value)
        {
            return _group.CompareTo((value as Group)?._group ?? value);
        }

        public bool Equals(Group group)
        {
            return this == group;
        }

        public override bool Equals(object obj)
        {
            return this == (obj as Group);
        }

        public override int GetHashCode()
        {
            return _group.GetHashCode();
        }

        public override string ToString()
        {
            return $"ID {_group}: {((Color)this).ToString()} - {((GroupType)this).ToString()}";
        }

        public string ToDisplayString()
        {
            switch (_group)
            {
                case (int)GroupType.Various: return "Weiss - Gerät";
                case (int)GroupType.Light: return "Gelb - Licht";
                case (int)GroupType.Shading: return "Grau - Beschattung";
                case (int)GroupType.Heating: return "Blau - Heizung";
                case (int)GroupType.Cooling: return "Blau - Kühlung";
                case (int)GroupType.Ventilation: return "Blau - Lüftung";
                case (int)GroupType.Window: return "Blau - Fenster";
                case (int)GroupType.AirRecirculation: return "Blau - Luftzirkulation";
                case (int)GroupType.ApartmentVentilation: return "Blau - Lüftungssystem";
                case (int)GroupType.TemperatureControl: return "Blau - Einzelraumregelung";
                case (int)GroupType.Audio: return "Cyan - Audio";
                case (int)GroupType.Video: return "Magenta - Video";
                case (int)GroupType.Security: return "Rot - Sicherheit";
                case (int)GroupType.Access: return "Grün - Zugang";
                case (int)GroupType.Joker: return "Schwarz - Joker";
                default: return string.Format("Unbekannt ({0})", (int)_group);
            }
        }
    }
}