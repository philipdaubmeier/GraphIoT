using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace PhilipDaubmeier.NetatmoClient.Model.Core
{
    /// <summary>
    /// Netatmo device and module id that is derived from their MAC adresses, i.e. 6 byte long
    /// and formatted in single byte hex blocks separated by ':', e.g. "70:ee:50:09:f0:xx"
    /// </summary>
    [DebuggerDisplay("{ToString(),nq}")]
    public class ModuleId : IComparable, IComparable<ModuleId>, IEquatable<ModuleId>
    {
        public static int Size => 6;

        private readonly byte[] _bytes = new byte[Size];

        public ModuleId(string hex)
        {
            hex = new string(hex.ToLowerInvariant().ToCharArray().Where(c => IsHexChar(c)).ToArray());
            hex = hex.Substring(0, Math.Min(hex.Length, Size * 2)).PadLeft(Size * 2, '0');
            for (int i = 0; i < Math.Min(_bytes.Length, hex.Length >> 1); ++i)
                _bytes[i] = (byte)((GetHexVal(hex[i << 1]) << 4) + GetHexVal(hex[(i << 1) + 1]));
        }

        public static ModuleId ReadFrom(Stream stream)
        {
            using var reader = new BinaryReader(stream, Encoding.UTF8, true);
            var array = reader.ReadBytes(Size);
            return new ModuleId(BitConverter.ToString(array));
        }

        public void WriteTo(Stream stream)
        {
            for (int i = 0; i < _bytes.Length; ++i)
                stream.WriteByte(_bytes[i]);
        }

        private static int GetHexVal(char hex)
        {
            return hex - (hex < 58 ? 48 : 87);
        }

        private static bool IsHexChar(char c)
        {
            return (c >= 48 && c <= 57) || (c >= 97 && c <= 102);
        }

        public static implicit operator ModuleId(string hex)
        {
            return new ModuleId(hex);
        }

        public static implicit operator string(ModuleId dsuid)
        {
            return BitConverter.ToString(dsuid._bytes).ToLowerInvariant().Replace('-', ':');
        }

        public int CompareTo(ModuleId? value)
        {
            return ((string)this).CompareTo(value ?? string.Empty);
        }

        public int CompareTo(object? value)
        {
            return ((string)this).CompareTo((value as ModuleId) ?? value);
        }

        public override bool Equals(object? obj)
        {
            return obj is ModuleId value && this == value;
        }

        public bool Equals(ModuleId? g)
        {
            return this == g;
        }

        public override int GetHashCode()
        {
            return ((string)this).GetHashCode();
        }

        public static bool operator ==(ModuleId? a, ModuleId? b)
        {
            if (a is null || b is null)
                return ReferenceEquals(a, b);

            if (a._bytes.Length != b._bytes.Length)
                return false;

            for (int i = 0; i < a._bytes.Length; i++)
                if (a._bytes[i] != b._bytes[i])
                    return false;

            return true;
        }

        public static bool operator !=(ModuleId? a, ModuleId? b)
        {
            return !(a == b);
        }

        public override string ToString()
        {
            return this;
        }
    }
}