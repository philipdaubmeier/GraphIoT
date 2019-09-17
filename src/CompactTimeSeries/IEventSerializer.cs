using System;
using System.IO;

namespace PhilipDaubmeier.CompactTimeSeries
{
    public interface IEventSerializer<TEvent> where TEvent : class
    {
        /// <summary>
        /// Returns the serialized size of TEvent in bytes. Has to be constant for all instances of TEvent.
        /// </summary>
        int SizeOf { get; }

        /// <summary>
        /// Returns the timestamp of the given event in UTC timezone
        /// </summary>
        DateTime GetTimestampUtc(TEvent eventObj);

        /// <summary>
        /// Returns whether or not the given object should be serialized to the stream.
        /// </summary>
        bool CanSerialize(TEvent eventObj);

        /// <summary>
        /// Writes the given event instance to the binary writer. The stream position is advanced exactly 'SizeOf' bytes.
        /// </summary>
        void Serialize(BinaryWriter writer, TEvent eventObj);

        /// <summary>
        /// Reads data from the stream reader into a new event object. The stream position is advanced exactly 'SizeOf' bytes.
        /// </summary>
        TEvent Deserialize(BinaryReader reader, DateTime timestampUtc);
    }
}