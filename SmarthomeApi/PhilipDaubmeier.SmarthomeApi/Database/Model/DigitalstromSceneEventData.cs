using PhilipDaubmeier.CompactTimeSeries;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SceneEventStream = PhilipDaubmeier.CompactTimeSeries.EventTimeSeriesStream<PhilipDaubmeier.DigitalstromClient.Model.Events.DssEvent, SmarthomeApi.Services.EventProcessing.DssSceneEventProcessorPlugin.DssSceneEventSerializer>;

namespace SmarthomeApi.Database.Model
{
    public class DigitalstromSceneEventData
    {
        // 935 events times 8 byte times base64 encoding factor are <10000 characters string, which is the db column limit
        public static int MaxEventsPerDay => 935;

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Required]
        public DateTime Day { get; set; }

        [MaxLength(10000)]
        public string EventStreamEncoded { get; set; }

        [NotMapped]
        public TimeSeriesSpan Span => new TimeSeriesSpan(Day, Day.AddDays(1), MaxEventsPerDay);

        [NotMapped]
        public SceneEventStream EventStream
        {
            get => SceneEventStream.FromByteArray(Span, Convert.FromBase64String(EventStreamEncoded));
            set { EventStreamEncoded = Convert.ToBase64String(value.ToByteArray()); }
        }
    }
}