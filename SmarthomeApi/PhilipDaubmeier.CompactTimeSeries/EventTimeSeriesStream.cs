using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace PhilipDaubmeier.CompactTimeSeries
{
    /// <summary>
    /// Generic class for representing unevenly spaced time series for events that happen irregularly in time.
    /// </summary>
    public class EventTimeSeriesStream<TEvent, TSerializer> : IEnumerable<TEvent>, IDisposable
        where TEvent : class
        where TSerializer : IEventSerializer<TEvent>, new()
    {
        private readonly Stream _stream;
        private readonly bool _isStreamManaged = false;
        private readonly int _startPosition = 0;
        private int _count = 0;
        private readonly TSerializer _serializer = new TSerializer();

        /// <summary>
        /// Creates a new EventTimeSeriesStream object with the start time and max number of events given in the span
        /// </summary>
        public EventTimeSeriesStream(TimeSeriesSpan span)
            : this(new MemoryStream(span.Count * (sizeof(int) + new TSerializer().SizeOf)), 0, span)
        {
            _isStreamManaged = true;

            var nullEventBytes = Enumerable.Range(0, _serializer.SizeOf).Select(b => (byte)0).ToArray();
            using (var writer = new BinaryWriter(_stream, Encoding.UTF8, true))
            {
                for (int i = 0; i < span.Count; i++)
                {
                    writer.Write(-1);
                    writer.Write(nullEventBytes);
                }
            }
        }

        /// <summary>
        /// Creates a new EventTimeSeriesStream object with an existing underlying stream that has to be
        /// readable, writable and seekable. The event series starts at the given streamPosition.
        /// </summary>
        public EventTimeSeriesStream(Stream stream, int streamPosition, TimeSeriesSpan span)
        {
            if (stream == null)
                throw new ArgumentException("underlying stream can not be null");

            if (!stream.CanSeek || !stream.CanRead || !stream.CanWrite)
                throw new ArgumentException("underlying stream must be readable, writable and seekable");

            Span = span;
            _stream = stream;
            _startPosition = streamPosition;
        }

        public void Dispose()
        {
            if (_isStreamManaged)
                _stream.Dispose();
        }

        public static EventTimeSeriesStream<TEvent, TSerializer> FromByteArray(TimeSeriesSpan span, byte[] array)
        {
            // create a SceneEventStream with a self-managed stream and copy over the array
            var eventstream = new EventTimeSeriesStream<TEvent, TSerializer>(span);
            eventstream._stream.Seek(0, SeekOrigin.Begin);
            (eventstream._stream as MemoryStream)?.Write(array, 0, Math.Min((int)eventstream._stream.Length, array.Length));

            // restore count by iterating until no more non-null events are found
            var count = 0;
            foreach (var item in eventstream)
                count++;
            eventstream._count = count;

            return eventstream;
        }

        public byte[] ToByteArray()
        {
            return (_stream as MemoryStream)?.ToArray();
        }

        public TimeSeriesSpan Span { get; }

        public IEnumerator<TEvent> GetEnumerator()
        {
            _stream.Seek(_startPosition, SeekOrigin.Begin);
            
            for (int i = 0; i < Span.Count; i++)
            {
                var item = ReadEvent();
                if (item == null)
                    yield break;

                yield return item;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void WriteEvent(TEvent eventItem)
        {
            if (eventItem == null)
                return;

            var eventTimestamp = _serializer.GetTimestampUtc(eventItem);
            var begin = Span.Begin.ToUniversalTime();
            if (eventTimestamp < begin || (eventTimestamp - begin).TotalMilliseconds > int.MaxValue)
                return;

            if (!_serializer.CanSerialize(eventItem))
                return;

            _stream.Seek(_startPosition + (_count * (sizeof(int) + _serializer.SizeOf)), SeekOrigin.Begin);
            using (var writer = new BinaryWriter(_stream, Encoding.UTF8, true))
            {
                writer.Write((int)(eventTimestamp - begin).TotalMilliseconds);
                _serializer.Serialize(writer, eventItem);
            }
            _count++;
        }
        
        private TEvent ReadEvent()
        {
            using (var reader = new BinaryReader(_stream, Encoding.UTF8, true))
            {
                var millis = reader.ReadInt32();
                if (millis < 0)
                    return null;

                var timestamp = Span.Begin.ToUniversalTime().AddMilliseconds(millis);
                return _serializer.Deserialize(reader, timestamp);
            }
        }
    }
}