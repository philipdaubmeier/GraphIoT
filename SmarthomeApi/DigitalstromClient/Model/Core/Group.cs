using System;
using System.Collections.Generic;
using System.Linq;

namespace DigitalstromClient.Model.Core
{
    public class Group
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
        public enum Type
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

        private int _group = (int)Color.Yellow;

        private Group(int colorCode)
        {
            _group = Math.Min(Math.Max(colorCode, 0), 128);
        }

        public static implicit operator int(Group group)
        {
            return group._group;
        }

        public static implicit operator Group(int group)
        {
            return new Group(group);
        }

        public static implicit operator Group(string groupID)
        {
            int color;
            if (!int.TryParse(groupID, out color))
                return new Group((int)Color.Gray);

            return new Group(color);
        }

        public static implicit operator Group(Color color)
        {
            return new Group((int)color);
        }

        public static implicit operator Group(Type type)
        {
            return new Group((int)type);
        }

        public static implicit operator Color(Group group)
        {
            if (group._group <= 0 || group._group > 8)
                return Color.White;

            return (Color)group._group;
        }

        public static implicit operator Type(Group group)
        {
            if (group._group == 48 || group._group == 64)
                return (Type)group._group;
            if (group._group <= 0 || group._group > 12)
                return Type.Various;

            return (Type)group._group;
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
            if ((object)group1 == null || (object)group2 == null)
                return ReferenceEquals(group1, group2);
            return group1._group == group2._group;
        }

        public override bool Equals(object obj)
        {
            return ((Group)obj)._group == _group;
        }

        public override int GetHashCode()
        {
            return _group.GetHashCode();
        }

        public override string ToString()
        {
            switch (_group)
            {
                case 0: return "ID 0: White - Various";
                case 1: return "ID 1: Yellow - Light";
                case 2: return "ID 2: Grey - Shading";
                case 3: return "ID 3: Blue - Heating";
                case 4: return "ID 4: Cyan - Audio";
                case 5: return "ID 5: Magenta - Video";
                case 6: return "ID 6: Red - Security";
                case 7: return "ID 7: Green - Access";
                case 8: return "ID 8: Black - Joker";
                case 9: return "ID 9: Blue - Cooling";
                case 10: return "ID 10: Blue - Ventilation";
                case 11: return "ID 11: Blue - Window";
                case 12: return "ID 12: Blue - AirRecirculation";
                case 48: return "ID 48: Blue - TemperatureControl";
                case 64: return "ID 64: Blue - Ventilation";
                default: return string.Format("ID {0}: Unknown", _group);
            }
        }
    }
}
