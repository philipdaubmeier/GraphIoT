using System;

namespace PhilipDaubmeier.DigitalstromClient.Model.Core
{
    public record Zone(int Id) : IComparable, IComparable<Zone>, IEquatable<Zone>
    {
        public int Id { get; init; } = Math.Min(Math.Max(Id, 0), ushort.MaxValue);

        public static implicit operator int(Zone zone) => zone.Id;

        public static implicit operator Zone(long zone) => (int)zone;

        public static implicit operator Zone(int zone) => new(zone);

        public static implicit operator Zone(string zoneID)
        {
            if (!int.TryParse(zoneID, out int zone))
                return new Zone(0);

            return new Zone(zone);
        }

        public int CompareTo(Zone? value) => Id.CompareTo(value?.Id);

        public int CompareTo(object? value) => Id.CompareTo((value as Zone)?.Id ?? value);
    }
}