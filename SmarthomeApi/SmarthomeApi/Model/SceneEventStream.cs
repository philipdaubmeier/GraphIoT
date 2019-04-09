using CompactTimeSeries;
using DigitalstromClient.Model.Core;
using DigitalstromClient.Model.Events;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SmarthomeApi.Model
{
    public class SceneEventStream : IEnumerable<DssEvent>, IDisposable
    {
        private Stream _stream;
        private bool _isStreamManaged = false;
        private int _startPosition = 0;
        private int _count = 0;
        private static int _sizeOfEvent = (sizeof(int) + sizeof(short) + 2 * sizeof(byte));

        /// <summary>
        /// Creates a new SceneEventStream object with the start time and max number of events given in the span
        /// </summary>
        public SceneEventStream(TimeSeriesSpan span)
            : this(new MemoryStream(span.Count * _sizeOfEvent), 0, span)
        {
            _isStreamManaged = true;

            for (int i = 0; i < span.Count; i++)
            {
                using (var writer = new BinaryWriter(_stream, Encoding.UTF8, true))
                {
                    writer.Write(-1);
                    writer.Write(0);
                }
            }
        }

        /// <summary>
        /// Creates a new SceneEventStream object with an existing underlying stream that has to be
        /// readable, writable and seekable. The event series starts at the given streamPosition.
        /// </summary>
        public SceneEventStream(Stream stream, int streamPosition, TimeSeriesSpan span)
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

        public static SceneEventStream FromByteArray(TimeSeriesSpan span, byte[] array)
        {
            // create a SceneEventStream with a self-managed stream and copy over the array
            var eventstream = new SceneEventStream(span);
            eventstream._stream.Seek(0, SeekOrigin.Begin);
            (eventstream._stream as MemoryStream)?.Write(array, 0, Math.Min((int)eventstream._stream.Length, array.Length));

            // restore count by iterating until no more valid DssEvents are found
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

        public IEnumerator<DssEvent> GetEnumerator()
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

        public void WriteEvent(DssEvent dssevent)
        {
            if (dssevent == null)
                return;

            if (dssevent.TimestampUtc < Span.Begin.ToUniversalTime() ||
                (dssevent.TimestampUtc - Span.Begin.ToUniversalTime()).TotalMilliseconds > int.MaxValue)
                return;

            if (dssevent.systemEvent != SystemEventName.EventType.CallScene &&
                dssevent.systemEvent != SystemEventName.EventType.CallSceneBus)
                return;

            _stream.Seek(_startPosition + (_count * _sizeOfEvent), SeekOrigin.Begin);
            using (var writer = new BinaryWriter(_stream, Encoding.UTF8, true))
            {
                writer.Write((int)(dssevent.TimestampUtc - Span.Begin.ToUniversalTime()).TotalMilliseconds);
                writer.Write((ushort)Math.Min(Math.Max(dssevent.properties.zone, 0), ushort.MaxValue));
                writer.Write((byte)Math.Min(Math.Max(dssevent.properties.group, 0), byte.MaxValue));
                writer.Write((byte)Math.Min(Math.Max(dssevent.properties.scene, 0), byte.MaxValue));
            }
            _count++;
        }
        
        private DssEvent ReadEvent()
        {
            using (var reader = new BinaryReader(_stream, Encoding.UTF8, true))
            {
                var millis = reader.ReadInt32();
                if (millis < 0)
                    return null;

                var timestamp = Span.Begin.AddMilliseconds(millis);
                var zone = (Zone)reader.ReadUInt16();
                var group = (Group)reader.ReadByte();
                var scene = (Scene)reader.ReadByte();
                var props = new DssEventProperties()
                {
                    zoneID = ((int)zone).ToString(),
                    groupID = ((int)group).ToString(),
                    sceneID = ((int)scene).ToString(),
                    originDSUID = string.Empty,
                    originToken = string.Empty,
                    callOrigin = string.Empty
                };
                var dssevent = new DssEvent(timestamp)
                {
                    name = new SystemEventName(SystemEventName.EventType.CallScene).name,
                    properties = props,
                    source = new DssEventSource()
                };

                return dssevent;
            }
        }
    }
}