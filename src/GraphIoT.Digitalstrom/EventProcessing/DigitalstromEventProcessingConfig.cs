using PhilipDaubmeier.GraphIoT.Core.Parsers;
using System;

namespace PhilipDaubmeier.GraphIoT.Digitalstrom.EventProcessing
{
    public class DigitalstromEventProcessingConfig
    {
        public string ItemCollectionInterval { get; set; }

        public TimeSpan ItemCollectionTimeSpan => ItemCollectionInterval.ToTimeSpan();

        public string DuplicateDetectionInterval { get; set; }

        public TimeSpan DuplicateDetectionTimeSpan => DuplicateDetectionInterval.ToTimeSpan();
    }
}