using Microsoft.Extensions.DependencyInjection;
using PhilipDaubmeier.CompactTimeSeries;
using PhilipDaubmeier.DigitalstromClient.Model.Core;
using PhilipDaubmeier.DigitalstromClient.Model.Events;
using PhilipDaubmeier.GraphIoT.Digitalstrom.Database;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PhilipDaubmeier.GraphIoT.Digitalstrom.EventProcessing
{
    public class DssSceneEventProcessorPlugin : IDigitalstromEventProcessorPlugin
    {
        private static IEnumerable<IEventName> AcceptedEvents
        {
            get
            {
                yield return (SystemEventName)SystemEvent.CallScene;
                yield return (SystemEventName)SystemEvent.CallSceneBus;
            }
        }

        public class DssSceneEventSerializer : IEventSerializer<DssEvent>
        {
            /// <summary>
            /// See <see cref="IEventSerializer{TEvent}.SizeOf"/>
            /// </summary>
            public int SizeOf => sizeof(short) + 2 * sizeof(byte);

            /// <summary>
            /// See <see cref="IEventSerializer{TEvent}.GetTimestampUtc(TEvent)"/>
            /// </summary>
            public DateTime GetTimestampUtc(DssEvent eventObj) => eventObj?.TimestampUtc ?? DateTime.MinValue;

            /// <summary>
            /// See <see cref="IEventSerializer{TEvent}.CanSerialize(TEvent)"/>
            /// </summary>
            public bool CanSerialize(DssEvent eventObj) => AcceptedEvents.Contains(eventObj.SystemEvent);

            /// <summary>
            /// See <see cref="IEventSerializer{TEvent}.Serialize(BinaryWriter, TEvent)"/>
            /// </summary>
            public void Serialize(BinaryWriter writer, DssEvent eventObj)
            {
                if (writer == null)
                    throw new ArgumentNullException("writer");

                if (eventObj == null)
                    throw new ArgumentNullException("eventObj");

                writer.Write((ushort)Math.Min(Math.Max(eventObj.Properties.ZoneID, 0), ushort.MaxValue));
                writer.Write((byte)Math.Min(Math.Max(eventObj.Properties.GroupID, 0), byte.MaxValue));
                writer.Write((byte)Math.Min(Math.Max(eventObj.Properties.SceneID, 0), byte.MaxValue));
            }

            /// <summary>
            /// See <see cref="IEventSerializer{TEvent}.Deserialize(BinaryReader, DateTime)"/>
            /// </summary>
            public DssEvent Deserialize(BinaryReader reader, DateTime timestampUtc)
            {
                if (reader == null)
                    throw new ArgumentNullException("reader");

                var zone = (Zone)reader.ReadUInt16();
                var group = (Group)reader.ReadByte();
                var scene = (Scene)reader.ReadByte();
                var props = new DssEventProperties()
                {
                    ZoneID = ((int)zone).ToString(),
                    GroupID = ((int)group).ToString(),
                    SceneID = ((int)scene).ToString(),
                    OriginDSUID = string.Empty,
                    OriginToken = string.Empty,
                    CallOrigin = string.Empty
                };
                var dssevent = new DssEvent(timestampUtc)
                {
                    Name = new SystemEventName(SystemEvent.CallScene).Name,
                    Properties = props,
                    Source = new DssEventSource()
                };

                return dssevent;
            }
        }

        private readonly IServiceScopeFactory _serviceScopeFactory;
        private EventTimeSeriesStream<DssEvent, DssSceneEventSerializer> _eventStream = null;
        private bool _hasChanges = false;

        public DssSceneEventProcessorPlugin(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        /// <summary>
        /// See <see cref="IDssEventProcessorPlugin.EventNames"/>
        /// </summary>
        public IEnumerable<IEventName> EventNames => AcceptedEvents;

        /// <summary>
        /// See <see cref="IDssEventProcessorPlugin.Span"/>
        /// </summary>
        public TimeSeriesSpan Span => _eventStream?.Span;

        /// <summary>
        /// See <see cref="IDssEventProcessorPlugin.WriteToEventStream(DssEvent)"/>
        /// </summary>
        public void WriteToEventStream(DssEvent dsEvent)
        {
            _eventStream.WriteEvent(dsEvent);
            _hasChanges = true;
        }

        /// <summary>
        /// See <see cref="IDssEventProcessorPlugin.HasDuplicate(DssEvent, int)"/>
        /// </summary>
        public bool HasDuplicate(DssEvent dsEvent, int milliseconds)
        {
            var lastEvent = _eventStream.LastOrDefault();
            if (lastEvent == null)
                return false;

            return lastEvent.TimestampUtc.AddMilliseconds(milliseconds) > dsEvent.TimestampUtc
                && lastEvent.Properties.ZoneID == dsEvent.Properties.ZoneID
                && lastEvent.Properties.GroupID == dsEvent.Properties.GroupID
                && lastEvent.Properties.SceneID == dsEvent.Properties.SceneID;
        }

        /// <summary>
        /// See <see cref="IDssEventProcessorPlugin.ReadOrCreateEventStream(DateTime)"/>
        /// </summary>
        public void ReadOrCreateEventStream(DateTime date)
        {
            // We already have an event stream for the current day, so just use this
            if (_eventStream != null && Span.IsIncluded(date))
                return;

            // We have reached the day boundary - save the current event stream and create a new one for the new day
            if (_eventStream != null && !Span.IsIncluded(date))
                SaveEventStreamToDb();

            using (var scope = _serviceScopeFactory.CreateScope())
            using (var dbContext = scope.ServiceProvider.GetRequiredService<IDigitalstromDbContext>())
            {
                var dbSceneEvents = dbContext.DsSceneEventDataSet.Where(x => x.Key == date).FirstOrDefault();
                if (dbSceneEvents != null)
                    _eventStream = dbSceneEvents.EventStream;
                else
                    _eventStream = new EventTimeSeriesStream<DssEvent, DssSceneEventSerializer>(new TimeSeriesSpan(date, date.AddDays(1), DigitalstromSceneEventData.MaxEventsPerDay));
            }
        }

        /// <summary>
        /// See <see cref="IDssEventProcessorPlugin.SaveEventStreamToDb()"/>
        /// </summary>
        public void SaveEventStreamToDb()
        {
            if (!_hasChanges)
                return;

            var date = Span.Begin.Date;

            using (var scope = _serviceScopeFactory.CreateScope())
            using (var dbContext = scope.ServiceProvider.GetRequiredService<IDigitalstromDbContext>())
            {
                var dbSceneEvents = dbContext.DsSceneEventDataSet.Where(x => x.Key == date).FirstOrDefault();
                if (dbSceneEvents == null)
                    dbContext.DsSceneEventDataSet.Add(dbSceneEvents = new DigitalstromSceneEventData() { Key = date });

                dbSceneEvents.EventStream = _eventStream;

                dbContext.SaveChanges();

                _hasChanges = false;
            }
        }
    }
}