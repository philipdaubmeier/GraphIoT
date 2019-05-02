using System;
using System.IO;
using System.Linq;
using System.Text;

namespace PhilipDaubmeier.DigitalstromClient.Model.Core
{
    public class DSUID : IComparable, IComparable<DSUID>, IEquatable<DSUID>
    {
        public static int Size => 17;

        private string _hex;

        public DSUID(string hex)
        {
            _hex = new string(hex.ToLowerInvariant().ToCharArray().Where(c => IsHexChar(c)).ToArray());
            _hex = _hex.Substring(0, Math.Min(_hex.Length, Size * 2)).PadLeft(Size * 2, '0');
        }

        public static DSUID ReadFrom(Stream stream)
        {
            using (var reader = new BinaryReader(stream, Encoding.UTF8, true))
            {
                byte[] array = reader.ReadBytes(Size);
                return new DSUID(BitConverter.ToString(array).Replace("-", ""));
            }
        }

        public void WriteTo(Stream stream)
        {
            for (int i = 0; i < _hex.Length >> 1; ++i)
                stream.WriteByte((byte)((GetHexVal(_hex[i << 1]) << 4) + GetHexVal(_hex[(i << 1) + 1])));
        }

        private int GetHexVal(char hex) => hex - (hex < 58 ? 48 : 87);
        private bool IsHexChar(char c) => (c >= 48 && c <= 57) || (c >= 97 && c <= 102);

        public static implicit operator DSUID(string hex) => new DSUID(hex);
        public static implicit operator string(DSUID dsuid) => dsuid._hex;
        public int CompareTo(DSUID value) => _hex.CompareTo(value._hex);
        public int CompareTo(object value) => _hex.CompareTo((value as DSUID)?._hex ?? value);
        public override bool Equals(object o) => this == (o as DSUID);
        public bool Equals(DSUID g) => this == g;
        public override int GetHashCode() => _hex.GetHashCode();
        public static bool operator ==(DSUID a, DSUID b) => a._hex == b._hex;
        public static bool operator !=(DSUID a, DSUID b) => !(a == b);
        public override string ToString() => this;
    }
}