using System;

namespace PhilipDaubmeier.DigitalstromClient.Model.Core
{
    public class Zone : IComparable, IComparable<Zone>, IEquatable<Zone>
    {
        private readonly int _zone = 0;

        private Zone(int zone)
        {
            _zone = Math.Min(Math.Max(zone, 0), ushort.MaxValue);
        }

        public static implicit operator int(Zone zone)
        {
            return zone._zone;
        }

        public static implicit operator Zone(long zone)
        {
            return (int)zone;
        }

        public static implicit operator Zone(int zone)
        {
            return new Zone(zone);
        }

        public static implicit operator Zone(string zoneID)
        {
            if (!int.TryParse(zoneID, out int zone))
                return new Zone(0);

            return new Zone(zone);
        }

        public static bool operator !=(Zone zone1, Zone zone2)
        {
            return !(zone1 == zone2);
        }

        public static bool operator ==(Zone zone1, Zone zone2)
        {
            if (zone1 is null || zone2 is null)
                return ReferenceEquals(zone1, zone2);
            return zone1._zone == zone2._zone;
        }

        public int CompareTo(Zone value)
        {
            return _zone.CompareTo(value._zone);
        }

        public int CompareTo(object value)
        {
            return _zone.CompareTo((value as Zone)?._zone ?? value);
        }

        public bool Equals(Zone zone)
        {
            return this == zone;
        }

        public override bool Equals(object obj)
        {
            return this == (obj as Zone);
        }

        public override int GetHashCode()
        {
            return _zone.GetHashCode();
        }

        public override string ToString()
        {
            return string.Format("Zone ID {0}", _zone);
        }
    }
}