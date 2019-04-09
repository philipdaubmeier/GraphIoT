using CompactTimeSeries;
using SmarthomeApi.FormatParsers;
using SmarthomeApi.Model;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
        public SceneEventStream EventStream
        {
            get => EventStreamEncoded.ToEventStream(new TimeSeriesSpan(Day, Day.AddDays(1), MaxEventsPerDay));
            set { EventStreamEncoded = value.ToBase64(); }
        }
    }
}