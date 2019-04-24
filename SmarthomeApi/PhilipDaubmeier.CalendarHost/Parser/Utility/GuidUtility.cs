using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace PhilipDaubmeier.CalendarHost.FormatParsers
{
    /// <summary>
    /// Helper methods for working with <see cref="Guid"/>.
    /// </summary>
    public static class GuidUtility
    {
        private const string OutlookByteArrayID = "040000008200E00074C5B7101A82E008";

        /// <summary>       
        /// There are two supported forms of textual representation of a ICal UID property
        /// in an ics file: either textual (UTF-8 string, like produced by Google Calendar
        /// for example) or an encoded global id that essentially contains a Guid (which is
        /// produced by Outlook for example). The Augmented Backus-Naur Form (ABNF) syntax,
        /// as specified in [RFC5234], for this value is shown in the following example:
        /// (see https://msdn.microsoft.com/en-us/library/ee202693(v=exchg.80).aspx)
        /// 
        /// UID = EncodedGlobalId / ThirdPartyGlobalId
        ///  
        /// EncodedGlobalId        = Header GlobalIdData
        /// ThirdPartyGlobalId     = 1*UTF8-octets     ; Assuming UTF-8 is the encoding
        /// 
        /// Header = ByteArrayID InstanceDate CreationDateTime Padding DataSize
        /// 
        /// ByteArrayID           = "040000008200E00074C5B7101A82E008"
        /// InstanceDate          = InstanceYear InstanceMonth InstanceDay
        /// InstanceYear          = 4*4HEXDIG     ; UInt16
        /// InstanceMonth         = 2*2HEXDIG     ; UInt8
        /// InstanceDay           = 2*2HEXDIG     ; UInt8
        /// CreationDateTime      = FileTime 
        /// FileTime              = 16*16HEXDIG   ; UInt64
        /// Padding               = 16*16HEXDIG   ; "0000000000000000" recommended
        /// DataSize              = 8*8HEXDIG     ; UInt32 little-endian
        /// GlobalIdData          = 2*HEXDIG Guid
        /// Guid                  = 32*32HEXDIG   ; The actual Guid we want to extract
        /// 
        /// UTF8-octets = *( UTF8-char )
        /// UTF8-char   = UTF8-1 / UTF8-2 / UTF8-3 / UTF8-4
        /// UTF8-1      = %x00-7F
        /// UTF8-2      = %xC2-DF UTF8-tail
        /// UTF8-3      = %xE0 %xA0-BF UTF8-tail / %xE1-EC 2( UTF8-tail ) /
        ///               %xED %x80-9F UTF8-tail / %xEE-EF 2( UTF8-tail )
        /// UTF8-4      = %xF0 %x90-BF 2( UTF8-tail ) / %xF1-F3 3( UTF8-tail ) /
        ///               %xF4 %x80-8F 2( UTF8-tail )
        /// UTF8-tail   = %x80-BF
        /// 
        /// Example for an EncodedGlobalId:
        /// ByteArrayID                      InstanceDate CreationDateTime Padding          DataSize Datasize Guid
        /// 040000008200E00074C5B7101A82E008 00000000     A092A619B086D401 0000000000000000 10000000 A1       91FBB6DB4D4C44B3E2BE9231490B06
        /// 
        /// Example for a ThirdPartyGlobalId:
        /// 7643616C2D55696401000000716138377132743535326465746637373335303870647263686340676F6F676C652E636F6D00
        /// which is decoded to "qa87q2t552detf773508pdrchc@google.com"
        /// </summary>
        /// <param name="uid">The UID from the ics ICal file.</param>
        /// <returns>A UUID either extracted from the UID or a name-based UUID derived from the text-based UID.</returns>
        public static Guid FromIcsUID(string uid)
        {
            if (string.IsNullOrWhiteSpace(uid))
                new Guid();

            // Check if we have an Outlook EncodedGlobalId, if not just create a name-based UUID from the string
            var uidStr = uid.ToUpperInvariant().Trim();
            if (!uidStr.StartsWith(OutlookByteArrayID) || uidStr.Length <= 32
                || !Guid.TryParseExact(uidStr.Substring(uidStr.Length - 32, 32), "N", out Guid extractedGuid))
                return Create(UrlNamespace, uid);

            return extractedGuid;
        }

        /// <summary>
        /// Creates a name-based UUID using the algorithm from RFC 4122 §4.3.
        /// </summary>
        /// <param name="namespaceId">The ID of the namespace.</param>
        /// <param name="name">The name (within that namespace).</param>
        /// <returns>A UUID derived from the namespace and name.</returns>
        public static Guid Create(Guid namespaceId, string name) => Create(namespaceId, name, 5);

        /// <summary>
        /// Creates a name-based UUID using the algorithm from RFC 4122 §4.3.
        /// </summary>
        /// <param name="namespaceId">The ID of the namespace.</param>
        /// <param name="name">The name (within that namespace).</param>
        /// <param name="version">The version number of the UUID to create; this value must be either
        /// 3 (for MD5 hashing) or 5 (for SHA-1 hashing).</param>
        /// <returns>A UUID derived from the namespace and name.</returns>
        public static Guid Create(Guid namespaceId, string name, int version)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));

            // convert the name to a sequence of octets (as defined by the standard or conventions of its namespace) (step 3)
            // ASSUME: UTF-8 encoding is always appropriate
            byte[] nameBytes = Encoding.UTF8.GetBytes(name);

            return Create(namespaceId, nameBytes, version);
        }

        /// <summary>
        /// Creates a name-based UUID using the algorithm from RFC 4122 §4.3.
        /// </summary>
        /// <param name="namespaceId">The ID of the namespace.</param>
        /// <param name="nameBytes">The name (within that namespace).</param>
        /// <returns>A UUID derived from the namespace and name.</returns>
        public static Guid Create(Guid namespaceId, byte[] nameBytes) => Create(namespaceId, nameBytes, 5);

        /// <summary>
        /// Creates a name-based UUID using the algorithm from RFC 4122 §4.3.
        /// </summary>
        /// <param name="namespaceId">The ID of the namespace.</param>
        /// <param name="nameBytes">The name (within that namespace).</param>
        /// <param name="version">The version number of the UUID to create; this value must be either
        /// 3 (for MD5 hashing) or 5 (for SHA-1 hashing).</param>
        /// <returns>A UUID derived from the namespace and name.</returns>
        public static Guid Create(Guid namespaceId, byte[] nameBytes, int version)
        {
            if (version != 3 && version != 5)
                throw new ArgumentOutOfRangeException(nameof(version), "version must be either 3 or 5.");

            // convert the namespace UUID to network order (step 3)
            byte[] namespaceBytes = namespaceId.ToByteArray();
            SwapByteOrder(namespaceBytes);

            // compute the hash of the namespace ID concatenated with the name (step 4)
            byte[] data = namespaceBytes.Concat(nameBytes).ToArray();
            byte[] hash;
            using (HashAlgorithm algorithm = version == 3 ? (HashAlgorithm)MD5.Create() : SHA1.Create())
                hash = algorithm.ComputeHash(data);

            // most bytes from the hash are copied straight to the bytes of the new GUID (steps 5-7, 9, 11-12)
            byte[] newGuid = new byte[16];
            Array.Copy(hash, 0, newGuid, 0, 16);

            // set the four most significant bits (bits 12 through 15) of the time_hi_and_version field to the appropriate 4-bit version number from Section 4.1.3 (step 8)
            newGuid[6] = (byte)((newGuid[6] & 0x0F) | (version << 4));

            // set the two most significant bits (bits 6 and 7) of the clock_seq_hi_and_reserved to zero and one, respectively (step 10)
            newGuid[8] = (byte)((newGuid[8] & 0x3F) | 0x80);

            // convert the resulting UUID to local byte order (step 13)
            SwapByteOrder(newGuid);
            return new Guid(newGuid);
        }

        /// <summary>
        /// The namespace for fully-qualified domain names (from RFC 4122, Appendix C).
        /// </summary>
        public static readonly Guid DnsNamespace = new Guid("6ba7b810-9dad-11d1-80b4-00c04fd430c8");

        /// <summary>
        /// The namespace for URLs (from RFC 4122, Appendix C).
        /// </summary>
        public static readonly Guid UrlNamespace = new Guid("6ba7b811-9dad-11d1-80b4-00c04fd430c8");

        /// <summary>
        /// The namespace for ISO OIDs (from RFC 4122, Appendix C).
        /// </summary>
        public static readonly Guid IsoOidNamespace = new Guid("6ba7b812-9dad-11d1-80b4-00c04fd430c8");

        // Converts a GUID (expressed as a byte array) to/from network order (MSB-first).
        internal static void SwapByteOrder(byte[] guid)
        {
            SwapBytes(guid, 0, 3);
            SwapBytes(guid, 1, 2);
            SwapBytes(guid, 4, 5);
            SwapBytes(guid, 6, 7);
        }

        private static void SwapBytes(byte[] guid, int left, int right)
        {
            byte temp = guid[left];
            guid[left] = guid[right];
            guid[right] = temp;
        }
    }
}