using System;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace PhilipDaubmeier.WeConnectClient.Model.Core
{
    /// <summary>
    /// Class representing a Vehicle Identification Number (VIN), that is a wrapper
    /// around a 17-character string under the that verifies and normalizes VINs,
    /// makes them comparable and provides seamless implicit conversion from and to string.
    /// </summary>
    /// <remarks>
    /// More information on Vehicle Identification Numbers:
    /// https://en.wikipedia.org/wiki/Vehicle_identification_number
    /// </remarks>
    [DebuggerDisplay("{ToString(),nq}")]
    public class Vin : IComparable, IComparable<Vin>, IEquatable<Vin>
    {
        private readonly string _vin = string.Empty;

        private static readonly Regex vinPattern = new Regex(@"^[A-HJ-NPR-Z\d]{9}[A-HJ-NPR-Z\d-]{2}[\d]{6}$", RegexOptions.Compiled);

        public static Vin Empty => new Vin("S0000000000000000");

        public Vin(string vin)
        {
            _vin = vin.Trim().ToUpperInvariant();
            if (!vinPattern.IsMatch(_vin))
                throw new ArgumentException($"Not a valid VIN: \"{vin}\"");
        }

        public static implicit operator Vin(string vin)
        {
            return new Vin(vin);
        }

        public static implicit operator string(Vin vin)
        {
            return vin._vin;
        }

        public int CompareTo(Vin? value)
        {
            return ((string)this).CompareTo(value ?? string.Empty);
        }

        public int CompareTo(object? value)
        {
            return ((string)this).CompareTo((value as Vin) ?? value);
        }

        public override bool Equals(object? obj)
        {
            if (!(obj is Vin value))
                return false;
            return this == value;
        }

        public bool Equals(Vin? g)
        {
            return this == g;
        }

        public override int GetHashCode()
        {
            return ((string)this).GetHashCode();
        }

        public static bool operator ==(Vin? a, Vin? b)
        {
            return a?._vin == b?._vin;
        }

        public static bool operator !=(Vin? a, Vin? b)
        {
            return !(a == b);
        }

        public override string ToString()
        {
            return this;
        }
    }
}