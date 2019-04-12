using System.IO;
using System.IO.Compression;

namespace PhilipDaubmeier.CompactTimeSeries
{
    public class CompressableMemoryStream : MemoryStream
    {
        private const double _compressionEstimate = 0.2d;

        /// <summary>
        /// Initializes a new instance of the CompressableMemoryStream class with an expandable 
        /// capacity initialized as specified.
        /// </summary>
        public CompressableMemoryStream(int capacity) : base(capacity) { }

        /// <summary>
        /// Returns a GZipped byte array with the contents of this MemoryStream
        /// </summary>
        public byte[] ToCompressedByteArray()
        {
            using (var compressedStream = new MemoryStream((int)(Length * _compressionEstimate)))
            using (var compressionStream = new GZipStream(compressedStream, CompressionMode.Compress, true))
            {
                Position = 0;
                CopyTo(compressionStream);
                compressionStream.Close();
                return compressedStream.ToArray();
            }
        }

        /// <summary>
        /// Initializes a CompressableMemoryStream from a GZipped byte array.
        /// </summary>
        public static CompressableMemoryStream FromCompressedByteArray(byte[] byteArray)
        {
            var stream = new CompressableMemoryStream((int)(byteArray.Length / _compressionEstimate));
            using (var compressedStream = new MemoryStream(byteArray, 0, byteArray.Length, false) { Position = 0 })
            using (var decompressionStream = new GZipStream(compressedStream, CompressionMode.Decompress))
            {
                decompressionStream.CopyTo(stream);
                stream.Position = 0;
            }
            return stream;
        }
    }
}