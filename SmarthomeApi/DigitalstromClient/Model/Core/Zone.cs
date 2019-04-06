using System;

namespace DigitalstromClient.Model.Core
{
    public class Zone
    {
        private int _zone = 0;

        private Zone(int zone)
        {
            _zone = Math.Min(Math.Max(zone, 0), ushort.MaxValue);
        }

        public static implicit operator int(Zone zone)
        {
            return zone._zone;
        }

        public static implicit operator Zone(int zone)
        {
            return new Zone(zone);
        }

        public static implicit operator Zone(string zoneID)
        {
            int zone;
            if (!int.TryParse(zoneID, out zone))
                return new Zone(0);

            return new Zone(zone);
        }

        public static bool operator !=(Zone zone1, Zone zone2)
        {
            return !(zone1 == zone2);
        }

        public static bool operator ==(Zone zone1, Zone zone2)
        {
            if ((object)zone1 == null || (object)zone2 == null)
                return ReferenceEquals(zone1, zone2);
            return zone1._zone == zone2._zone;
        }

        public override bool Equals(object obj)
        {
            return ((Zone)obj)._zone == _zone;
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