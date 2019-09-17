using PhilipDaubmeier.CompactTimeSeries;
using PhilipDaubmeier.TimeseriesHostCommon.Database;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SceneEventStream = PhilipDaubmeier.CompactTimeSeries.EventTimeSeriesStream<PhilipDaubmeier.DigitalstromClient.Model.Events.DssEvent, PhilipDaubmeier.DigitalstromHost.EventProcessing.DssSceneEventProcessorPlugin.DssSceneEventSerializer>;

namespace PhilipDaubmeier.DigitalstromHost.Database
{
    public class DigitalstromSceneEventData : ITimeSeriesDbEntity
    {
        // 935 events times 8 byte times base64 encoding factor are <10000 characters string, which is the db column limit
        public static int MaxEventsPerDay => 935;

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Required, Column("Day")]
        public DateTime Key { get; set; }

        [MaxLength(10000)]
        public string EventStreamEncoded { get; set; }

        [NotMapped]
        public TimeSeriesSpan Span => new TimeSeriesSpan(Key, Key.AddDays(1), MaxEventsPerDay);

        [NotMapped]
        public SceneEventStream EventStream
        {
            get => SceneEventStream.FromByteArray(Span, Convert.FromBase64String(EventStreamEncoded));
            set { EventStreamEncoded = Convert.ToBase64String(value.ToByteArray()); }
        }

        public TimeSeries<T> GetSeries<T>(int index) where T : struct => null;
        public void SetSeries<T>(int index, TimeSeries<T> series) where T : struct { }
    }
}