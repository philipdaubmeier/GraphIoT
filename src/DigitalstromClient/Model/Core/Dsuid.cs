using System;
using System.IO;
using System.Linq;
using System.Text;

namespace PhilipDaubmeier.DigitalstromClient.Model.Core
{
    public record Dsuid(string Hex) : IComparable, IComparable<Dsuid>
    {
        public string Hex { get; init; } = NormalizeDsuid(Hex);

        public static int Size => 17;

        public static Dsuid ReadFrom(Stream stream)
        {
            using var reader = new BinaryReader(stream, Encoding.UTF8, true);
            return new Dsuid(BitConverter.ToString(reader.ReadBytes(Size)).Replace("-", ""));
        }

        public void WriteTo(Stream stream)
        {
            for (int i = 0; i < Hex.Length >> 1; ++i)
                stream.WriteByte((byte)((GetHexVal(Hex[i << 1]) << 4) + GetHexVal(Hex[(i << 1) + 1])));
        }

        private static string NormalizeDsuid(string input)
        {
            input = new string(input.ToLowerInvariant().ToCharArray().Where(c => IsHexChar(c)).ToArray());
            return input.Substring(0, Math.Min(input.Length, Size * 2)).PadLeft(Size * 2, '0');
        }

        private static int GetHexVal(char hex) => hex - (hex < 58 ? 48 : 87);

        private static bool IsHexChar(char c) => (c >= 48 && c <= 57) || (c >= 97 && c <= 102);

        public static implicit operator Dsuid(string hex) => new(hex);

        public static implicit operator string(Dsuid dsuid) => dsuid.Hex;

        public int CompareTo(Dsuid? value) => Hex.CompareTo(value?.Hex);

        public int CompareTo(object? value) => Hex.CompareTo((value as Dsuid)?.Hex ?? value);

        public override string ToString() => this;
    }
}