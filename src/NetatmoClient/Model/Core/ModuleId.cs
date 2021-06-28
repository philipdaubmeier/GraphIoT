using System;
using System.Collections;
using System.Collections.Generic;
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
    public sealed record ModuleId(byte[] Bytes) : IComparable, IComparable<ModuleId>, IEquatable<ModuleId>
    {
        public static int Size => 6;

        public byte[] Bytes { get; private init; } = Bytes;

        public static ModuleId ReadFrom(Stream stream)
        {
            using var reader = new BinaryReader(stream, Encoding.UTF8, true);
            var array = reader.ReadBytes(Size);
            return new ModuleId(BitConverter.ToString(array));
        }

        public void WriteTo(Stream stream)
        {
            for (int i = 0; i < Bytes.Length; ++i)
                stream.WriteByte(Bytes[i]);
        }

        private static int GetHexVal(char hex) => hex - (hex < 58 ? 48 : 87);

        private static bool IsHexChar(char c) => (c >= 48 && c <= 57) || (c >= 97 && c <= 102);

        public static implicit operator ModuleId(string hex)
        {
            var bytes = new byte[Size];
            hex = new string(hex.ToLowerInvariant().ToCharArray().Where(c => IsHexChar(c)).ToArray());
            hex = hex.Substring(0, Math.Min(hex.Length, Size * 2)).PadLeft(Size * 2, '0');
            for (int i = 0; i < Math.Min(bytes.Length, hex.Length >> 1); ++i)
                bytes[i] = (byte)((GetHexVal(hex[i << 1]) << 4) + GetHexVal(hex[(i << 1) + 1]));
            return new ModuleId(bytes);
        }

        public static implicit operator string(ModuleId dsuid)
        {
            return BitConverter.ToString(dsuid.Bytes).ToLowerInvariant().Replace('-', ':');
        }

        public bool Equals(ModuleId? other)
        {
            if (other is null)
                return ReferenceEquals(this, other);

            return ((IStructuralEquatable)Bytes).Equals(other.Bytes, EqualityComparer<byte>.Default);
        }

        public override int GetHashCode() => ((string)this).GetHashCode();

        public int CompareTo(ModuleId? value) => ((string)this).CompareTo(value ?? string.Empty);

        public int CompareTo(object? value) => ((string)this).CompareTo((value as ModuleId) ?? value);

        public override string ToString() => this;
    }
}