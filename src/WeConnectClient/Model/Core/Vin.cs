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
    public record Vin(string Number) : IComparable, IComparable<Vin>, IEquatable<Vin>
    {
        private static readonly Regex vinPattern = new(@"^[A-HJ-NPR-Z\d]{9}[A-HJ-NPR-Z\d-]{2}[\d]{6}$", RegexOptions.Compiled);

        public static Vin Empty => new("S0000000000000000");

        public string Number { get; init; } = NormalizeVin(Number);

        private static string NormalizeVin(string vin)
        {
            vin = vin.Trim().ToUpperInvariant();
            if (!vinPattern.IsMatch(vin))
                throw new ArgumentException($"Not a valid VIN: \"{vin}\"");
            return vin;
        }

        public static implicit operator Vin(string vin) => new(vin);

        public static implicit operator string(Vin vin) => vin.Number;

        public int CompareTo(Vin? value) => ((string)this).CompareTo(value ?? string.Empty);

        public int CompareTo(object? value) => ((string)this).CompareTo((value as Vin) ?? value);

        public override string ToString() => this;
    }
}